// -------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyKernel.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2019 Ninject Project Contributors. All rights reserved.
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
    using Ninject.Components;
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
        private readonly INinjectSettings settings;
        private readonly ICache cache;
        private readonly IPlanner planner;
        private readonly IConstructorScorer constructorScorer;
        private readonly IPipeline pipeline;
        private readonly IBindingPrecedenceComparer bindingPrecedenceComparer;
        private readonly IExceptionFormatter exceptionFormatter;
        private readonly IEnumerable<IBindingResolver> bindingResolvers;
        private readonly IEnumerable<IMissingBindingResolver> missingBindingResolvers;
        private readonly object missingBindingCacheLock = new object();

        private Dictionary<Type, IBinding[]> bindingCache = new Dictionary<Type, IBinding[]>(new ReferenceEqualityTypeComparer());
        private Dictionary<Type, ICollection<IBinding>> bindings = new Dictionary<Type, ICollection<IBinding>>(new ReferenceEqualityTypeComparer());

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyKernel"/> class.
        /// </summary>
        /// <param name="settings">The <see cref="INinjectSettings"/>.</param>
        /// <param name="bindings">The preconfigured bindings.</param>
        /// <param name="cache">The <see cref="ICache"/> component.</param>
        /// <param name="planner">The <see cref="IPlanner"/> component.</param>
        /// <param name="constructorScorer">The <see cref="IConstructorScorer"/> component.</param>
        /// <param name="pipeline">The <see cref="IPipeline"/> component.</param>
        /// <param name="exceptionFormatter">The <see cref="IExceptionFormatter"/> component.</param>
        /// <param name="bindingPrecedenceComparer">The <see cref="IBindingPrecedenceComparer"/> component.</param>
        /// <param name="bindingResolvers">The binding resolvers.</param>
        /// <param name="missingBindingResolvers">The missing binding resolvers.</param>
        internal ReadOnlyKernel(
            INinjectSettings settings,
            Dictionary<Type, ICollection<IBinding>> bindings,
            ICache cache,
            IPlanner planner,
            IConstructorScorer constructorScorer,
            IPipeline pipeline,
            IExceptionFormatter exceptionFormatter,
            IBindingPrecedenceComparer bindingPrecedenceComparer,
            IEnumerable<IBindingResolver> bindingResolvers,
            IEnumerable<IMissingBindingResolver> missingBindingResolvers)
        {
            this.settings = settings;
            this.bindings = bindings;
            this.bindingResolvers = bindingResolvers;
            this.missingBindingResolvers = missingBindingResolvers;
            this.cache = cache;
            this.planner = planner;
            this.constructorScorer = constructorScorer;
            this.pipeline = pipeline;
            this.exceptionFormatter = exceptionFormatter;
            this.bindingPrecedenceComparer = bindingPrecedenceComparer;

            this.AddReadOnlyKernelBinding<IReadOnlyKernel>(this, bindings);
            this.AddReadOnlyKernelBinding<IResolutionRoot>(this, bindings);
        }

        /// <summary>
        /// Injects the specified existing instance, without managing its lifecycle.
        /// </summary>
        /// <param name="instance">The instance to inject.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
        public void Inject(object instance, params IParameter[] parameters)
        {
            Ensure.ArgumentNotNull(instance, nameof(instance));

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
        /// <returns>
        /// <see langword="true"/> if the request can be resolved; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        public bool CanResolve(IRequest request)
        {
            Ensure.ArgumentNotNull(request, nameof(request));

            return this.GetBindings(request.Service).Any(this.SatifiesRequest(request));
        }

        /// <summary>
        /// Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="ignoreImplicitBindings">if set to <see langword="true"/> implicit bindings are ignored.</param>
        /// <returns>
        /// <see langword="true"/> if the request can be resolved; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        public bool CanResolve(IRequest request, bool ignoreImplicitBindings)
        {
            Ensure.ArgumentNotNull(request, nameof(request));

            return this.GetBindings(request.Service)
                .Any(binding => (!ignoreImplicitBindings || !binding.IsImplicit) && this.SatifiesRequest(request)(binding));
        }

        /// <summary>
        /// Resolves instances for the specified request. The instances are not actually resolved
        /// until a consumer iterates over the enumerator.
        /// </summary>
        /// <param name="request">The request to resolve.</param>
        /// <returns>
        /// An enumerator of instances that match the request.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        public IEnumerable<object> Resolve(IRequest request)
        {
            Ensure.ArgumentNotNull(request, nameof(request));

            return this.Resolve(request, true);
        }

        /// <summary>
        /// Creates a request for the specified service.
        /// </summary>
        /// <param name="service">The service that is being requested.</param>
        /// <param name="constraint">The constraint to apply to the bindings to determine if they match the request.</param>
        /// <param name="parameters">The parameters to pass to the resolution.</param>
        /// <param name="isOptional"><see langword="true"/> if the request is optional; otherwise, <see langword="false"/>.</param>
        /// <param name="isUnique"><see langword="true"/> if the request should return a unique result; otherwise, <see langword="false"/>.</param>
        /// <returns>
        /// The request for the specified service.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="service"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
        public IRequest CreateRequest(Type service, Func<IBindingMetadata, bool> constraint, IReadOnlyList<IParameter> parameters, bool isOptional, bool isUnique)
        {
            return new Request(service, constraint, parameters, null, isOptional, isUnique);
        }

        /// <summary>
        /// Deactivates and releases the specified instance if it is currently managed by Ninject.
        /// </summary>
        /// <param name="instance">The instance to release.</param>
        /// <returns>
        /// <see langword="true"/> if the instance was found and released; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/>.</exception>
        public bool Release(object instance)
        {
            Ensure.ArgumentNotNull(instance, nameof(instance));

            return this.cache.Release(instance);
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>
        /// The service object.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> is <see langword="null"/>.</exception>
        public object GetService(Type serviceType)
        {
            Ensure.ArgumentNotNull(serviceType, nameof(serviceType));

            return this.Get(serviceType);
        }

        /// <summary>
        /// Gets the bindings registered for the specified service.
        /// </summary>
        /// <param name="service">The service in question.</param>
        /// <returns>
        /// A series of bindings that are registered for the service.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="service"/> is <see langword="null"/>.</exception>
        public IBinding[] GetBindings(Type service)
        {
            Ensure.ArgumentNotNull(service, nameof(service));

            return this.GetBindingsCore(service);
        }

        /// <summary>
        /// Returns a predicate that can determine if a given <see cref="IBinding"/> matches the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// A predicate that can determine if a given <see cref="IBinding"/> matches the request.
        /// </returns>
        protected virtual Func<IBinding, bool> SatifiesRequest(IRequest request)
        {
            return binding => binding.Matches(request) && request.Matches(binding);
        }

        /// <summary>
        /// Attempts to handle a missing binding for a request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// <see langword="true"/> if the missing binding can be handled; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        protected virtual bool HandleMissingBinding(IRequest request)
        {
            Ensure.ArgumentNotNull(request, nameof(request));

            var bindings = this.GetBindingsFromFirstResolverThatReturnsAtLeastOneBinding(request);
            if (bindings == null)
            {
                return false;
            }

            bindings.ForEach(binding => binding.IsImplicit = true);

            lock (this.missingBindingCacheLock)
            {
                if (this.CanResolve(request))
                {
                    return true;
                }

                var newBindings = new Dictionary<Type, ICollection<IBinding>>(this.bindings, new ReferenceEqualityTypeComparer());
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
            return new Context(this, this.settings, request, binding, this.cache, this.planner, this.pipeline, this.exceptionFormatter);
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

            throw new ActivationException(this.exceptionFormatter.CouldNotResolveBinding(request));
        }

        private IEnumerable<object> Resolve(IRequest request, bool handleMissingBindings)
        {
            void UpdateRequest(Type service)
            {
                if (request.ParentRequest == null)
                {
                    request = this.CreateRequest(service, null, request.Parameters.GetShouldInheritParameters(), true, false);
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

        private void AddReadOnlyKernelBinding<T>(T readonlyKernel, Dictionary<Type, ICollection<IBinding>> bindings)
        {
            var binding = new Binding(typeof(T));
            new BindingBuilder<T>(binding, this.planner, this.constructorScorer, typeof(T).Format()).ToConstant(readonlyKernel);
            bindings[typeof(T)] = new[] { binding };
        }

        private IBinding[] GetBindingsCore(Type service)
        {
            if (this.bindingCache.TryGetValue(service, out IBinding[] result))
            {
                return result;
            }

            result = this.CreateBindings(service);
            if (result.Length > 0)
            {
                var newBindingCache = new Dictionary<Type, IBinding[]>(this.bindingCache, new ReferenceEqualityTypeComparer());
                newBindingCache[service] = result;
                this.bindingCache = newBindingCache;
            }

            return result;
        }

        private IBinding[] CreateBindings(Type service)
        {
            var bindingList = new List<IBinding>();
            foreach (var bindingResolver in this.bindingResolvers)
            {
                var bindings = bindingResolver.Resolve(this.bindings, service);
                if (bindings.Count > 0)
                {
                    bindingList.AddRange(bindings);
                }
            }

            var bindingCount = bindingList.Count;
            if (bindingCount == 0)
            {
                return Array.Empty<IBinding>();
            }
            else if (bindingCount == 1)
            {
                return new[] { bindingList[0] };
            }
            else
            {
                var bindingArray = bindingList.ToArray();
                Array.Sort(bindingArray, new ReverseComparer<IBinding>(this.bindingPrecedenceComparer));
                return bindingArray;
            }
        }
    }
}