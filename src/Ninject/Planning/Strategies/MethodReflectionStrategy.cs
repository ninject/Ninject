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
using Ninject.Infrastructure;
using Ninject.Injection;
using Ninject.Planning.Directives;
using Ninject.Selection;
#endregion

namespace Ninject.Planning.Strategies
{
	/// <summary>
	/// Adds directives to plans indicating which methods should be injected during activation.
	/// </summary>
	public class MethodReflectionStrategy : NinjectComponent, IPlanningStrategy
	{
		/// <summary>
		/// Gets the selector component.
		/// </summary>
		public ISelector Selector { get; private set; }

		/// <summary>
		/// Gets the injector factory component.
		/// </summary>
		public IInjectorFactory InjectorFactory { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodReflectionStrategy"/> class.
		/// </summary>
		/// <param name="selector">The selector component.</param>
		/// <param name="injectorFactory">The injector factory component.</param>
		public MethodReflectionStrategy(ISelector selector, IInjectorFactory injectorFactory)
		{
			Ensure.ArgumentNotNull(selector, "selector");
			Ensure.ArgumentNotNull(injectorFactory, "injectorFactory");

			Selector = selector;
			InjectorFactory = injectorFactory;
		}

		/// <summary>
		/// Adds a <see cref="MethodInjectionDirective"/> to the plan for each method
		/// that should be injected.
		/// </summary>
		/// <param name="plan">The plan that is being generated.</param>
		public void Execute(IPlan plan)
		{
			Ensure.ArgumentNotNull(plan, "plan");

			foreach (MethodInfo method in Selector.SelectMethodsForInjection(plan.Type))
				plan.Add(new MethodInjectionDirective(method, InjectorFactory.Create(method)));
		}
	}
}