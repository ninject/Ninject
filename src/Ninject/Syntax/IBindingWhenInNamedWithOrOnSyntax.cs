// -------------------------------------------------------------------------------------------------
// <copyright file="IBindingWhenInNamedWithOrOnSyntax.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Syntax
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Used to set the condition, scope, name, or add additional information or actions to a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1502:ElementMustNotBeOnSingleLine", Justification = "Reviewed. Suppression is OK here.")]
    public interface IBindingWhenInNamedWithOrOnSyntax<T> : IBindingWhenSyntax<T>,
                                                            IBindingInSyntax<T>,
                                                            IBindingNamedSyntax<T>,
                                                            IBindingWithSyntax<T>,
                                                            IBindingOnSyntax<T>
    {
    }
}