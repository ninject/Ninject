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
using System.Linq;
using System.Reflection;
using Ninject.Infrastructure;
using Ninject.Planning.Targets;
#endregion

namespace Ninject.Planning.Directives
{
	/// <summary>
	/// Describes the injection of a method or constructor.
	/// </summary>
	public abstract class MethodInjectionDirectiveBase<TMethod, TInjector> : IDirective
		where TMethod : MethodBase
	{
		/// <summary>
		/// Gets or sets the injector that will be triggered.
		/// </summary>
		public TInjector Injector { get; private set; }

		/// <summary>
		/// Gets or sets the targets for the directive.
		/// </summary>
		public ITarget[] Targets { get; private set; }

		/// <summary>
		/// Initializes a new instance of the MethodInjectionDirectiveBase&lt;TMethod, TInjector&gt; class.
		/// </summary>
		/// <param name="method">The method this directive represents.</param>
		/// <param name="injector">The injector that will be triggered.</param>
		protected MethodInjectionDirectiveBase(TMethod method, TInjector injector)
		{
			Ensure.ArgumentNotNull(method, "method");
			Ensure.ArgumentNotNull(injector, "injector");

			Injector = injector;
			Targets = CreateTargetsFromParameters(method);
		}

		/// <summary>
		/// Creates targets for the parameters of the method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>The targets for the method's parameters.</returns>
		protected virtual ITarget[] CreateTargetsFromParameters(TMethod method)
		{
			return method.GetParameters().Select(parameter => new ParameterTarget(method, parameter)).ToArray();
		}
	}
}
