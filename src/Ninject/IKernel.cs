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
using Ninject.Activation.Blocks;
using Ninject.Infrastructure.Disposal;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Syntax;
#endregion

namespace Ninject
{
    /// <summary>
    /// A super-factory that can create objects of all kinds, following hints provided by <see cref="IBinding"/>s.
    /// </summary>
    public interface IKernel : IKernelConfiguration, IResolutionRoot, IServiceProvider, IDisposableObject
    {
        /// <summary>
        /// Injects the specified existing instance, without managing its lifecycle.
        /// </summary>
        /// <param name="instance">The instance to inject.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        void Inject(object instance, params IParameter[] parameters);

        /// <summary>
        /// Gets the bindings registered for the specified service.
        /// </summary>
        /// <param name="service">The service in question.</param>
        /// <returns>A series of bindings that are registered for the service.</returns>
        IEnumerable<IBinding> GetBindings(Type service);

        /// <summary>
        /// Begins a new activation block, which can be used to deterministically dispose resolved instances.
        /// </summary>
        /// <returns>The new activation block.</returns>
        IActivationBlock BeginBlock();
    }
}
