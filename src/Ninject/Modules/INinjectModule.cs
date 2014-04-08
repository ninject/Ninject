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
using Ninject.Infrastructure;
using Ninject.Syntax;
#endregion

namespace Ninject.Modules
{
    /// <summary>
    /// A pluggable unit that can be loaded into an <see cref="IKernelConfiguration"/>.
    /// </summary>
    public interface INinjectModule
    {
        /// <summary>
        /// Gets the kernel configuration that the module is loaded into.
        /// </summary>
        /// <value>The kernel configuration that the module is loaded into.</value>
        IKernelConfiguration KernelConfiguration { get; }
        
        /// <summary>
        /// Gets the module's name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Called when the module is loaded into a kernel.
        /// </summary>
        /// <param name="kernelConfiguration">The kernel configuration that is loading the module.</param>
        void OnLoad(IKernelConfiguration kernelConfiguration);

        /// <summary>
        /// Called when the module is unloaded from a kernel.
        /// </summary>
        /// <param name="kernelConfiguration">The kernel configuration that is unloading the module.</param>
        void OnUnload(IKernelConfiguration kernelConfiguration);

        /// <summary>
        /// Called after loading the modules. A module can verify here if all other required modules are loaded.
        /// </summary>
        void OnVerifyRequiredModules();
    }
}