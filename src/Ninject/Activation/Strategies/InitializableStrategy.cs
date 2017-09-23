// -------------------------------------------------------------------------------------------------
// <copyright file="InitializableStrategy.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Activation.Strategies
{
    /// <summary>
    /// During activation, initializes instances that implement <see cref="IInitializable"/>.
    /// </summary>
    public class InitializableStrategy : ActivationStrategy
    {
        /// <summary>
        /// Initializes the specified instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being activated.</param>
        public override void Activate(IContext context, InstanceReference reference)
        {
            reference.IfInstanceIs<IInitializable>(x => x.Initialize());
        }
    }
}