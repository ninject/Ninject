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
using Ninject.Infrastructure;
using Ninject.Injection;
#endregion

namespace Ninject.Planning.Directives
{
	/// <summary>
	/// Describes the injection of a method.
	/// </summary>
	public class MethodInjectionDirective : MethodInjectionDirectiveBase<MethodInfo, MethodInjector>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MethodInjectionDirective"/> class.
		/// </summary>
		/// <param name="method">The method described by the directive.</param>
		/// <param name="injector">The injector that will be triggered.</param>
		public MethodInjectionDirective(MethodInfo method, MethodInjector injector)
			: base(method, injector) { }
	}
}