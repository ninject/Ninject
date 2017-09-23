// -------------------------------------------------------------------------------------------------
// <copyright file="IActivationBlock.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Activation.Blocks
{
    using Ninject.Infrastructure.Disposal;
    using Ninject.Syntax;

    /// <summary>
    /// A block used for deterministic disposal of activated instances. When the block is
    /// disposed, all instances activated via it will be deactivated.
    /// </summary>
    public interface IActivationBlock : IResolutionRoot, INotifyWhenDisposed
    {
    }
}