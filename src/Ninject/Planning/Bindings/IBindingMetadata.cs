using System;

namespace Ninject.Planning.Bindings
{
	public interface IBindingMetadata
	{
		string Name { get; set; }
		bool Has(string key);
		object Get(string key);
		T Get<T>(string key);
		void Set(string key, object value);
	}
}