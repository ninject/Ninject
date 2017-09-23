// -------------------------------------------------------------------------------------------------
// <copyright file="IInjectorFactory.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Injection
{
    using System.Reflection;
    using Ninject.Components;

    /// <summary>
    /// Creates injectors from members.
    /// </summary>
    public interface IInjectorFactory : INinjectComponent
    {
        /// <summary>
        /// Gets or creates an injector for the specified constructor.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <returns>The created injector.</returns>
        ConstructorInjector Create(ConstructorInfo constructor);

        /// <summary>
        /// Gets or creates an injector for the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The created injector.</returns>
        PropertyInjector Create(PropertyInfo property);

        /// <summary>
        /// Gets or creates an injector for the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The created injector.</returns>
        MethodInjector Create(MethodInfo method);
    }
}