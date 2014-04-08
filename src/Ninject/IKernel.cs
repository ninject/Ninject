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
using Ninject.Parameters;
using Ninject.Planning.Bindings;

#endregion

namespace Ninject
{
    /// <summary>
    /// A super-factory that can create objects of all kinds, following hints provided by <see cref="IBinding"/>s.
    /// </summary>
    [Obsolete("Use IKernelConfiguration and IReadonlyKernel")]
    public interface IKernel : IKernelConfiguration, IReadonlyKernel
    {
    }
}
