// -------------------------------------------------------------------------------------------------
// <copyright file="ConstructorReflectionStrategy.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Planning.Strategies
{
    using System;
    using System.Reflection;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Language;
    using Ninject.Injection;
    using Ninject.Planning.Directives;
    using Ninject.Selection;

    /// <summary>
    /// Adds a directive to plans indicating which constructor should be injected during activation.
    /// </summary>
    public class ConstructorReflectionStrategy : NinjectComponent, IPlanningStrategy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorReflectionStrategy"/> class.
        /// </summary>
        /// <param name="selector">The selector component.</param>
        /// <param name="injectorFactory">The injector factory component.</param>
        public ConstructorReflectionStrategy(ISelector selector, IInjectorFactory injectorFactory)
        {
            Ensure.ArgumentNotNull(selector, "selector");
            Ensure.ArgumentNotNull(injectorFactory, "injectorFactory");

            this.Selector = selector;
            this.InjectorFactory = injectorFactory;
        }

        /// <summary>
        /// Gets the selector component.
        /// </summary>
        public ISelector Selector { get; private set; }

        /// <summary>
        /// Gets or sets the injector factory component.
        /// </summary>
        public IInjectorFactory InjectorFactory { get; set; }

        /// <summary>
        /// Adds a <see cref="ConstructorInjectionDirective"/> to the plan for the constructor
        /// that should be injected.
        /// </summary>
        /// <param name="plan">The plan that is being generated.</param>
        public void Execute(IPlan plan)
        {
            Ensure.ArgumentNotNull(plan, "plan");

            var constructors = this.Selector.SelectConstructorsForInjection(plan.Type);
            if (constructors == null)
            {
                return;
            }

            foreach (ConstructorInfo constructor in constructors)
            {
                var hasInjectAttribute = constructor.HasAttribute(this.Settings.InjectAttribute);
                var hasObsoleteAttribute = constructor.HasAttribute(typeof(ObsoleteAttribute));
                var directive = new ConstructorInjectionDirective(constructor, this.InjectorFactory.Create(constructor))
                {
                    HasInjectAttribute = hasInjectAttribute,
                    HasObsoleteAttribute = hasObsoleteAttribute,
                };

                plan.Add(directive);
            }
        }
    }
}