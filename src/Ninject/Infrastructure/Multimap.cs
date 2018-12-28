// -------------------------------------------------------------------------------------------------
// <copyright file="Multimap.cs" company="Ninject Project Contributors">
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

namespace Ninject.Infrastructure
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A data structure that contains multiple values for a each key.
    /// </summary>
    /// <typeparam name="TKey">The type of key.</typeparam>
    /// <typeparam name="TValue">The type of value.</typeparam>
    public class Multimap<TKey, TValue> : IEnumerable<KeyValuePair<TKey, IList<TValue>>>
    {
        private readonly Dictionary<TKey, IList<TValue>> items = new Dictionary<TKey, IList<TValue>>();

        /// <summary>
        /// Gets the collection of keys.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { return this.items.Keys; }
        }

        /// <summary>
        /// Gets the collection of collections of values.
        /// </summary>
        public ICollection<IList<TValue>> Values
        {
            get { return this.items.Values; }
        }

        /// <summary>
        /// Gets the collection of values stored under the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        public IList<TValue> this[TKey key]
        {
            get
            {
                if (this.items.TryGetValue(key, out var values))
                {
                    return values;
                }

                values = new List<TValue>();
                this.items[key] = values;
                return values;
            }
        }

        /// <summary>
        /// Adds the specified value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        public void Add(TKey key, TValue value)
        {
            Ensure.ArgumentNotNull(key, nameof(key));
            Ensure.ArgumentNotNull(value, nameof(value));

            this[key].Add(value);
        }

        /// <summary>
        /// Removes the specified value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// <see langword="true"/> if such a value existed and was removed; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        public bool Remove(TKey key, TValue value)
        {
            Ensure.ArgumentNotNull(key, nameof(key));
            Ensure.ArgumentNotNull(value, nameof(value));

            if (this.items.TryGetValue(key, out var values))
            {
                return values.Remove(value);
            }

            return false;
        }

        /// <summary>
        /// Removes all values for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// <see langword="true"/> if any such values existed; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        public bool RemoveAll(TKey key)
        {
            return this.items.Remove(key);
        }

        /// <summary>
        /// Removes all values.
        /// </summary>
        public void Clear()
        {
            this.items.Clear();
        }

        /// <summary>
        /// Determines whether the multimap contains any values for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// <see langword="true"/> if the multimap has one or more values for the specified key; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        public bool ContainsKey(TKey key)
        {
            return this.items.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the multimap contains the specified value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// <see langword="true"/> if the multimap contains such a value; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        public bool ContainsValue(TKey key, TValue value)
        {
            Ensure.ArgumentNotNull(key, "key");
            Ensure.ArgumentNotNull(value, "value");

            return this.items.TryGetValue(key, out var values) && values.Contains(value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a the multimap.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the multimap.</returns>
        public IEnumerator GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a the multimap.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> object that can be used to iterate through the multimap.</returns>
        IEnumerator<KeyValuePair<TKey, IList<TValue>>> IEnumerable<KeyValuePair<TKey, IList<TValue>>>.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }
    }
}