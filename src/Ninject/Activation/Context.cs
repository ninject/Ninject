// -------------------------------------------------------------------------------------------------
// <copyright file="Context.cs" company="Ninject Project Contributors">
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

namespace Ninject.Activation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ninject.Activation.Caching;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Parameters;
    using Ninject.Planning;
    using Ninject.Planning.Bindings;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Contains information about the activation of a single instance.
    /// </summary>
    public class Context : IContext
    {
        /// <summary>
        /// The ninject settings.
        /// </summary>
        private readonly INinjectSettings settings;

        /// <summary>
        /// The <see cref="IExceptionFormatter"/> component.
        /// </summary>
        private readonly IExceptionFormatter exceptionFormatter;

        /// <summary>
        /// The cached scope object.
        /// </summary>
        private object cachedScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="kernel">The kernel managing the resolution.</param>
        /// <param name="settings">The ninject settings.</param>
        /// <param name="request">The context's request.</param>
        /// <param name="binding">The context's binding.</param>
        /// <param name="cache">The cache component.</param>
        /// <param name="planner">The planner component.</param>
        /// <param name="pipeline">The pipeline component.</param>
        /// <param name="exceptionFormatter">The <see cref="IExceptionFormatter"/> component.</param>
        /// <exception cref="ArgumentNullException"><paramref name="kernel"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="settings"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="binding"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="cache"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="planner"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="exceptionFormatter"/> is <see langword="null"/>.</exception>
        public Context(IReadOnlyKernel kernel, INinjectSettings settings, IRequest request, IBinding binding, ICache cache, IPlanner planner, IPipeline pipeline, IExceptionFormatter exceptionFormatter)
        {
            Ensure.ArgumentNotNull(kernel, nameof(kernel));
            Ensure.ArgumentNotNull(settings, nameof(settings));
            Ensure.ArgumentNotNull(request, nameof(request));
            Ensure.ArgumentNotNull(binding, nameof(binding));
            Ensure.ArgumentNotNull(cache, nameof(cache));
            Ensure.ArgumentNotNull(planner, nameof(planner));
            Ensure.ArgumentNotNull(pipeline, nameof(pipeline));
            Ensure.ArgumentNotNull(exceptionFormatter, nameof(exceptionFormatter));

            this.settings = settings;
            this.Kernel = kernel;
            this.Request = request;
            this.Binding = binding;
            this.Parameters = request.Parameters.Union(binding.Parameters);
            this.Cache = cache;
            this.Planner = planner;
            this.Pipeline = pipeline;
            this.exceptionFormatter = exceptionFormatter;

            if (binding.Service.IsGenericTypeDefinition)
            {
                this.HasInferredGenericArguments = true;
                this.GenericArguments = request.Service.GenericTypeArguments;
            }
        }

        /// <summary>
        /// Gets or sets the kernel that is driving the activation.
        /// </summary>
        public IReadOnlyKernel Kernel { get; set; }

        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        public IRequest Request { get; set; }

        /// <summary>
        /// Gets or sets the binding.
        /// </summary>
        public IBinding Binding { get; set; }

        /// <summary>
        /// Gets or sets the activation plan.
        /// </summary>
        public IPlan Plan { get; set; }

        /// <summary>
        /// Gets or sets the parameters that were passed to manipulate the activation process.
        /// </summary>
        public IEnumerable<IParameter> Parameters { get; set; }

        /// <summary>
        /// Gets the generic arguments for the request, if any.
        /// </summary>
        public Type[] GenericArguments { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the request involves inferred generic arguments.
        /// </summary>
        public bool HasInferredGenericArguments { get; private set; }

        /// <summary>
        /// Gets the cache component.
        /// </summary>
        public ICache Cache { get; private set; }

        /// <summary>
        /// Gets the planner component.
        /// </summary>
        public IPlanner Planner { get; private set; }

        /// <summary>
        /// Gets the pipeline component.
        /// </summary>
        public IPipeline Pipeline { get; private set; }

        /// <summary>
        /// Gets the scope for the context that "owns" the instance activated therein.
        /// </summary>
        /// <returns>
        /// The object that acts as the scope.
        /// </returns>
        public object GetScope()
        {
            return this.cachedScope ?? this.Request.GetScope() ?? this.Binding.GetScope(this);
        }

        /// <summary>
        /// Gets the provider that should be used to create the instance for this context.
        /// </summary>
        /// <returns>
        /// The provider that should be used.
        /// </returns>
        public IProvider GetProvider()
        {
            return this.Binding.GetProvider(this);
        }

        /// <summary>
        /// Resolves the instance associated with this hook.
        /// </summary>
        /// <returns>
        /// The resolved instance.
        /// </returns>
        public object Resolve()
        {
            if (this.Request.ActiveBindings.Contains(this.Binding) &&
                IsCyclical(this.Request.ParentRequest, this.Request.Target))
            {
                throw new ActivationException(this.exceptionFormatter.CyclicalDependenciesDetected(this));
            }

            try
            {
                this.cachedScope = this.Request.GetScope() ?? this.Binding.GetScope(this);

                if (this.cachedScope != null)
                {
                    lock (this.cachedScope)
                    {
                        return this.ResolveInternal(this.cachedScope);
                    }
                }
                else
                {
                    return this.ResolveInternal(null);
                }
            }
            finally
            {
                this.cachedScope = null;
            }
        }

        private static bool IsCyclical(IRequest request, ITarget target)
        {
            if (request == null)
            {
                return false;
            }

            if (request.Target == target)
            {
                return true;
            }

            return IsCyclical(request.ParentRequest, target);
        }

        private object ResolveInternal(object scope)
        {
            if (scope != null)
            {
                var cachedInstance = this.Cache.TryGet(this, scope);
                if (cachedInstance != null)
                {
                    return cachedInstance;
                }
            }

            this.Request.ActiveBindings.Push(this.Binding);

            var reference = new InstanceReference { Instance = this.GetProvider().Create(this) };

            this.Request.ActiveBindings.Pop();

            if (reference.Instance == null)
            {
                if (!this.settings.AllowNullInjection)
                {
                    throw new ActivationException(this.exceptionFormatter.ProviderReturnedNull(this));
                }

                if (this.Plan == null)
                {
                    this.Plan = this.Planner.GetPlan(this.Request.Service);
                }

                return null;
            }

            if (scope != null)
            {
                this.Cache.Remember(this, scope, reference);
            }

            if (this.Plan == null)
            {
                this.Plan = this.Planner.GetPlan(reference.Instance.GetType());
            }

            try
            {
                this.Pipeline.Activate(this, reference);
            }
            catch (ActivationException)
            {
                if (scope != null)
                {
                    this.Cache.Release(reference.Instance);
                }

                throw;
            }

            return reference.Instance;
        }
    }
}