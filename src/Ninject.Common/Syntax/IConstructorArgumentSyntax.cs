//-------------------------------------------------------------------------------
// <copyright file="IConstructorArgumentSyntax.cs" company="Ninject Project Contributors">
//   Copyright (c) 2009-2011 Ninject Project Contributors
//   Authors: Remo Gloor (remo.gloor@gmail.com)
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
//-------------------------------------------------------------------------------

namespace Ninject.Syntax
{
    using Ninject.Activation;

    /// <summary>
    /// Passed to ToConstructor to specify that a constructor value is Injected.
    /// </summary>
    public interface IConstructorArgumentSyntax : IFluentSyntax
    {
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        IContext Context { get; }

        /// <summary>
        /// Specifies that the argument is injected.
        /// </summary>
        /// <typeparam name="T">The type of the parameter</typeparam>
        /// <returns>Not used. This interface has no implementation.</returns>
        T Inject<T>();
    }
}