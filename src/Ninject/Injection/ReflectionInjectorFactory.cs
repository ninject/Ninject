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
using Ninject.Components;
#endregion

namespace Ninject.Injection
{
	/// <summary>
	/// Creates injectors from members via reflective invocation.
	/// </summary>
	public class ReflectionInjectorFactory : NinjectComponent, IInjectorFactory
	{
		/// <summary>
		/// Gets or creates an injector for the specified constructor.
		/// </summary>
		/// <param name="constructor">The constructor.</param>
		/// <returns>The created injector.</returns>
		public ConstructorInjector Create(ConstructorInfo constructor)
		{
			return args => constructor.Invoke(args);
		}

		/// <summary>
		/// Gets or creates an injector for the specified property.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns>The created injector.</returns>
		public PropertyInjector Create(PropertyInfo property)
		{
			return (target, value) => property.SetValue(target, value, null);
		}

		/// <summary>
		/// Gets or creates an injector for the specified method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>The created injector.</returns>
		public MethodInjector Create(MethodInfo method)
		{
			return (target, args) => method.Invoke(target, args);
		}
	}
}