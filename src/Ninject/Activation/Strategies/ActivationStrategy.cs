// -------------------------------------------------------------------------------------------------
// <copyright file="ActivationStrategy.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Activation.Strategies
{
    using Ninject.Components;

    /// <summary>
    /// Contributes to a <see cref="IPipeline"/>, and is called during the activation
    /// and deactivation of an instance.
    /// </summary>
    public abstract class ActivationStrategy : NinjectComponent, IActivationStrategy
    {
        /// <summary>
        /// Contributes to the activation of the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being activated.</param>
        public virtual void Activate(IContext context, InstanceReference reference)
        {
        }

        /// <summary>
        /// Contributes to the deactivation of the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being deactivated.</param>
        public virtual void Deactivate(IContext context, InstanceReference reference)
        {
        }
    }
}