// -------------------------------------------------------------------------------------------------
// <copyright file="IBindingPrecedenceComparer.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject
{
    using System.Collections.Generic;

    using Ninject.Components;
    using Ninject.Planning.Bindings;

    /// <summary>
    /// The binding precedence comparer interface
    /// </summary>
    public interface IBindingPrecedenceComparer : INinjectComponent, IComparer<IBinding>
    {
    }
}