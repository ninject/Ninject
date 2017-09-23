// -------------------------------------------------------------------------------------------------
// <copyright file="BindingTarget.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Planning.Bindings
{
    /// <summary>
    /// Describes the target of a binding.
    /// </summary>
    public enum BindingTarget
    {
        /// <summary>
        /// Indicates that the binding is from a type to itself.
        /// </summary>
        Self,

        /// <summary>
        /// Indicates that the binding is from one type to another.
        /// </summary>
        Type,

        /// <summary>
        /// Indicates that the binding is from a type to a provider.
        /// </summary>
        Provider,

        /// <summary>
        /// Indicates that the binding is from a type to a callback method.
        /// </summary>
        Method,

        /// <summary>
        /// Indicates that the binding is from a type to a constant value.
        /// </summary>
        Constant,
    }
}