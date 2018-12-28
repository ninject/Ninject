// -------------------------------------------------------------------------------------------------
// <copyright file="ConstructorInjectionDirective.cs" company="Ninject Project Contributors">
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

namespace Ninject.Planning.Directives
{
    using System;
    using System.Reflection;

    using Ninject.Injection;

    /// <summary>
    /// Describes the injection of a constructor.
    /// </summary>
    public class ConstructorInjectionDirective : MethodInjectionDirectiveBase<ConstructorInfo, ConstructorInjector>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorInjectionDirective"/> class.
        /// </summary>
        /// <param name="constructor">The constructor described by the directive.</param>
        /// <param name="injector">The injector that will be triggered.</param>
        /// <exception cref="ArgumentNullException"><paramref name="constructor"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="injector"/> is <see langword="null"/>.</exception>
        public ConstructorInjectionDirective(ConstructorInfo constructor, ConstructorInjector injector)
            : base(constructor, injector)
        {
            this.Constructor = constructor;
        }

        /// <summary>
        /// Gets the base .ctor definition.
        /// </summary>
        public ConstructorInfo Constructor { get; private set; }
    }
}