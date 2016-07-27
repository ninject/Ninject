//-------------------------------------------------------------------------------------------------
// <copyright file="MethodReflectionStrategy.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2009, Enkari, Ltd.
//   Copyright (c) 2009-2011 Ninject Project Contributors
//   Authors: Nate Kohari (nate@enkari.com)
//            Remo Gloor (remo.gloor@gmail.com)
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   you may not use this file except in compliance with one of the Licenses.
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
//-------------------------------------------------------------------------------------------------
namespace Ninject.Planning.Strategies
{
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using Ninject.Components;
    using Ninject.Injection;
    using Ninject.Planning.Directives;
    using Ninject.Selection;

    /// <summary>
    /// Adds directives to plans indicating which methods should be injected during activation.
    /// </summary>
    public class MethodReflectionStrategy : NinjectComponent, IPlanningStrategy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodReflectionStrategy"/> class.
        /// </summary>
        /// <param name="selector">The selector component.</param>
        /// <param name="injectorFactory">The injector factory component.</param>
        public MethodReflectionStrategy(ISelector selector, IInjectorFactory injectorFactory)
        {
            Contract.Requires(selector != null);
            Contract.Requires(injectorFactory != null);

            this.Selector = selector;
            this.InjectorFactory = injectorFactory;
        }

        /// <summary>
        /// Gets the selector component.
        /// </summary>
        public ISelector Selector { get; private set; }

        /// <summary>
        /// Gets the injector factory component.
        /// </summary>
        public IInjectorFactory InjectorFactory { get; private set; }

        /// <summary>
        /// Adds a <see cref="MethodInjectionDirective"/> to the plan for each method
        /// that should be injected.
        /// </summary>
        /// <param name="plan">The plan that is being generated.</param>
        public void Execute(IPlan plan)
        {
            Contract.Requires(plan != null);

            foreach (MethodInfo method in this.Selector.SelectMethodsForInjection(plan.Type))
            {
                plan.Add(new MethodInjectionDirective(plan.Type, method, this.InjectorFactory.Create(method)));
            }
        }
    }
}