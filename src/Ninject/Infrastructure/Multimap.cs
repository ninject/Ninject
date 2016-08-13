//-------------------------------------------------------------------------------------------------
// <copyright file="Multimap.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2016, Ninject Project Contributors
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
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A data structure that contains multiple values for a each key.
    /// </summary>
    /// <typeparam name="K">The type of key.</typeparam>
    /// <typeparam name="V">The type of value.</typeparam>
    public class Multimap<K, V> // : IEnumerable<KeyValuePair<K, ICollection<V>>>
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
        public IEnumerable<V> this[K key]
        {
            get
            {
                ICollection<V> result;

                return this.items.TryGetValue(key, out result) ? result : Enumerable.Empty<V>();
            }
        }

        /// <summary>
        /// Adds the specified value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(K key, V value)
        {
            this.GetValues(key).Add(value);
        }

        /// <summary>
        /// Removes the specified value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>True</c> if such a value existed and was removed; otherwise <c>false</c>.</returns>
        public bool Remove(K key, V value)
        {
            ICollection<V> values;

            return this.items.TryGetValue(key, out values) &&
                   values.Remove(value);
        }

        /// <summary>
        /// Removes all values for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>True</c> if any such values existed; otherwise <c>false</c>.</returns>
        public bool RemoveAll(K key)
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
        /// <returns><c>True</c> if the multimap has one or more values for the specified key; otherwise, <c>false</c>.</returns>
        public bool ContainsKey(K key)
        {
            return this.items.ContainsKey(key);
        }

        /*
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
        }*/

        /// <summary>
        /// Gets the values associated with the given key
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="values">The values</param>
        /// <returns><c>True</c> when values are found or <c>false</c> if not</returns>
        public bool TryGetValues(K key, out IEnumerable<V> values)
        {
            ICollection<V> tempValues;

            var result = this.items.TryGetValue(key, out tempValues);
            values = tempValues;

            return result;
        }

        private ICollection<V> GetValues(K key)
        {
            ICollection<V> result;
            if (!this.items.TryGetValue(key, out result))
            {
                result = new List<V>();
                this.items[key] = result;
            }

            return result;
        }

        public Multimap<K, V> Clone()
        {
            var map = new Multimap<K, V>();
            foreach (var key in Keys)
                map.items.Add(key, items[key].ToList());

            return map;
        }

    }
}