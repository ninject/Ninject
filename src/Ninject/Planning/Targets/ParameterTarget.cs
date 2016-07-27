//-------------------------------------------------------------------------------------------------
// <copyright file="ParameterTarget.cs" company="Ninject Project Contributors">
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
namespace Ninject.Planning.Targets
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Represents an injection target for a <see cref="ParameterInfo"/>.
    /// </summary>
    public class ParameterTarget : Target<ParameterInfo>
    {
        private readonly Lazy<object> defaultValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterTarget"/> class.
        /// </summary>
        /// <param name="service">The service type.</param>
        /// <param name="method">The method that defines the parameter.</param>
        /// <param name="site">The parameter that this target represents.</param>
        public ParameterTarget(Type service, MethodBase method, ParameterInfo site)
            : base(service, method, site)
        {
            this.defaultValue = new Lazy<object>(() => site.DefaultValue);
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
            get { return this.Site.ParameterType; }
        }

        /// <summary>
        /// Gets a value indicating whether the target has a default value.
        /// </summary>
        public override bool HasDefaultValue
        {
            get { return this.defaultValue.Value != DBNull.Value; }
        }

        /// <summary>
        /// Gets the default value for the target.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">If the item does not have a default value.</exception>
        public override object DefaultValue
        {
            get { return this.HasDefaultValue ? this.defaultValue.Value : base.DefaultValue; }
        }
    }
}