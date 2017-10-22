// -------------------------------------------------------------------------------------------------
// <copyright file="WeakConstructorArgument.cs" company="Ninject Project Contributors">
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

namespace Ninject.Parameters
{
    using System;

    using Ninject.Activation;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Overrides the injected value of a constructor argument.
    /// </summary>
    public class WeakConstructorArgument : Parameter, IConstructorArgument
    {
        /// <summary>
        /// A weak reference to the constructor argument value.
        /// </summary>
        private readonly WeakReference weakReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakConstructorArgument"/> class.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="value">The value to inject into the property.</param>
        public WeakConstructorArgument(string name, object value)
            : this(name, value, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakConstructorArgument"/> class.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="value">The value to inject into the property.</param>
        /// <param name="shouldInherit">Whether the parameter should be inherited into child requests.</param>
        public WeakConstructorArgument(string name, object value, bool shouldInherit)
            : base(name, value, shouldInherit)
        {
            this.weakReference = new WeakReference(value);
            this.ValueCallback = (ctx, target) => this.weakReference.Target;
        }

        /// <summary>
        /// Determines if the parameter applies to the given target.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>
        /// True if the parameter applies in the specified context to the specified target.
        /// </returns>
        /// <remarks>
        /// Only one parameter may return true.
        /// </remarks>
        public bool AppliesToTarget(IContext context, ITarget target)
        {
            return string.Equals(this.Name, target.Name);
        }
    }
}