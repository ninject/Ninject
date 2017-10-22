// -------------------------------------------------------------------------------------------------
// <copyright file="SpecificConstructorSelector.cs" company="Ninject Project Contributors">
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
            return directive.Constructor.Equals(this.constructorInfo) ? 1 : 0;
        }
    }
}