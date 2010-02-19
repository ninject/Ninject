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
using Ninject.Injection;
using Ninject.Planning.Targets;
#endregion

namespace Ninject.Planning.Directives
{
	/// <summary>
	/// Describes the injection of a property.
	/// </summary>
	public class PropertyInjectionDirective : IDirective
	{
		/// <summary>
		/// Gets or sets the injector that will be triggered.
		/// </summary>
		public PropertyInjector Injector { get; private set; }

		/// <summary>
		/// Gets or sets the injection target for the directive.
		/// </summary>
		public ITarget Target { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyInjectionDirective"/> class.
		/// </summary>
		/// <param name="member">The member the directive describes.</param>
		/// <param name="injector">The injector that will be triggered.</param>
		public PropertyInjectionDirective(PropertyInfo member, PropertyInjector injector)
		{
			Injector = injector;
			Target = CreateTarget(member);
		}

		/// <summary>
		/// Creates a target for the property.
		/// </summary>
		/// <param name="propertyInfo">The property.</param>
		/// <returns>The target for the property.</returns>
		protected virtual ITarget CreateTarget(PropertyInfo propertyInfo)
		{
			return new PropertyTarget(propertyInfo);
		}
	}
}
