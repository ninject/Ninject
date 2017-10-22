// -------------------------------------------------------------------------------------------------
// <copyright file="ConstantProvider.cs" company="Ninject Project Contributors">
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

namespace Ninject.Activation.Providers
{
    /// <summary>
    /// A provider that always returns the same constant value.
    /// </summary>
    /// <typeparam name="T">The type of value that is returned.</typeparam>
    public class ConstantProvider<T> : Provider<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantProvider{T}"/> class.
        /// </summary>
        /// <param name="value">The value that the provider should return.</param>
        public ConstantProvider(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value that the provider will return.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Creates an instance within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The constant value this provider returns.</returns>
        protected override T CreateInstance(IContext context)
        {
            return this.Value;
        }
    }
}