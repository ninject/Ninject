//-------------------------------------------------------------------------------
// <copyright file="IConstructorArgumentSyntax.cs" company="Ninject Project Contributors">
//   Copyright (c) 2011 Ninject Project Contributors
//   Author: Remo Gloor (remo.gloor@gmail.com)
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace Ninject.Selection.Heuristics
{
    using System.Reflection;
    using Ninject.Activation;
    using Ninject.Components;
    using Ninject.Planning.Directives;

    /// <summary>
    /// Constructor selector that selects the constructor matching the one passed to the constructor.
    /// </summary>
    public class SpecificConstructorSelector : NinjectComponent, IConstructorScorer
    {
        private readonly ConstructorInfo constructorInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecificConstructorSelector"/> class.
        /// </summary>
        /// <param name="constructorInfo">The constructor info of the constructor that shall be selected.</param>
        public SpecificConstructorSelector(ConstructorInfo constructorInfo)
        {
            this.constructorInfo = constructorInfo;
        }

        /// <summary>
        /// Gets the score for the specified constructor.
        /// </summary>
        /// <param name="context">The injection context.</param>
        /// <param name="directive">The constructor.</param>
        /// <returns>The constructor's score.</returns>
        public virtual int Score(IContext context, ConstructorInjectionDirective directive)
        {
            return directive.Constructor.Equals(constructorInfo) ? 1 : 0;
        }
    }
}