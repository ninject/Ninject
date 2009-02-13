using System;
using System.Collections.Generic;

namespace Ninject.Planning.Bindings
{
	/// <summary>
	/// Additional information available about a binding, which can be used in constraints
	/// to select bindings to use in activation.
	/// </summary>
	public class BindingMetadata : IBindingMetadata
	{
		private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

		/// <summary>
		/// Gets or sets the binding's name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Determines whether a piece of metadata with the specified key has been defined.
		/// </summary>
		/// <param name="key">The metadata key.</param>
		/// <returns><c>True</c> if such a piece of metadata exists; otherwise, <c>false</c>.</returns>
		public bool Has(string key)
		{
			return _values.ContainsKey(key);
		}

		/// <summary>
		/// Gets the value of metadata defined with the specified key, cast to the specified type.
		/// </summary>
		/// <typeparam name="T">The type of value to expect.</typeparam>
		/// <param name="key">The metadata key.</param>
		/// <returns>The metadata value.</returns>
		public T Get<T>(string key)
		{
			return (T)Get(key);
		}

		/// <summary>
		/// Gets the value of metadata defined with the specified key.
		/// </summary>
		/// <param name="key">The metadata key.</param>
		/// <returns>The metadata value.</returns>
		public object Get(string key)
		{
			return _values.ContainsKey(key) ? _values[key] : null;
		}

		/// <summary>
		/// Sets the value of a piece of metadata.
		/// </summary>
		/// <param name="key">The metadata key.</param>
		/// <param name="value">The metadata value.</param>
		public void Set(string key, object value)
		{
			_values[key] = value;
		}
	}
}