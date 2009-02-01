using System;
using System.Collections;
using System.Collections.Generic;

namespace Ninject.Infrastructure
{
	public class Multimap<K, V> : IEnumerable<KeyValuePair<K, ICollection<V>>>
	{
		private readonly Dictionary<K, ICollection<V>> _items = new Dictionary<K, ICollection<V>>();

		public ICollection<V> this[K key]
		{
			get
			{
				if (!_items.ContainsKey(key))
					_items[key] = new List<V>();

				return _items[key];
			}
		}

		public ICollection<K> Keys
		{
			get { return _items.Keys; }
		}

		public ICollection<ICollection<V>> Values
		{
			get { return _items.Values; }
		}

		public void Add(K key, V value)
		{
			this[key].Add(value);
		}

		public bool Remove(K key, V value)
		{
			if (!_items.ContainsKey(key))
				return false;

			return _items[key].Remove(value);
		}

		public bool RemoveAll(K key)
		{
			return _items.Remove(key);
		}

		public void Clear()
		{
			_items.Clear();
		}

		public bool ContainsKey(K key)
		{
			return _items.ContainsKey(key);
		}

		public bool ContainsValue(K key, V value)
		{
			return _items.ContainsKey(key) && _items[key].Contains(value);
		}

		IEnumerator<KeyValuePair<K, ICollection<V>>> IEnumerable<KeyValuePair<K, ICollection<V>>>.GetEnumerator()
		{
			foreach (KeyValuePair<K, ICollection<V>> pair in _items)
				yield return pair;
		}

		public IEnumerator GetEnumerator()
		{
			foreach (KeyValuePair<K, ICollection<V>> pair in _items)
				yield return pair;
		}
	}
}