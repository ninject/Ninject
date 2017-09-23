// -------------------------------------------------------------------------------------------------
// <copyright file="StartableStrategy.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Activation.Strategies
{
    /// <summary>
    /// Starts instances that implement <see cref="IStartable"/> during activation,
    /// and stops them during deactivation.
    /// </summary>
    public class StartableStrategy : ActivationStrategy
    {
        /// <summary>
        /// Starts the specified instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being activated.</param>
        public override void Activate(IContext context, InstanceReference reference)
        {
            reference.IfInstanceIs<IStartable>(x => x.Start());
        }

        /// <summary>
        /// Stops the specified instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being deactivated.</param>
        public override void Deactivate(IContext context, InstanceReference reference)
        {
            reference.IfInstanceIs<IStartable>(x => x.Stop());
        }
    }
}