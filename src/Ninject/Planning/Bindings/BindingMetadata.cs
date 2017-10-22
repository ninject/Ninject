// -------------------------------------------------------------------------------------------------
// <copyright file="BindingMetadata.cs" company="Ninject Project Contributors">
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

namespace Ninject.Planning.Bindings
{
    using System.Collections.Generic;

    using Ninject.Infrastructure;

    /// <summary>
    /// Additional information available about a binding, which can be used in constraints
    /// to select bindings to use in activation.
    /// </summary>
    public class BindingMetadata : IBindingMetadata
    {
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the binding's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Determines whether a piece of metadata with the specified key has been defined.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <returns><c>True</c> if such a piece of metadata exists; otherwise, <c>false</c>.</returns>
        public bool Has(string key)
        {
            Ensure.ArgumentNotNullOrEmpty(key, "key");

            return this.values.ContainsKey(key);
        }

        /// <summary>
        /// Gets the value of metadata defined with the specified key, cast to the specified type.
        /// </summary>
        /// <typeparam name="T">The type of value to expect.</typeparam>
        /// <param name="key">The metadata key.</param>
        /// <returns>The metadata value.</returns>
        public T Get<T>(string key)
        {
            Ensure.ArgumentNotNullOrEmpty(key, "key");

            return this.Get(key, default(T));
        }

        /// <summary>
        /// Gets the value of metadata defined with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of value to expect.</typeparam>
        /// <param name="key">The metadata key.</param>
        /// <param name="defaultValue">The value to return if the binding has no metadata set with the specified key.</param>
        /// <returns>The metadata value, or the default value if none was set.</returns>
        public T Get<T>(string key, T defaultValue)
        {
            Ensure.ArgumentNotNullOrEmpty(key, "key");

            return this.values.ContainsKey(key) ? (T)this.values[key] : defaultValue;
        }

        /// <summary>
        /// Sets the value of a piece of metadata.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        public void Set(string key, object value)
        {
            Ensure.ArgumentNotNullOrEmpty(key, "key");

            this.values[key] = value;
        }
    }
}