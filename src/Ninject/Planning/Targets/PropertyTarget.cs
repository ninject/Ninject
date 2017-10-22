// -------------------------------------------------------------------------------------------------
// <copyright file="PropertyTarget.cs" company="Ninject Project Contributors">
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

namespace Ninject.Planning.Targets
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Represents an injection target for a <see cref="PropertyInfo"/>.
    /// </summary>
    public class PropertyTarget : Target<PropertyInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyTarget"/> class.
        /// </summary>
        /// <param name="site">The property that this target represents.</param>
        public PropertyTarget(PropertyInfo site)
            : base(site, site)
        {
        }

        /// <summary>
        /// Gets the name of the target.
        /// </summary>
        public override string Name
        {
            get { return this.Site.Name; }
        }

        /// <summary>
        /// Gets the type of the target.
        /// </summary>
        public override Type Type
        {
            get { return this.Site.PropertyType; }
        }
    }
}