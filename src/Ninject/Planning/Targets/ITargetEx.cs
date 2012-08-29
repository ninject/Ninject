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
using System.Reflection;
using Ninject.Activation;
using Ninject.Planning.Bindings;
#endregion

namespace Ninject.Planning.Targets
{
    /// <summary>
    /// Represents a site on a type where a value will be injected.
    /// </summary>
    public interface ITargetEx : ITarget
#if !WINRT
        , ICustomAttributeProvider
#endif
    {
    }
}