// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 

namespace Ninject.Planning.Bindings
{
    using Ninject.Syntax;

    /// <summary>
    /// The syntax to define bindings.
    /// </summary>
    /// <typeparam name="T">The type of the service.</typeparam>
    public interface IBindingConfigurationSyntax<T> : 
        IBindingWhenInNamedWithOrOnSyntax<T>, 
        IBindingInNamedWithOrOnSyntax<T>, 
        IBindingNamedWithOrOnSyntax<T>, 
        IBindingWithOrOnSyntax<T>
    {
    }
}