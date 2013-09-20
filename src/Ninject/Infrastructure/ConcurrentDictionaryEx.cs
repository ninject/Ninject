#if !NET_35
using System.Collections.Concurrent;
#endif
using System;
using System.Collections;
using System.Collections.Generic;

namespace Ninject.Infrastructure
{
	/// <summary>
	/// Provides concurrent access to the Dictionary via ConcurrentDictionary if provided, otherwise a lock is used internally
	/// </summary>
	/// <typeparam name="TKey">The type of key.</typeparam>
	/// <typeparam name="TValue">The type of value.</typeparam>
	public class ConcurrentDictionaryEx<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
	{
#if !NET_35
		readonly ConcurrentDictionary<TKey, TValue> _dictionary;
#else
		readonly Dictionary<TKey, TValue> _dictionary;
		readonly object _lock = new object();
#endif

		/// <summary>
		/// Initializes a new ConcurrentDictionaryEx
		/// </summary>
		public ConcurrentDictionaryEx() : this(EqualityComparer<TKey>.Default)
		{
		}

		/// <summary>
		/// Initializes a new ConcurrentDictionaryEx with the specified Equality Comparer
		/// </summary>
		/// <param name="equalityComparer">The Equlity Comparer to use on objects</param>
		public ConcurrentDictionaryEx(IEqualityComparer<TKey> equalityComparer)
		{
#if !NET_35
			_dictionary = new ConcurrentDictionary<TKey, TValue>(equalityComparer);
#else
			_dictionary = new Dictionary<TKey, TValue>(equalityComparer);
#endif
		}

		/// <summary>
		/// Gets or sets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key of the value to get or set</param>
		public TValue this[TKey key]
		{
			get {
#if !NET_35
				return _dictionary[key];
#else
				lock(_lock)
					return _dictionary[key];
#endif
			}

			set {
#if !NET_35
				_dictionary[key] = value;
#else
				lock(_lock)
					_dictionary[key] = value;
#endif
			}
		}

		/// <summary>
		/// Gets the number of key/value pairs contained in the ConcurrentDictionaryEx.
		/// </summary>
		public int Count
		{
			get {
#if !NET_35
				return _dictionary.Count;
#else
				lock(_dictionary)
					return _dictionary.Count;
#endif
			}
		}

		/// <summary>
		/// Returns an enumerator that iterates through the ConcurrentDictionaryEx.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
#if !NET_35
			return _dictionary.GetEnumerator();
#else
			lock(_lock)
				return _dictionary.GetEnumerator();
#endif
		}

		/// <summary>
		/// Gets an System.Collections.ICollection object containing the keys of the System.Collections.IDictionary object.
		/// </summary>
		public ICollection<TKey> Keys
		{
			get {
#if !NET_35
				return _dictionary.Keys;
#else
				lock(_lock)
					return _dictionary.Keys;
#endif
			}
		}

		/// <summary>
		/// Gets a collection containing the values in the ConcurrentDictionaryEx{TKey,TValue}
		/// </summary>
		public ICollection<TValue> Values
		{
			get {
#if !NET_35
				return _dictionary.Values;
#else
				lock(_lock)
					return _dictionary.Values;
#endif
			}
		}

		/// <summary>
		/// Removes all keys and values from the ConcurrentDictionaryEx
		/// </summary>
		public void Clear()
		{
#if !NET_35
			_dictionary.Clear();
#else
			lock(_lock)
				_dictionary.Clear();
#endif
		}

		/// <summary>
		/// Determines whether the ConcurrentDictionaryEx contains the specified key.
		/// </summary>
		/// <param name="key">The key to locate in the ConcurrentDictionaryEx.</param>
		/// <returns>true if the ConcurrentDictionaryEx contains an element with the specified key; otherwise, false.</returns>
		public bool ContainsKey(TKey key)
		{
#if !NET_35
			return _dictionary.ContainsKey(key);
#else
			lock(_lock)
				return _dictionary.ContainsKey(key);
#endif
		}

		/// <summary>
		/// Attempts to get the value associated with the specified key from the ConcurrentDictionaryEx.
		/// </summary>
		/// <param name="key">The key of the value to get.</param>
		/// <param name="value">When this method returns, value contains the object from the ConcurrentDictionaryEx with the specified key or the default value of , if the operation failed.</param>
		/// <returns>true if the key was found in the ConcurrentDictionaryEx; otherwise, false.</returns>
		public bool TryGetValue(TKey key, out TValue value)
		{
#if !NET_35
			return _dictionary.TryGetValue(key, out value);
#else
			lock(_lock) {
				if (!_dictionary.ContainsKey(key)) {
					value = default(TValue);
					return false;
				}
			
				value = _dictionary[key];
				return true;
			}
#endif
		}

		/// <summary>
		/// Adds a key/value pair to the ConcurrentDictionaryEx if the key does not already exist.
		/// </summary>
		/// <param name="key">The key of the element to add.</param>
		/// <param name="value">The value to be added, if the key does not already exist</param>
		/// <returns>The value for the key. This will be either the existing value for the key if the key is already in the dictionary, or the new value if the key was not in the dictionary.</returns>
		public TValue GetOrAdd(TKey key, TValue value)
		{
#if !NET_35
			return _dictionary.GetOrAdd(key, value);
#else
			lock(_lock) {
				if (!_dictionary.ContainsKey(key))
					_dictionary[key] = value;

				return _dictionary[key];
			}
#endif
		}

		/// <summary>
		/// Adds a key/value pair to the ConcurrentDictionaryEx if the key does not already exist.
		/// </summary>
		/// <param name="key">The key of the element to add.</param>
		/// <param name="valueFactory">The function used to generate a value for the key</param>
		/// <returns>The value for the key. This will be either the existing value for the key if the key is already in the dictionary, or the new value for the key as returned by valueFactory if the key was not in the dictionary.</returns>
		public TValue GetOrAdd(TKey key, System.Func<TKey, TValue> valueFactory)
		{
#if !NET_35
			return _dictionary.GetOrAdd(key, valueFactory);
#else
			lock(_lock) {
				if (!_dictionary.ContainsKey(key))
					_dictionary[key] = valueFactory(key);

				return _dictionary[key];
			}
#endif
		}

		/// <summary>
		/// Attempts to add the specified key and value to the ConcurrentDictionaryEx
		/// </summary>
		/// <param name="key">The key of the element to add.</param>
		/// <param name="value">The value of the element to add. The value can be a null reference (Nothing in Visual Basic) for reference types.</param>
		/// <returns>The value for the key. This will be either the existing value for the key if the key is already in the dictionary, or the new value if the key was not in the dictionary.</returns>
		public bool TryAdd(TKey key, TValue value)
		{
#if !NET_35
			return _dictionary.TryAdd(key, value);
#else
			lock(_lock) {
				if (_dictionary.ContainsKey(key))
					return false;
			
				_dictionary[key] = value;
				return true;
			}
#endif
		}

		/// <summary>
		/// Attempts to remove and return the value with the specified key from the ConcurrentDictionaryEx.
		/// </summary>
		/// <param name="key">The key of the element to remove and return.</param>
		/// <param name="value">When this method returns, value contains the object removed from the ConcurrentDictionaryEx or the default value of if the operation failed.</param>
		/// <returns>true if an object was removed successfully; otherwise, false.</returns>
		public bool TryRemove(TKey key, out TValue value)
		{
#if !NET_35
			return _dictionary.TryRemove(key, out value);
#else
			lock(_lock) {
				if (!_dictionary.ContainsKey(key)) {
					value = default(TValue);
					return false;
				}

				value = _dictionary[key];
				_dictionary.Remove(key);
				return true;
			}
#endif
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}