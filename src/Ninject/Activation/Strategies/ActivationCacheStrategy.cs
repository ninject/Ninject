// -------------------------------------------------------------------------------------------------
// <copyright file="ActivationCacheStrategy.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Activation.Strategies
{
    using Ninject.Activation.Caching;
    using Ninject.Components;
    using Ninject.Infrastructure;

    /// <summary>
    /// Adds all activated instances to the activation cache.
    /// </summary>
    public class ActivationCacheStrategy : NinjectComponent, IActivationStrategy
    {
        /// <summary>
        /// The activation cache.
        /// </summary>
        private readonly IActivationCache activationCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationCacheStrategy"/> class.
        /// </summary>
        /// <param name="activationCache">The activation cache.</param>
        public ActivationCacheStrategy(IActivationCache activationCache)
        {
            Ensure.ArgumentNotNull(activationCache, "activationCache");

            this.activationCache = activationCache;
        }

        /// <summary>
        /// Contributes to the activation of the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being activated.</param>
        public void Activate(IContext context, InstanceReference reference)
        {
            this.activationCache.AddActivatedInstance(reference.Instance);
        }

        /// <summary>
        /// Contributes to the deactivation of the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being deactivated.</param>
        public void Deactivate(IContext context, InstanceReference reference)
        {
            this.activationCache.AddDeactivatedInstance(reference.Instance);
        }
    }
}