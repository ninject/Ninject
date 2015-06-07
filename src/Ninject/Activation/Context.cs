//-------------------------------------------------------------------------------
// <copyright file="Context.cs" company="Ninject Project Contributors">
//   Copyright (c) 2009-2014 Ninject Project Contributors
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
//-------------------------------------------------------------------------------

namespace Ninject.Activation
{
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Ninject.Activation.Caching;
using Ninject.Infrastructure.Introspection;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;

    /// <summary>
    /// Contains information about the activation of a single instance.
    /// </summary>
    public class Context : IContext
    {
        private object cachedScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="readonlyKernel">The kernel managing the resolution.</param>
        /// <param name="request">The context's request.</param>
        /// <param name="binding">The context's binding.</param>
        /// <param name="cache">The cache component.</param>
        /// <param name="planner">The planner component.</param>
        /// <param name="pipeline">The pipeline component.</param>
        public Context(IReadonlyKernel readonlyKernel, IRequest request, IBinding binding, ICache cache, IPlanner planner, IPipeline pipeline)
        {
            this.Kernel = readonlyKernel;
            this.Request = request;
            this.Binding = binding;
            this.Parameters = request.Parameters.Union(binding.Parameters).ToList();
            this.Cache = cache;
            this.Planner = planner;
            this.Pipeline = pipeline;

            if (binding.Service.GetTypeInfo().IsGenericTypeDefinition)
            {
                HasInferredGenericArguments = true;
                GenericArguments = request.Service.GetTypeInfo().GenericTypeArguments;
            }
        }

        /// <inheritdoc />
        public IReadonlyKernel Kernel { get; set; }

        /// <inheritdoc />
        public IRequest Request { get; set; }

        /// <inheritdoc />
        public IBinding Binding { get; set; }

        /// <inheritdoc />
        public IPlan Plan { get; set; }

        /// <inheritdoc />
        public ICollection<IParameter> Parameters { get; set; }

        /// <inheritdoc />
        public Type[] GenericArguments { get; private set; }

        /// <inheritdoc />
        public bool HasInferredGenericArguments { get; private set; }

        /// <inheritdoc />
        public ICache Cache { get; private set; }

        /// <inheritdoc />
        public IPlanner Planner { get; private set; }

        /// <inheritdoc />
        public IPipeline Pipeline { get; private set; }

        /// <inheritdoc />
        public object GetScope()
        {
            return this.cachedScope ?? this.Request.GetScope() ?? this.Binding.GetScope(this);
        }

        /// <inheritdoc />
        public IProvider GetProvider()
        {
            return this.Binding.GetProvider(this);
        }

        /// <inheritdoc />
        public object Resolve()
        {
            if (this.Request.ActiveBindings.Contains(this.Binding))
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

        /// <inheritdoc />
        public void BuildPlan(Type type)
        {
            if (this.Plan == null)
            {
                this.Plan = this.Planner.GetPlan(type);
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
    }
}