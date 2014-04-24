namespace Ninject
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ninject.Activation;
    using Ninject.Activation.Caching;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Disposal;
    using Ninject.Infrastructure.Introspection;
    using Ninject.Infrastructure.Language;
    using Ninject.Parameters;
    using Ninject.Planning;
    using Ninject.Planning.Bindings;
    using Ninject.Planning.Bindings.Resolvers;
    using Ninject.Selection;
    using Ninject.Syntax;

    /// <summary>
    /// The readonly kernel
    /// </summary>
    public class ReadonlyKernel : DisposableObject, IReadonlyKernel
    {
        private readonly ICache cache;
        private readonly IPlanner planner;
        private readonly IPipeline pipeline;
        private readonly IBindingPrecedenceComparer bindingPrecedenceComparer;
        private readonly IEnumerable<IBindingResolver> bindingResolvers;
        private readonly IEnumerable<IMissingBindingResolver> missingBindingResolvers;
        private readonly object missingBindingCacheLock = new object();

        private Dictionary<Type, List<IBinding>> bindingCache = new Dictionary<Type, List<IBinding>>();
        private Dictionary<Type, IEnumerable<IBinding>> bindings = new Dictionary<Type, IEnumerable<IBinding>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadonlyKernel"/> class.
        /// </summary>
        /// <param name="bindings">The preconfigured bindings</param>
        /// <param name="cache">Dependency injection for <see cref="ICache"/></param>
        /// <param name="planner">Dependency injection for <see cref="IPlanner"/></param>
        /// <param name="pipeline">Dependency injection for <see cref="IPipeline"/></param>
        /// <param name="bindingPrecedenceComparer">Dependency injection for <see cref="IBindingPrecedenceComparer"/></param>
        /// <param name="bindingResolvers">Dependency injection for all binding resolvers</param>
        /// <param name="missingBindingResolvers">Dependency injection for all missng binding resolvers</param>
        /// <param name="settings">Dependency injection for for <see cref="INinjectSettings"/></param>
        /// <param name="selector">Dependency injection for <see cref="ISelector"/></param>
        public ReadonlyKernel(
            Multimap<Type, IBinding> bindings, 
            ICache cache, 
            IPlanner planner, 
            IPipeline pipeline, 
            IBindingPrecedenceComparer bindingPrecedenceComparer,
            IEnumerable<IBindingResolver> bindingResolvers,
            IEnumerable<IMissingBindingResolver> missingBindingResolvers,
            INinjectSettings settings,
            ISelector selector)
        {
            this.bindingResolvers = bindingResolvers;
            this.missingBindingResolvers = missingBindingResolvers;
            this.cache = cache;
            this.planner = planner;
            this.pipeline = pipeline;
            this.bindingPrecedenceComparer = bindingPrecedenceComparer;
            this.Selector = selector;
            this.Settings = settings;

            this.AddReadonlyKernelBinding<IReadonlyKernel>(this, bindings);
            this.AddReadonlyKernelBinding<IResolutionRoot>(this, bindings);

            this.bindings = bindings.Keys.ToDictionary(type => type, type => bindings[type]);
        }

        private void AddReadonlyKernelBinding<T>(T readonlyKernel, Multimap<Type, IBinding> bindings)
        {
            var binding = new Binding(typeof(T));
            new BindingBuilder<T>(binding, this.Settings, typeof(T).Format()).ToConstant(readonlyKernel);
            bindings.Add(typeof(T), binding);
        }

        /// <inheritdoc />
        public void Inject(object instance, params IParameter[] parameters)
        {
            Ensure.ArgumentNotNull(instance, "instance");
            Ensure.ArgumentNotNull(parameters, "parameters");

            Type service = instance.GetType();

            var binding = new Binding(service);
            var request = this.CreateRequest(service, null, parameters, false, false);
            var context = this.CreateContext(request, binding);

            context.Plan = this.planner.GetPlan(service);

            var reference = new InstanceReference { Instance = instance };
            this.pipeline.Activate(context, reference);
        }

        /// <inheritdoc />
        public bool CanResolve(IRequest request)
        {
            Ensure.ArgumentNotNull(request, "request");
            return this.GetBindings(request.Service).Any(this.SatifiesRequest(request));
        }

        /// <inheritdoc />
        public bool CanResolve(IRequest request, bool ignoreImplicitBindings)
        {
            Ensure.ArgumentNotNull(request, "request");
            return this.GetBindings(request.Service)
                .Any(binding => (!ignoreImplicitBindings || !binding.IsImplicit) && this.SatifiesRequest(request)(binding));
        }

        /// <inheritdoc />
        public IEnumerable<object> Resolve(IRequest request)
        {
            Ensure.ArgumentNotNull(request, "request");

            var resolveBindings = Enumerable.Empty<IBinding>();

            if (this.CanResolve(request) || this.HandleMissingBinding(request))
            {
                resolveBindings = this.GetBindings(request.Service)
                                      .Where(this.SatifiesRequest(request));
            }

            if (!resolveBindings.Any())
            {
                if (request.IsOptional)
                {
                    return Enumerable.Empty<object>();
                }

                throw new ActivationException(ExceptionFormatter.CouldNotResolveBinding(request));
            }

            if (request.IsUnique)
            {
                resolveBindings = resolveBindings.OrderByDescending(b => b, this.bindingPrecedenceComparer).ToList();
                var model = resolveBindings.First(); // the type (conditonal, implicit, etc) of binding we'll return
                resolveBindings =
                    resolveBindings.TakeWhile(binding => this.bindingPrecedenceComparer.Compare(binding, model) == 0);

                if (resolveBindings.Count() > 1)
                {
                    if (request.IsOptional && !request.ForceUnique)
                    {
                        return Enumerable.Empty<object>();
                    }

                    var formattedBindings =
                        from binding in resolveBindings
                        let context = this.CreateContext(request, binding)
                        select binding.Format(context);
                    throw new ActivationException(ExceptionFormatter.CouldNotUniquelyResolveBinding(request, formattedBindings.ToArray()));
                }
            }

            if (resolveBindings.Any(binding => !binding.IsImplicit))
            {
                resolveBindings = resolveBindings.Where(binding => !binding.IsImplicit);
            }

            return resolveBindings
                .Select(binding => this.CreateContext(request, binding).Resolve());
        }

        /// <inheritdoc />
        public IRequest CreateRequest(Type service, Func<IBindingMetadata, bool> constraint, IEnumerable<IParameter> parameters, bool isOptional, bool isUnique)
        {
            Ensure.ArgumentNotNull(service, "service");
            Ensure.ArgumentNotNull(parameters, "parameters");

            return new Request(service, constraint, parameters, null, isOptional, isUnique);
        }

        /// <inheritdoc />
        public bool Release(object instance)
        {
            Ensure.ArgumentNotNull(instance, "instance");
            return this.cache.Release(instance);
        }

        /// <inheritdoc />
        public INinjectSettings Settings { get; private set; }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            return this.Get(serviceType);
        }

        /// <inheritdoc />
        public override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                this.cache.Clear();
            }

            base.Dispose(disposing);
        }

        /// <inheritdoc />
        public IEnumerable<IBinding> GetBindings(Type service)
        {
            Ensure.ArgumentNotNull(service, "service");

            List<IBinding> result;
            if (this.bindingCache.TryGetValue(service, out result))
            {
                return result;
            }

            var newBindingCache = new Dictionary<Type, List<IBinding>>(this.bindingCache);
            if (newBindingCache.TryGetValue(service, out result))
            {
                return result;
            }

            result = bindingResolvers.SelectMany(resolver => resolver.Resolve(this.bindings, service)).ToList();
            if (result.Count > 0)
            {
                newBindingCache[service] = result;

                // We might loose other entries in case multiple cache entries are added at the same time
                // But this is no problem in this case they will be added later again.
                this.bindingCache = newBindingCache;
            }

            return result;
        }

        // Todo: Remove
        /// <summary>
        /// Gets the selector
        /// </summary>
        public ISelector Selector { get; private set; }

        /// <summary>
        /// Returns a predicate that can determine if a given IBinding matches the request.
        /// </summary>
        /// <param name="request">The request/</param>
        /// <returns>A predicate that can determine if a given IBinding matches the request.</returns>
        protected virtual Func<IBinding, bool> SatifiesRequest(IRequest request)
        {
            return binding => binding.Matches(request) && request.Matches(binding);
        }

        /// <summary>
        /// Attempts to handle a missing binding for a request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns><c>True</c> if the missing binding can be handled; otherwise <c>false</c>.</returns>
        protected virtual bool HandleMissingBinding(IRequest request)
        {
            Ensure.ArgumentNotNull(request, "request");

            var bindings = this.GetBindingsFromFirstResolverThatReturnsAtLeastOneBinding(request);
            if (bindings == null)
            {
                return false;
            }

            bindings.Map(binding => binding.IsImplicit = true);

            lock (this.missingBindingCacheLock)
            {
                if (CanResolve(request)) return true;

                var newBindings = new Dictionary<Type, IEnumerable<IBinding>>(this.bindings);
                bindings.GroupBy(b => b.Service, b => b, (service, b) => new { service, bindings = b })
                    .Map(
                        serviceGroup =>
                            {
                                IEnumerable<IBinding> existingBindings;
                                if (newBindings.TryGetValue(serviceGroup.service, out existingBindings))
                                {
                                    var newBindingList = new List<IBinding>(existingBindings);
                                    newBindingList.AddRange(serviceGroup.bindings);
                                    newBindings[serviceGroup.service] = newBindingList;
                                }
                                else
                                {
                                    newBindings[serviceGroup.service] =
                                        new List<IBinding>(serviceGroup.bindings);
                                }
                            });

                this.bindings = newBindings;
            }

            return true;
        }

        private List<IBinding> GetBindingsFromFirstResolverThatReturnsAtLeastOneBinding(IRequest request)
        {
            return this.missingBindingResolvers
                .Select(c => c.Resolve(this.bindings, request).ToList())
                .FirstOrDefault(b => b.Any());
        }

        /// <summary>
        /// Creates a context for the specified request and binding.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="binding">The binding.</param>
        /// <returns>The created context.</returns>
        protected virtual IContext CreateContext(IRequest request, IBinding binding)
        {
            Ensure.ArgumentNotNull(request, "request");
            Ensure.ArgumentNotNull(binding, "binding");

            return new Context(this, request, binding, this.cache, this.planner, this.pipeline);
        }
    }
}