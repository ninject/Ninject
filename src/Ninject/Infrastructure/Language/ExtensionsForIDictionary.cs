#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
#endregion

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