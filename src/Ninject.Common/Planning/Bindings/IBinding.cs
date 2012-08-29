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
    /// Contains information about a service registration.
    /// </summary>
    public interface IBinding : IBindingConfiguration
    {
        /// <summary>
        /// Gets the binding configuration.
        /// </summary>
        /// <value>The binding configuration.</value>
        IBindingConfiguration BindingConfiguration { get; }

        /// <summary>
        /// Gets the service type that is controlled by the binding.
        /// </summary>
        Type Service { get; }
    }
}