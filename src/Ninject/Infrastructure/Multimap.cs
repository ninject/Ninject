#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion
#region Using Directives
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace Ninject.Infrastructure
{
	/// <summary>
	/// A data structure that contains multiple values for a each key.
	/// </summary>
	/// <typeparam name="K">The type of key.</typeparam>
	/// <typeparam name="V">The type of value.</typeparam>
	public class Multimap<K, V> : IEnumerable<KeyValuePair<K, ICollection<V>>>
	{
		private readonly Dictionary<K, ICollection<V>> _items = new Dictionary<K, ICollection<V>>();

		/// <summary>
		/// Gets the collection of values stored under the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		public ICollection<V> this[K key]
		{
			get
			{
				Ensure.ArgumentNotNull(key, "key");

				if (!_items.ContainsKey(key))
					_items[key] = new List<V>();

				return _items[key];
			}
		}

		/// <summary>
		/// Gets the collection of keys.
		/// </summary>
		public ICollection<K> Keys
		{
			get { return _items.Keys; }
		}

		/// <summary>
		/// Gets the collection of collections of values.
		/// </summary>
		public ICollection<ICollection<V>> Values
		{
			get { return _items.Values; }
		}

		/// <summary>
		/// Adds the specified value for the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public void Add(K key, V value)
		{
			Ensure.ArgumentNotNull(key, "key");
			Ensure.ArgumentNotNull(value, "value");

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
			Ensure.ArgumentNotNull(key, "key");
			Ensure.ArgumentNotNull(value, "value");

			if (!_items.ContainsKey(key))
				return false;

			return _items[key].Remove(value);
		}

		/// <summary>
		/// Removes all values for the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns><c>True</c> if any such values existed; otherwise <c>false</c>.</returns>
		public bool RemoveAll(K key)
		{
			Ensure.ArgumentNotNull(key, "key");
			return _items.Remove(key);
		}

		/// <summary>
		/// Removes all values.
		/// </summary>
		public void Clear()
		{
			_items.Clear();
		}

		/// <summary>
		/// Determines whether the multimap contains any values for the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns><c>True</c> if the multimap has one or more values for the specified key; otherwise, <c>false</c>.</returns>
		public bool ContainsKey(K key)
		{
			Ensure.ArgumentNotNull(key, "key");
			return _items.ContainsKey(key);
		}

		/// <summary>
		/// Determines whether the multimap contains the specified value for the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns><c>True</c> if the multimap contains such a value; otherwise, <c>false</c>.</returns>
		public bool ContainsValue(K key, V value)
		{
			Ensure.ArgumentNotNull(key, "key");
			Ensure.ArgumentNotNull(value, "value");

			return _items.ContainsKey(key) && _items[key].Contains(value);
		}

		/// <summary>
		/// Returns an enumerator that iterates through a the multimap.
		/// </summary>
		/// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the multimap.</returns>
		public IEnumerator GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		IEnumerator<KeyValuePair<K, ICollection<V>>> IEnumerable<KeyValuePair<K, ICollection<V>>>.GetEnumerator()
		{
			return _items.GetEnumerator();
		}
	}
}