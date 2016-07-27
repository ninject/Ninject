//-------------------------------------------------------------------------------------------------
// <copyright file="NamedAttribute.cs" company="Ninject Project Contributors">
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
namespace Ninject
{
    using System.Diagnostics.Contracts;
    using Ninject.Planning.Bindings;

    /// <summary>
    /// Indicates that the decorated member should only be injected using binding(s) registered
    /// with the specified name.
    /// </summary>
    public class NamedAttribute : ConstraintAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the binding(s) to use.</param>
        public NamedAttribute(string name)
        {
            Contract.Requires(name != null);
            this.Name = name;
        }

        /// <summary>
        /// Gets the binding name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Determines whether the specified binding metadata matches the constraint.
        /// </summary>
        /// <param name="metadata">The metadata in question.</param>
        /// <returns><c>True</c> if the metadata matches; otherwise <c>false</c>.</returns>
        public override bool Matches(IBindingMetadata metadata)
        {
            Contract.Requires(metadata != null);
            return metadata.Name == this.Name;
        }
    }
}