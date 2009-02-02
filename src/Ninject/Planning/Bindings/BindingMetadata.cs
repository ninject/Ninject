using System;
using System.Collections.Generic;

namespace Ninject.Planning.Bindings
{
	public class BindingMetadata : IBindingMetadata
	{
		private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

		public string Name { get; set; }

		public bool Has(string key)
		{
			return _values.ContainsKey(key);
		}

		public T Get<T>(string key)
		{
			return (T)Get(key);
		}

		public object Get(string key)
		{
			return _values.ContainsKey(key) ? _values[key] : null;
		}

		public void Set(string key, object value)
		{
			_values[key] = value;
		}
	}
}