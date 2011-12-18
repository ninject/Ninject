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
using Ninject.Planning.Bindings;
#endregion

namespace Ninject.Infrastructure
{
    /// <summary>
    /// Indicates the object has a reference to a <see cref="IBinding"/>.
    /// </summary>
    public interface IHaveBindingConfiguration
    {
        /// <summary>
        /// Gets the binding.
        /// </summary>
        IBindingConfiguration BindingConfiguration { get; }
    }
}