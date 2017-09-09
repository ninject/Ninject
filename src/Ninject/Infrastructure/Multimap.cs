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
    /// <typeparam name="TK">The type of key.</typeparam>
    /// <typeparam name="TV">The type of value.</typeparam>
    public class Multimap<TK, TV> // : IEnumerable<KeyValuePair<K, ICollection<V>>>
    {
        private readonly Dictionary<TK, ICollection<TV>> items = new Dictionary<TK, ICollection<TV>>();

        /// <summary>
        /// Gets the collection of keys.
        /// </summary>
        public ICollection<TK> Keys
        {
            get { return this.items.Keys; }
        }

        /// <summary>
        /// Gets the collection of collections of values.
        /// </summary>
        public ICollection<ICollection<TV>> Values
        {
            get { return this.items.Values; }
        }

        /// <summary>
        /// Gets the collection of values stored under the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public IEnumerable<TV> this[TK key]
        {
            get
            {
                ICollection<TV> result;

                return this.items.TryGetValue(key, out result) ? result : Enumerable.Empty<TV>();
            }
        }

        /// <summary>
        /// Adds the specified value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(TK key, TV value)
        {
            this.GetValues(key).Add(value);
        }

        /// <summary>
        /// Removes the specified value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>True</c> if such a value existed and was removed; otherwise <c>false</c>.</returns>
        public bool Remove(TK key, TV value)
        {
            ICollection<TV> values;

            return this.items.TryGetValue(key, out values) &&
                   values.Remove(value);
        }

        /// <summary>
        /// Removes all values for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>True</c> if any such values existed; otherwise <c>false</c>.</returns>
        public bool RemoveAll(TK key)
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
        public bool ContainsKey(TK key)
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
        public bool TryGetValues(TK key, out IEnumerable<TV> values)
        {
            ICollection<TV> tempValues;

            var result = this.items.TryGetValue(key, out tempValues);
            values = tempValues;

            return result;
        }

        /// <summary>
        /// Clones the multimap.
        /// </summary>
        /// <returns>The cloned multimap.</returns>
        public Multimap<TK, TV> Clone()
        {
            var map = new Multimap<TK, TV>();
            foreach (var key in this.Keys)
            {
                map.items.Add(key, this.items[key].ToList());
            }

            return map;
        }

        private ICollection<TV> GetValues(TK key)
        {
            ICollection<TV> result;
            if (!this.items.TryGetValue(key, out result))
            {
                result = new List<TV>();
                this.items[key] = result;
            }

            return result;
        }
    }
}