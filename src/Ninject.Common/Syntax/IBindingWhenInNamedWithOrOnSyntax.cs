//-------------------------------------------------------------------------------
// <copyright file="IBindingWhenInNamedWithOrOnSyntax.cs" company="Ninject Project Contributors">
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
//-------------------------------------------------------------------------------

namespace Ninject.Syntax
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Used to set the condition, scope, name, or add additional information or actions to a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1502:ElementMustNotBeOnSingleLine", Justification = "Reviewed. Suppression is OK here.")]
    public interface IBindingWhenInNamedWithOrOnSyntax<T> : IBindingWhenSyntax<T>,
                                                            IBindingInSyntax<T>,
                                                            IBindingNamedSyntax<T>,
                                                            IBindingWithSyntax<T>,
                                                            IBindingOnSyntax<T>
    {
    }
}