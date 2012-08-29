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
using Ninject.Activation;
using Ninject.Parameters;
#endregion

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
        Constant
    }
}