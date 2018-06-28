// -------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsForDictionary.cs" company="Ninject Project Contributors">
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

namespace Ninject.Infrastructure.Language
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides extension methods for <see cref="IDictionary{TKey, TValue}"/>.
    /// </summary>
    internal static class ExtensionsForDictionary
    {
        /// <summary>
        /// Adds a value to the value collection.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value item.</typeparam>
        /// <param name="dictionary">The dictionary to manipulate.</param>
        /// <param name="key">The key of the value collection.</param>
        /// <param name="value">The value to add.</param>
        public static void Add<TKey, TValue>(this IDictionary<TKey, ICollection<TValue>> dictionary, TKey key, TValue value)
        {
            if (dictionary.TryGetValue(key, out ICollection<TValue> values))
            {
                values.Add(value);
            }
            else
            {
                dictionary[key] = new List<TValue> { value };
            }
        }

        /// <summary>
        /// Removes a value from the value collection.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value item.</typeparam>
        /// <param name="dictionary">The dictionary to manipulate.</param>
        /// <param name="key">The key of the value collection.</param>
        /// <param name="value">The value to remove.</param>
        public static void Remove<TKey, TValue>(this IDictionary<TKey, ICollection<TValue>> dictionary, TKey key, TValue value)
        {
            if (dictionary.TryGetValue(key, out ICollection<TValue> values))
            {
                values.Remove(value);
            }
        }

        /// <summary>
        /// Clones the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value item.</typeparam>
        /// <param name="dictionary">The dictionary to clone.</param>
        /// <returns>The cloned dictionary.</returns>
        public static IDictionary<TKey, ICollection<TValue>> Clone<TKey, TValue>(this IDictionary<TKey, ICollection<TValue>> dictionary)
        {
            var clonedDictionary = new Dictionary<TKey, ICollection<TValue>>();

            foreach (var key in dictionary.Keys)
            {
                clonedDictionary.Add(key, dictionary[key].ToList());
            }

            return clonedDictionary;
        }
    }
}