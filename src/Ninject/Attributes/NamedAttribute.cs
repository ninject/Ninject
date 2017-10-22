// -------------------------------------------------------------------------------------------------
// <copyright file="NamedAttribute.cs" company="Ninject Project Contributors">
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

namespace Ninject
{
    using Ninject.Infrastructure;
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
            Ensure.ArgumentNotNullOrEmpty(name, "name");

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
            Ensure.ArgumentNotNull(metadata, "metadata");

            return metadata.Name == this.Name;
        }
    }
}