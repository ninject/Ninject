//-------------------------------------------------------------------------------------------------
// <copyright file="Parameter.cs" company="Ninject Project Contributors">
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
namespace Ninject.Parameters
{
    using System;
    using System.Diagnostics.Contracts;
    using Ninject.Activation;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Modifies an activation process in some way.
    /// </summary>
    public class Parameter : IParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="valueCallback">The callback that will be triggered to get the parameter's value.</param>
        /// <param name="shouldInherit">Whether the parameter should be inherited into child requests.</param>
        public Parameter(string name, Func<IContext, object> valueCallback, bool shouldInherit)
        {
            Contract.Requires(name != null);
            Contract.Requires(valueCallback != null);

            this.Name = name;
            this.ValueCallback = (ctx, target) => valueCallback(ctx);
            this.ShouldInherit = shouldInherit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="valueCallback">The callback that will be triggered to get the parameter's value.</param>
        /// <param name="shouldInherit">Whether the parameter should be inherited into child requests.</param>
        public Parameter(string name, Func<IContext, ITarget, object> valueCallback, bool shouldInherit)
        {
            Contract.Requires(name != null);
            Contract.Requires(valueCallback != null);

            this.Name = name;
            this.ValueCallback = valueCallback;
            this.ShouldInherit = shouldInherit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="shouldInherit">Whether the parameter should be inherited into child requests.</param>
        public Parameter(string name, object value, bool shouldInherit)
            : this(name, (ctx, target) => value, shouldInherit)
        {
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the parameter should be inherited into child requests.
        /// </summary>
        public bool ShouldInherit { get; private set; }

        /// <summary>
        /// Gets the callback that will be triggered to get the parameter's value.
        /// </summary>
        public Func<IContext, ITarget, object> ValueCallback { get; internal set; }

        /// <summary>
        /// Gets the value for the parameter within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>The value for the parameter.</returns>
        public object GetValue(IContext context, ITarget target)
        {
            Contract.Requires(context != null);
            return this.ValueCallback(context, target);
        }

        /// <summary>
        /// Determines whether the object equals the specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this object.</param>
        /// <returns><c>True</c> if the objects are equal; otherwise <c>false</c></returns>
        public override bool Equals(object obj)
        {
            var parameter = obj as IParameter;
            return parameter != null ? this.Equals(parameter) : base.Equals(obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the object.</returns>
        public override int GetHashCode()
        {
            return this.GetType().GetHashCode() ^ this.Name.GetHashCode();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><c>True</c> if the objects are equal; otherwise <c>false</c></returns>
        public bool Equals(IParameter other)
        {
            return other.GetType() == this.GetType() && other.Name.Equals(this.Name);
        }
    }
}