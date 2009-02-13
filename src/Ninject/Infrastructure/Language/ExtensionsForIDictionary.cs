using System;
using System.Collections.Generic;

namespace Ninject.Infrastructure.Language
{
	internal static class ExtensionsForIDictionary
	{
		public static V GetOrAddNew<K, V>(this IDictionary<K, V> dictionary, K key, Func<K, V> createCallback)
		{
			V value;

			if (dictionary.ContainsKey(key))
			{
				value = dictionary[key];
			}
			else
			{
				value = createCallback(key);
				dictionary.Add(key, value);
			}

			return value;
		}
	}
}