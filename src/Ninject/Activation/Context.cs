#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Activation.Caching;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Introspection;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;
using Ninject.Syntax;

#endregion

namespace Ninject.Activation
{
    /// <summary>
    /// Contains information about the activation of a single instance.
    /// </summary>
    public class Context : IContext
    {
        private WeakReference cachedScope;

        /// <summary>
        /// Gets the kernel that is driving the activation.
        /// </summary>
        public IReadonlyKernel Kernel { get; set; }

        /// <summary>
        /// Gets the request.
        /// </summary>
        public IRequest Request { get; set; }

        /// <summary>
        /// Gets the binding.
        /// </summary>
        public IBinding Binding { get; set; }

        /// <summary>
        /// Gets or sets the activation plan.
        /// </summary>
        public IPlan Plan { get; set; }

        /// <summary>
        /// Gets the parameters that were passed to manipulate the activation process.
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
        /// Gets or sets the cache component.
        /// </summary>
        public ICache Cache { get; private set; }

        /// <summary>
        /// Gets or sets the planner component.
        /// </summary>
        public IPlanner Planner { get; private set; }

        /// <summary>
        /// Gets or sets the pipeline component.
        /// </summary>
        public IPipeline Pipeline { get; private set; }

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
            Kernel = readonlyKernel;
            Request = request;
            Binding = binding;
            Parameters = request.Parameters.Union(binding.Parameters).ToList();

            Cache = cache;
            Planner = planner;
            Pipeline = pipeline;

            if (binding.Service.IsGenericTypeDefinition)
            {
                HasInferredGenericArguments = true;
                GenericArguments = request.Service.GetGenericArguments();
            }
        }

        /// <summary>
        /// Gets the scope for the context that "owns" the instance activated therein.
        /// </summary>
        /// <returns>The object that acts as the scope.</returns>
        public object GetScope()
        {
            if (this.cachedScope == null)
            {
                var scope = this.Request.GetScope() ?? this.Binding.GetScope(this);
                this.cachedScope = new WeakReference(scope);
            }
            
            return this.cachedScope.Target;
        }

        /// <summary>
        /// Gets the provider that should be used to create the instance for this context.
        /// </summary>
        /// <returns>The provider that should be used.</returns>
        public IProvider GetProvider()
        {
            return Binding.GetProvider(this);
        }

        /// <summary>
        /// Resolves the instance associated with this hook.
        /// </summary>
        /// <returns>The resolved instance.</returns>
        public object Resolve()
        {
            if (Request.ActiveBindings.Contains(Binding))
                throw new ActivationException(ExceptionFormatter.CyclicalDependenciesDetected(this));

            var scope = this.GetScope();

            if (scope != null)
            {
                lock (scope)
                {
                    return this.ResolveInternal(scope);
                }
            }
            else
            {
                return this.ResolveInternal(null);
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