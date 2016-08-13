//-------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyKernel.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2016, Ninject Project Contributors
//   Authors: Nate Kohari (nate@enkari.com)
//            Remo Gloor (remo.gloor@gmail.com)
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   you may not use this file except in compliance with one of the Licenses.
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
//-------------------------------------------------------------------------------------------------

namespace Ninject
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
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
    using Ninject.Selection;
    using Ninject.Syntax;

    /// <summary>
    /// The readonly kernel
    /// </summary>
    public class ReadOnlyKernel : DisposableObject, IReadOnlyKernel
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
        /// Initializes a new instance of the <see cref="ReadOnlyKernel"/> class.
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
        public ReadOnlyKernel(
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
            this.Settings = settings;

            this.AddReadonlyKernelBinding<IReadOnlyKernel>(this, bindings);
            this.AddReadonlyKernelBinding<IResolutionRoot>(this, bindings);

            this.bindings = bindings.Keys.ToDictionary(type => type, type => bindings[type]);
            this.InitializeBindings(selector);
        }

        /// <inheritdoc />
        public INinjectSettings Settings { get; private set; }

        /// <inheritdoc />
        public void Inject(object instance, params IParameter[] parameters)
        {
            Contract.Requires(instance != null);
            Contract.Requires(parameters != null);

            var service = instance.GetType();

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
            Contract.Requires(request != null);
            return this.GetBindings(request.Service).Any(b => this.SatifiesRequest(request, b));
        }

        /// <inheritdoc />
        public bool CanResolve(IRequest request, bool ignoreImplicitBindings)
        {
            Contract.Requires(request != null);
            return this.GetBindings(request.Service)
                .Any(binding => (!ignoreImplicitBindings || !binding.IsImplicit) && this.SatifiesRequest(request, binding));
        }

        /// <inheritdoc />
        public IEnumerable<object> Resolve(IRequest request)
        {
            Contract.Requires(request != null);

            return this.Resolve(request, true);
        }

        /// <inheritdoc />
        public IRequest CreateRequest(Type service, Func<IBindingMetadata, bool> constraint, IEnumerable<IParameter> parameters, bool isOptional, bool isUnique)
        {
            Contract.Requires(service != null);
            Contract.Requires(parameters != null);

            return new Request(service, constraint, parameters, null, isOptional, isUnique);
        }

        /// <inheritdoc />
        public bool Release(object instance)
        {
            Contract.Requires(instance != null);
            return this.cache.Release(instance);
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            return this.Get(serviceType);
        }

        /// <inheritdoc />
        public override void Dispose(bool disposing)
        {
            if (disposing && !this.IsDisposed)
            {
                this.cache.Clear();
            }

            base.Dispose(disposing);
        }

        /// <inheritdoc />
        public IEnumerable<IBinding> GetBindings(Type service)
        {
            Contract.Requires(service != null);

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

            result = this.bindingResolvers.SelectMany(resolver => resolver.Resolve(this.bindings, service))
                .OrderByDescending(b => b, this.bindingPrecedenceComparer)
                .ToList();
            if (result.Count > 0)
            {
                newBindingCache[service] = result;

                // We might loose other entries in case multiple cache entries are added at the same time
                // But this is no problem in this case they will be added later again.
                this.bindingCache = newBindingCache;
            }

            return result;
        }

        /// <summary>
        /// Returns a predicate that can determine if a given IBinding matches the request.
        /// </summary>
        /// <param name="request">The request/</param>
        /// <param name="binding">The binding</param>
        /// <returns>A predicate that can determine if a given IBinding matches the request.</returns>
        protected virtual bool SatifiesRequest(IRequest request, IBinding binding)
        {
            return binding.Matches(request) && request.Matches(binding);
        }

        /// <summary>
        /// Attempts to handle a missing binding for a request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns><c>True</c> if the missing binding can be handled; otherwise <c>false</c>.</returns>
        protected virtual bool HandleMissingBinding(IRequest request)
        {
            Contract.Requires(request != null);

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

        /// <summary>
        /// Creates a context for the specified request and binding.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="binding">The binding.</param>
        /// <returns>The created context.</returns>
        protected virtual IContext CreateContext(IRequest request, IBinding binding)
        {
            Contract.Requires(request != null);
            Contract.Requires(binding != null);

            return new Context(this, request, binding, this.cache, this.planner, this.pipeline);
        }

        private IEnumerable<object> ResolveWithMissingBindings(IRequest request, bool handleMissingBindings)
        {
            if (handleMissingBindings)
            {
                if (this.HandleMissingBinding(request))
                {
                    return this.Resolve(request, false);
                }
            }

            if (request.IsOptional)
            {
                return Enumerable.Empty<object>();
            }

            throw new ActivationException(ExceptionFormatter.CouldNotResolveBinding(request));
        }

        private IEnumerable<object> ResolveMultiple(IRequest request)
        {
            var selectedBindings = this.GetBindings(request.Service).Where(b => this.SatifiesRequest(request, b));
            var skipImplicit = false;

            foreach (var binding in selectedBindings.TakeWhile(binding => !binding.IsImplicit || !skipImplicit))
            {
                skipImplicit = !binding.IsImplicit;
                yield return this.CreateContext(request, binding).Resolve();
            }
        }

        private IEnumerable<object> Resolve(IRequest request, bool requestMissingBindings)
        {
            var resolveBindings = this.GetBindings(request.Service).Where(b => this.SatifiesRequest(request, b));
            var resolveBindingsIterator = resolveBindings.GetEnumerator();

            if (!resolveBindingsIterator.MoveNext())
            {
                return this.ResolveWithMissingBindings(request, requestMissingBindings);
            }

            if (request.IsUnique)
            {
                var first = resolveBindingsIterator.Current;
                if (resolveBindingsIterator.MoveNext() &&
                    this.bindingPrecedenceComparer.Compare(first, resolveBindingsIterator.Current) == 0)
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

                return new object[] { this.CreateContext(request, first).Resolve() };
            }
            else
            {
                return this.ResolveMultiple(request);
            }
        }

        private List<IBinding> GetBindingsFromFirstResolverThatReturnsAtLeastOneBinding(IRequest request)
        {
            return this.missingBindingResolvers
                .Select(c => c.Resolve(this.bindings, request).ToList())
                .FirstOrDefault(b => b.Any());
        }

        private void AddReadonlyKernelBinding<T>(T readonlyKernel, Multimap<Type, IBinding> bindings)
        {
            var binding = new Binding(typeof(T));
            new BindingBuilder<T>(binding, this.Settings, typeof(T).Format()).ToConstant(readonlyKernel);
            bindings.Add(typeof(T), binding);
        }

        private void InitializeBindings(ISelector selector)
        {
            foreach (var binding in this.bindings.Values.SelectMany(b => b))
            {
                binding.InitializeProviderCallback(selector);
                this.GetBindings(binding.Service);
            }
        }
    }
}
