// -------------------------------------------------------------------------------------------------
// <copyright file="GlobalKernelRegistrationModule.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject
{
    using Ninject.Modules;

    /// <summary>
    /// Registers the kernel into which the module is loaded on the GlobalKernelRegistry using the
    /// type specified by TGlobalKernelRegistry.
    /// </summary>
    /// <typeparam name="TGlobalKernelRegistry">The type that is used to register the kernel.</typeparam>
    public abstract class GlobalKernelRegistrationModule<TGlobalKernelRegistry> : NinjectModule
        where TGlobalKernelRegistry : GlobalKernelRegistration
    {
        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            GlobalKernelRegistration.RegisterKernelForType(this.Kernel, typeof(TGlobalKernelRegistry));
        }

        /// <summary>
        /// Unloads the module from the kernel.
        /// </summary>
        public override void Unload()
        {
            GlobalKernelRegistration.UnregisterKernelForType(this.Kernel, typeof(TGlobalKernelRegistry));
        }
    }
}