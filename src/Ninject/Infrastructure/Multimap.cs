//-------------------------------------------------------------------------------------------------
// <copyright file="Multimap.cs" company="Ninject Project Contributors">
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
namespace Ninject.Infrastructure
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// A data structure that contains multiple values for a each key.
    /// </summary>
    /// <typeparam name="K">The type of key.</typeparam>
    /// <typeparam name="V">The type of value.</typeparam>
    public class Multimap<K, V> : IEnumerable<KeyValuePair<K, ICollection<V>>>
    {
        private readonly Dictionary<K, ICollection<V>> items = new Dictionary<K, ICollection<V>>();

        /// <summary>
        /// Gets the collection of keys.
        /// </summary>
        public ICollection<K> Keys
        {
            get { return this.items.Keys; }
        }

        /// <summary>
        /// Gets the collection of collections of values.
        /// </summary>
        public ICollection<ICollection<V>> Values
        {
            get { return this.items.Values; }
        }

        /// <summary>
        /// Gets the collection of values stored under the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public ICollection<V> this[K key]
        {
            get
            {
                Contract.Requires(key != null);

                if (!this.items.ContainsKey(key))
                {
                    this.items[key] = new List<V>();
                }

                return this.items[key];
            }
        }

        /// <summary>
        /// Adds the specified value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(K key, V value)
        {
            Contract.Requires(key != null);
            Contract.Requires(value != null);

            this[key].Add(value);
        }

        /// <summary>
        /// Removes the specified value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>True</c> if such a value existed and was removed; otherwise <c>false</c>.</returns>
        public bool Remove(K key, V value)
        {
            Contract.Requires(key != null);
            Contract.Requires(value != null);

            if (!this.items.ContainsKey(key))
            {
                return false;
            }

            return this.items[key].Remove(value);
        }

        /// <summary>
        /// Removes all values for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>True</c> if any such values existed; otherwise <c>false</c>.</returns>
        public bool RemoveAll(K key)
        {
            Contract.Requires(key != null);
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
        /// <returns><c>True</c> if the multimap has one or more values for the specified key; otherwise, <c>false</c>.</returns>
        public bool ContainsKey(K key)
        {
            Contract.Requires(key != null);
            return this.items.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the multimap contains the specified value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>True</c> if the multimap contains such a value; otherwise, <c>false</c>.</returns>
        public bool ContainsValue(K key, V value)
        {
            Contract.Requires(key != null);
            Contract.Requires(value != null);

            return this.items.ContainsKey(key) && this.items[key].Contains(value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a the multimap.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the multimap.</returns>
        public IEnumerator GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        IEnumerator<KeyValuePair<K, ICollection<V>>> IEnumerable<KeyValuePair<K, ICollection<V>>>.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }
    }
}