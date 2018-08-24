// -------------------------------------------------------------------------------------------------
// <copyright file="ConstructorReflectionStrategy.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2017 Ninject Project Contributors. All rights reserved.
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   You may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Planning.Strategies
{
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Injection;
    using Ninject.Planning.Directives;
    using Ninject.Selection;

    /// <summary>
    /// Adds a directive to plans indicating which constructor should be injected during activation.
    /// </summary>
    public class ConstructorReflectionStrategy : NinjectComponent, IPlanningStrategy
    {
        /// <summary>
        /// the <see cref="ISelector"/> component.
        /// </summary>
        private readonly ISelector selector;

        /// <summary>
        /// The <see cref="IInjectorFactory"/> component.
        /// </summary>
        private readonly IInjectorFactory injectorFactory;

        /// <summary>
        /// The ninject settings.
        /// </summary>
        private readonly INinjectSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorReflectionStrategy"/> class.
        /// </summary>
        /// <param name="selector">The selector component.</param>
        /// <param name="injectorFactory">The injector factory component.</param>
        /// <param name="settings">The ninject settings.</param>
        public ConstructorReflectionStrategy(ISelector selector, IInjectorFactory injectorFactory, INinjectSettings settings)
        {
            Ensure.ArgumentNotNull(selector, "selector");
            Ensure.ArgumentNotNull(injectorFactory, "injectorFactory");
            Ensure.ArgumentNotNull(settings, "settings");

            this.selector = selector;
            this.injectorFactory = injectorFactory;
            this.settings = settings;
        }

        /// <summary>
        /// Adds a serial of <see cref="ConstructorInjectionDirective"/>s to the plan for the constructors
        /// that could be injected.
        /// </summary>
        /// <param name="plan">The plan that is being generated.</param>
        public void Execute(IPlan plan)
        {
            Ensure.ArgumentNotNull(plan, "plan");

            var constructors = this.selector.SelectConstructorsForInjection(plan.Type);

            foreach (var constructor in constructors)
            {
                var directive = new ConstructorInjectionDirective(constructor, this.injectorFactory.Create(constructor));
                plan.Add(directive);
            }
        }
    }
}