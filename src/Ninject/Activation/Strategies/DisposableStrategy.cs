// -------------------------------------------------------------------------------------------------
// <copyright file="DisposableStrategy.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Activation.Strategies
{
    using System;

    /// <summary>
    /// During deactivation, disposes instances that implement <see cref="IDisposable"/>.
    /// </summary>
    public class DisposableStrategy : ActivationStrategy
    {
        /// <summary>
        /// Disposes the specified instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being deactivated.</param>
        public override void Deactivate(IContext context, InstanceReference reference)
        {
            reference.IfInstanceIs<IDisposable>(x => x.Dispose());
        }
    }
}