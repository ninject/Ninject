// -------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyKernel.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2017 Ninject Project Contributors. All rights reserved.
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   You may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ninject.Activation;
    using Ninject.Activation.Caching;
    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Disposal;
    using Ninject.Infrastructure.Introspection;
    using Ninject.Infrastructure.Language;
    using Ninject.Parameters;
    using Ninject.Planning;
    using Ninject.Planning.Bindings;
    using Ninject.Planning.Bindings.Resolvers;
    using Ninject.Selection.Heuristics;
    using Ninject.Syntax;

    /// <summary>
    /// The readonly kernel.
    /// </summary>
    public class ReadOnlyKernel : DisposableObject, IReadOnlyKernel
    {
        private readonly ICache cache;
        private readonly IPlanner planner;
        private readonly IConstructorScorer scorer;
        private readonly IPipeline pipeline;
        private readonly IBindingPrecedenceComparer bindingPrecedenceComparer;
        private readonly IEnumerable<IBindingResolver> bindingResolvers;
        private readonly IEnumerable<IMissingBindingResolver> missingBindingResolvers;
        private readonly object missingBindingCacheLock = new object();

        private Dictionary<Type, ICollection<IBinding>> bindingCache = new Dictionary<Type, ICollection<IBinding>>();
        private Dictionary<Type, ICollection<IBinding>> bindings = new Dictionary<Type, ICollection<IBinding>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyKernel"/> class.
        /// </summary>
        /// <param name="bindings">The preconfigured bindings.</param>
        /// <param name="cache">The <see cref="ICache"/> component.</param>
        /// <param name="planner">The <see cref="IPlanner"/> component.</param>
        /// <param name="scorer">The <see cref="IConstructorScorer"/> component.</param>
        /// <param name="pipeline">The <see cref="IPipeline"/> component.</param>
        /// <param name="bindingPrecedenceComparer">The <see cref="IBindingPrecedenceComparer"/> component.</param>
        /// <param name="bindingResolvers">The binding resolvers.</param>
        /// <param name="missingBindingResolvers">The missng binding resolvers.</param>
        /// <param name="settings">The <see cref="INinjectSettings"/>.</param>
        internal ReadOnlyKernel(
            INinjectSettings settings,
            IDictionary<Type, ICollection<IBinding>> bindings,
            ICache cache,
            IPlanner planner,
            IConstructorScorer scorer,
            IPipeline pipeline,
            IBindingPrecedenceComparer bindingPrecedenceComparer,
            IEnumerable<IBindingResolver> bindingResolvers,
            IEnumerable<IMissingBindingResolver> missingBindingResolvers)
        {
            this.Settings = settings;

            this.bindingResolvers = bindingResolvers;
            this.missingBindingResolvers = missingBindingResolvers;
            this.cache = cache;
            this.planner = planner;
            this.scorer = scorer;
            this.pipeline = pipeline;
            this.bindingPrecedenceComparer = bindingPrecedenceComparer;

            this.AddReadOnlyKernelBinding<IReadOnlyKernel>(this, bindings);
            this.AddReadOnlyKernelBinding<IResolutionRoot>(this, bindings);

            this.bindings = bindings.Keys.ToDictionary(type => type, type => bindings[type]);
            this.InitializeBindings();
        }

        /// <summary>
        /// Gets the kernel settings.
        /// </summary>
        public INinjectSettings Settings { get; private set; }

        /// <summary>
        /// Injects the specified existing instance, without managing its lifecycle.
        /// </summary>
        /// <param name="instance">The instance to inject.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        public void Inject(object instance, params IParameter[] parameters)
        {
            Ensure.ArgumentNotNull(instance, "instance");
            Ensure.ArgumentNotNull(parameters, "parameters");

            var service = instance.GetType();

            var binding = new Binding(service);
            var request = this.CreateRequest(service, null, parameters, false, false);
            var context = this.CreateContext(request, binding);

            context.Plan = this.planner.GetPlan(service);

            var reference = new InstanceReference { Instance = instance };
            this.pipeline.Activate(context, reference);
        }

        /// <summary>
        /// Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns><c>True</c> if the request can be resolved; otherwise, <c>false</c>.</returns>
        public bool CanResolve(IRequest request)
        {
            Ensure.ArgumentNotNull(request, "request");

            return this.GetBindings(request.Service).Any(this.SatifiesRequest(request));
        }

        /// <summary>
        /// Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="ignoreImplicitBindings">if set to <c>true</c> implicit bindings are ignored.</param>
        /// <returns>
        ///     <c>True</c> if the request can be resolved; otherwise, <c>false</c>.
        /// </returns>
        public bool CanResolve(IRequest request, bool ignoreImplicitBindings)
        {
            Ensure.ArgumentNotNull(request, "request");

            return this.GetBindings(request.Service)
                .Any(binding => (!ignoreImplicitBindings || !binding.IsImplicit) && this.SatifiesRequest(request)(binding));
        }

        /// <summary>
        /// Resolves instances for the specified request. The instances are not actually resolved
        /// until a consumer iterates over the enumerator.
        /// </summary>
        /// <param name="request">The request to resolve.</param>
        /// <returns>An enumerator of instances that match the request.</returns>
        public IEnumerable<object> Resolve(IRequest request)
        {
            Ensure.ArgumentNotNull(request, "request");

            return this.Resolve(request, true);
        }

        /// <summary>
        /// Creates a request for the specified service.
        /// </summary>
        /// <param name="service">The service that is being requested.</param>
        /// <param name="constraint">The constraint to apply to the bindings to determine if they match the request.</param>
        /// <param name="parameters">The parameters to pass to the resolution.</param>
        /// <param name="isOptional"><c>True</c> if the request is optional; otherwise, <c>false</c>.</param>
        /// <param name="isUnique"><c>True</c> if the request should return a unique result; otherwise, <c>false</c>.</param>
        /// <returns>The request for the specified service.</returns>
        public IRequest CreateRequest(Type service, Func<IBindingMetadata, bool> constraint, IEnumerable<IParameter> parameters, bool isOptional, bool isUnique)
        {
            Ensure.ArgumentNotNull(service, "service");
            Ensure.ArgumentNotNull(parameters, "parameters");

            return new Request(service, constraint, parameters, null, isOptional, isUnique);
        }

        /// <summary>
        /// Deactivates and releases the specified instance if it is currently managed by Ninject.
        /// </summary>
        /// <param name="instance">The instance to release.</param>
        /// <returns><see langword="True"/> if the instance was found and released; otherwise <see langword="false"/>.</returns>
        public bool Release(object instance)
        {
            Ensure.ArgumentNotNull(instance, "instance");

            return this.cache.Release(instance);
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The service object.</returns>
        public object GetService(Type serviceType)
        {
            Ensure.ArgumentNotNull(serviceType, "serviceType");

            return this.Get(serviceType);
        }

        /// <summary>
        /// Gets the bindings registered for the specified service.
        /// </summary>
        /// <param name="service">The service in question.</param>
        /// <returns>A series of bindings that are registered for the service.</returns>
        public IEnumerable<IBinding> GetBindings(Type service)
        {
            Ensure.ArgumentNotNull(service, "service");

            if (this.bindingCache.TryGetValue(service, out ICollection<IBinding> result))
            {
                return result;
            }

            var newBindingCache = new Dictionary<Type, ICollection<IBinding>>(this.bindingCache);
            if (newBindingCache.TryGetValue(service, out result))
            {
                return result;
            }

            result = this.bindingResolvers.SelectMany(resolver => resolver.Resolve(this.bindings, service))
                .OrderByDescending(b => b, this.bindingPrecedenceComparer)
                .ToList();
            if (result.Count > 0)
            {
                newBindingCache[service] = result;

                // But this is no problem in this case they will be added later again.
                this.bindingCache = newBindingCache;
            }

            return result;
        }

        /// <summary>
        /// Returns a predicate that can determine if a given IBinding matches the request.
        /// </summary>
        /// <param name="request">The request/.</param>
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
                if (this.CanResolve(request))
                {
                    return true;
                }

                var newBindings = new Dictionary<Type, ICollection<IBinding>>(this.bindings);
                bindings.GroupBy(b => b.Service, b => b, (service, b) => new { service, bindings = b })
                                    .Map(
                serviceGroup =>
                            {
                                if (newBindings.TryGetValue(serviceGroup.service, out ICollection<IBinding> existingBindings))
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

        private IEnumerable<object> ResolveWithMissingBindings(IRequest request, bool handleMissingBindings)
        {
            if (handleMissingBindings && this.HandleMissingBinding(request))
            {
                return this.Resolve(request);
            }

            if (request.IsOptional)
            {
                return Enumerable.Empty<object>();
            }

            throw new ActivationException(ExceptionFormatter.CouldNotResolveBinding(request));
        }

        private IEnumerable<object> Resolve(IRequest request, bool handleMissingBindings)
        {
            void UpdateRequest(Type service)
            {
                if (request.ParentRequest == null)
                {
                    request = this.CreateRequest(service, null, request.Parameters.Where(p => p.ShouldInherit), true, false);
                }
                else
                {
                    request = request.ParentRequest.CreateChild(service, request.ParentContext, request.Target);
                    request.IsOptional = true;
                }
            }

            if (request.Service.IsArray)
            {
                var service = request.Service.GetElementType();

                UpdateRequest(service);

                return new[] { this.Resolve(request, false).CastSlow(service).ToArraySlow(service) };
            }

            if (request.Service.IsGenericType)
            {
                var gtd = request.Service.GetGenericTypeDefinition();

                if (gtd == typeof(List<>) || gtd == typeof(IList<>) || gtd == typeof(ICollection<>))
                {
                    var service = request.Service.GenericTypeArguments[0];

                    UpdateRequest(service);

                    return new[] { this.Resolve(request, false).CastSlow(service).ToListSlow(service) };
                }

                if (gtd == typeof(IEnumerable<>))
                {
                    var service = request.Service.GenericTypeArguments[0];

                    UpdateRequest(service);

                    return new[] { this.Resolve(request, false).CastSlow(service) };
                }
            }

            var satisfiedBindings = this.GetBindings(request.Service)
                                        .Where(this.SatifiesRequest(request));
            var satisfiedBindingEnumerator = satisfiedBindings.GetEnumerator();

            if (!satisfiedBindingEnumerator.MoveNext())
            {
                return this.ResolveWithMissingBindings(request, handleMissingBindings);
            }

            if (request.IsUnique)
            {
                var selectedBinding = satisfiedBindingEnumerator.Current;

                if (satisfiedBindingEnumerator.MoveNext() &&
                    this.bindingPrecedenceComparer.Compare(selectedBinding, satisfiedBindingEnumerator.Current) == 0)
                {
                    if (request.IsOptional && !request.ForceUnique)
                    {
                        return Enumerable.Empty<object>();
                    }

                    var formattedBindings =
                        from binding in satisfiedBindings
                        let context = this.CreateContext(request, binding)
                        select binding.Format(context);

                    throw new ActivationException(ExceptionFormatter.CouldNotUniquelyResolveBinding(
                        request,
                        formattedBindings.ToArray()));
                }

                return new[] { this.CreateContext(request, selectedBinding).Resolve() };
            }
            else
            {
                if (satisfiedBindings.Any(binding => !binding.IsImplicit) || !handleMissingBindings)
                {
                    satisfiedBindings = satisfiedBindings.Where(binding => !binding.IsImplicit);
                }

                return satisfiedBindings
                    .Select(binding => this.CreateContext(request, binding).Resolve());
            }
        }

        private List<IBinding> GetBindingsFromFirstResolverThatReturnsAtLeastOneBinding(IRequest request)
        {
            return this.missingBindingResolvers
                .Select(c => c.Resolve(this.bindings, request).ToList())
                .FirstOrDefault(b => b.Any());
        }

        private void AddReadOnlyKernelBinding<T>(T readonlyKernel, IDictionary<Type, ICollection<IBinding>> bindings)
        {
            var binding = new Binding(typeof(T));
            new BindingBuilder<T>(binding, this.Settings, typeof(T).Format()).ToConstant(readonlyKernel);
            bindings[typeof(T)] = new[] { binding };
        }

        private void InitializeBindings()
        {
            foreach (var binding in this.bindings.Values.SelectMany(b => b))
            {
                binding.InitializeProviderCallback?.Invoke(this.planner, this.scorer);
                this.GetBindings(binding.Service);
            }
        }
    }
}