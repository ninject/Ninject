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

    public class ReadonlyKernel : DisposableObject, IReadonlyKernel
    {
        /// <summary>
        /// Lock used when adding missing bindings.
        /// </summary>
        //protected readonly object HandleMissingBindingLockObject = new object();

        private readonly Multimap<Type, IBinding> bindings;

        private readonly Multimap<Type, IBinding> bindingCache = new Multimap<Type, IBinding>();

        private readonly ICache cache;

        private readonly IPlanner planner;

        private readonly IPipeline pipeline;

        private IEnumerable<IBindingResolver> bindingResolvers;

        public ReadonlyKernel(
            Multimap<Type, IBinding> bindings, 
            ICache cache, 
            IPlanner planner, 
            IPipeline pipeline, 
            IEnumerable<IBindingResolver> bindingResolvers,
            INinjectSettings settings,
            ISelector selector)
        {
            this.bindings = bindings;

            this.bindingResolvers = bindingResolvers;
            this.cache = cache;
            this.planner = planner;
            this.pipeline = pipeline;
            this.Planner = planner;
            this.Selector = selector;
            this.Settings = settings;
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

            var bindingPrecedenceComparer = this.GetBindingPrecedenceComparer();
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
                resolveBindings = resolveBindings.OrderByDescending(b => b, bindingPrecedenceComparer).ToList();
                var model = resolveBindings.First(); // the type (conditonal, implicit, etc) of binding we'll return
                resolveBindings =
                    resolveBindings.TakeWhile(binding => bindingPrecedenceComparer.Compare(binding, model) == 0);

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
        public IComponentContainer Components { get; private set; }

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
                if (this.Components != null)
                {
                    // Deactivate all cached instances before shutting down the kernel.
                    var cache = this.Components.Get<ICache>();
                    cache.Clear();

                    this.Components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        /// <inheritdoc />
        public IEnumerable<IBinding> GetBindings(Type service)
        {
            Ensure.ArgumentNotNull(service, "service");

            lock (this.bindingCache)
            {
                if (!this.bindingCache.ContainsKey(service))
                {
                    this.bindingResolvers
                        .SelectMany(resolver => resolver.Resolve(this.bindings, service))
                        .Map(binding => this.bindingCache.Add(service, binding));
                }

                return this.bindingCache[service];
            }
        }

        // Todo: Remove
        public IPlanner Planner { get; private set; }

        // Todo: Remove
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
        /// Returns an IComparer that is used to determine resolution precedence.
        /// </summary>
        /// <returns>An IComparer that is used to determine resolution precedence.</returns>
        protected virtual IComparer<IBinding> GetBindingPrecedenceComparer()
        {
            return new BindingPrecedenceComparer();
        }

        /// <summary>
        /// Attempts to handle a missing binding for a request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns><c>True</c> if the missing binding can be handled; otherwise <c>false</c>.</returns>
        protected virtual bool HandleMissingBinding(IRequest request)
        {
            return false;

            // Todo: Add support for missing bindings
            //Ensure.ArgumentNotNull(request, "request");

            //var components = this.Components.GetAll<IMissingBindingResolver>();

            //// Take the first set of bindings that resolve.
            //var bindings = components
            //    .Select(c => c.Resolve(this.bindings, request).ToList())
            //    .FirstOrDefault(b => b.Any());

            //if (bindings == null)
            //{
            //    return false;
            //}

            //lock (this.HandleMissingBindingLockObject)
            //{
            //    if (!this.CanResolve(request))
            //    {
            //        bindings.Map(binding => binding.IsImplicit = true);
            //        this.AddBindings(bindings);
            //    }
            //}

            //return true;
        }

        //private void AddBindings(IEnumerable<IBinding> bindings)
        //{
        //    bindings.Map(binding => this.bindings.Add(binding.Service, binding));

        //    lock (this.bindingCache)
        //        this.bindingCache.Clear();
        //}

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

        // ToDo: Configure in ComponentContainer
        private class BindingPrecedenceComparer : IComparer<IBinding>
        {
            public int Compare(IBinding x, IBinding y)
            {
                if (x == y)
                {
                    return 0;
                }

                // Each function represents a level of precedence.
                var funcs = new List<Func<IBinding, bool>>
                            {
                                b => b != null,       // null bindings should never happen, but just in case
                                b => b.IsConditional, // conditional bindings > unconditional
                                b => !b.Service.ContainsGenericParameters, // closed generics > open generics
                                b => !b.IsImplicit,   // explicit bindings > implicit
                            };

                var q = from func in funcs
                        let xVal = func(x)
                        where xVal != func(y)
                        select xVal ? 1 : -1;

                // returns the value of the first function that represents a difference
                // between the bindings, or else returns 0 (equal)
                return q.FirstOrDefault();
            }
        }
    }
}