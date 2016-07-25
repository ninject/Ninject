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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Language;
using Ninject.Injection;
using Ninject.Planning.Directives;
using Ninject.Selection;
#endregion

namespace Ninject.Planning.Strategies
{
    /// <summary>
    /// Adds a directive to plans indicating which constructor should be injected during activation.
    /// </summary>
    public class ConstructorReflectionStrategy : NinjectComponent, IPlanningStrategy
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
        /// Initializes a new instance of the <see cref="ConstructorReflectionStrategy"/> class.
        /// </summary>
        /// <param name="selector">The selector component.</param>
        /// <param name="injectorFactory">The injector factory component.</param>
        public ConstructorReflectionStrategy(ISelector selector, IInjectorFactory injectorFactory)
        {
            Contract.Requires(selector != null);
            Contract.Requires(injectorFactory != null);

            Selector = selector;
            InjectorFactory = injectorFactory;
        }

        /// <summary>
        /// Adds a <see cref="ConstructorInjectionDirective"/> to the plan for the constructor
        /// that should be injected.
        /// </summary>
        /// <param name="plan">The plan that is being generated.</param>
        public void Execute(IPlan plan)
        {
            Contract.Requires(plan != null);

            var constructors = Selector.SelectConstructorsForInjection(plan.Type);
            if (constructors == null)
                return;

            foreach (ConstructorInfo constructor in constructors)
            {
                var hasInjectAttribute = constructor.HasAttribute(Settings.InjectAttribute);
                plan.Add(
                    new ConstructorInjectionDirective(plan.Type, constructor, InjectorFactory.Create(constructor))
                    {
                        HasInjectAttribute = hasInjectAttribute
                    });
            }
        }
    }
}