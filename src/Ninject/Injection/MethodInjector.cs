// -------------------------------------------------------------------------------------------------
// <copyright file="MethodInjector.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Injection
{
    /// <summary>
    /// Represents a delegate that can inject values into a method.
    /// </summary>
    /// <param name="target">The method info.</param>
    /// <param name="arguments">The arguments used for the method.</param>
    public delegate void MethodInjector(object target, params object[] arguments);
}