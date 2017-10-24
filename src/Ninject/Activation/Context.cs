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
    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Introspection;
    using Ninject.Parameters;
    using Ninject.Planning;
    using Ninject.Planning.Bindings;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Contains information about the activation of a single instance.
    /// </summary>
    public class Context : IContext
    {
        private object cachedScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="kernel">The kernel managing the resolution.</param>
        /// <param name="request">The context's request.</param>
        /// <param name="binding">The context's binding.</param>
        /// <param name="cache">The cache component.</param>
        /// <param name="planner">The planner component.</param>
        /// <param name="pipeline">The pipeline component.</param>
        public Context(IKernel kernel, IRequest request, IBinding binding, ICache cache, IPlanner planner, IPipeline pipeline)
        {
            Ensure.ArgumentNotNull(kernel, "kernel");
            Ensure.ArgumentNotNull(request, "request");
            Ensure.ArgumentNotNull(binding, "binding");
            Ensure.ArgumentNotNull(cache, "cache");
            Ensure.ArgumentNotNull(planner, "planner");
            Ensure.ArgumentNotNull(pipeline, "pipeline");

            this.Kernel = kernel;
            this.Request = request;
            this.Binding = binding;
            this.Parameters = request.Parameters.Union(binding.Parameters).ToList();

            this.Cache = cache;
            this.Planner = planner;
            this.Pipeline = pipeline;

            if (binding.Service.IsGenericTypeDefinition)
            {
                this.HasInferredGenericArguments = true;
                this.GenericArguments = request.Service.GenericTypeArguments;
            }
        }

        /// <summary>
        /// Gets or sets the kernel that is driving the activation.
        /// </summary>
        public IKernel Kernel { get; set; }

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
        public ICollection<IParameter> Parameters { get; set; }

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
        /// <returns>The object that acts as the scope.</returns>
        public object GetScope()
        {
            return this.cachedScope ?? this.Request.GetScope() ?? this.Binding.GetScope(this);
        }

        /// <summary>
        /// Gets the provider that should be used to create the instance for this context.
        /// </summary>
        /// <returns>The provider that should be used.</returns>
        public IProvider GetProvider()
        {
            return this.Binding.GetProvider(this);
        }

        /// <summary>
        /// Resolves the instance associated with this hook.
        /// </summary>
        /// <returns>The resolved instance.</returns>
        public object Resolve()
        {
            if (this.Request.ActiveBindings.Contains(this.Binding) &&
                this.IsCyclical(this.Request.ParentContext))
            {
                throw new ActivationException(ExceptionFormatter.CyclicalDependenciesDetected(this));
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

        private object ResolveInternal(object scope)
        {
            var cachedInstance = this.Cache.TryGet(this);

            if (cachedInstance != null)
            {
                return cachedInstance;
            }

            this.Request.ActiveBindings.Push(this.Binding);

            var reference = new InstanceReference { Instance = this.GetProvider().Create(this) };

            this.Request.ActiveBindings.Pop();

            if (reference.Instance == null)
            {
                if (!this.Kernel.Settings.AllowNullInjection)
                {
                    throw new ActivationException(ExceptionFormatter.ProviderReturnedNull(this));
                }

                if (this.Plan == null)
                {
                    this.Plan = this.Planner.GetPlan(this.Request.Service);
                }

                return null;
            }

            if (scope != null)
            {
                this.Cache.Remember(this, reference);
            }

            if (this.Plan == null)
            {
                this.Plan = this.Planner.GetPlan(reference.Instance.GetType());
            }

            this.Pipeline.Activate(this, reference);

            return reference.Instance;
        }

        private bool IsCyclical(IContext targetContext)
        {
            if (targetContext == null)
            {
                return false;
            }

            if (targetContext.Request.Service == this.Request.Service)
            {
                if ((this.Request.Target is ParameterTarget && targetContext.Request.Target is ParameterTarget) || targetContext.GetScope() != this.GetScope() || this.GetScope() == null)
                {
                    return true;
                }
            }

            if (this.IsCyclical(targetContext.Request.ParentContext))
            {
                return true;
            }

            return false;
        }
    }
}