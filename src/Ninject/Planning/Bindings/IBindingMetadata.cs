using System;

namespace Ninject.Planning.Bindings
{
	/// <summary>
	/// Additional information available about a binding, which can be used in constraints
	/// to select bindings to use in activation.
	/// </summary>
	public interface IBindingMetadata
	{
		/// <summary>
		/// Gets or sets the binding's name.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Determines whether a piece of metadata with the specified key has been defined.
		/// </summary>
		/// <param name="key">The metadata key.</param>
		/// <returns><c>True</c> if such a piece of metadata exists; otherwise, <c>false</c>.</returns>
		bool Has(string key);

		/// <summary>
		/// Gets the value of metadata defined with the specified key, cast to the specified type.
		/// </summary>
		/// <typeparam name="T">The type of value to expect.</typeparam>
		/// <param name="key">The metadata key.</param>
		/// <returns>The metadata value.</returns>
		T Get<T>(string key);

		/// <summary>
		/// Gets the value of metadata defined with the specified key.
		/// </summary>
		/// <param name="key">The metadata key.</param>
		/// <returns>The metadata value.</returns>
		object Get(string key);

		/// <summary>
		/// Sets the value of a piece of metadata.
		/// </summary>
		/// <param name="key">The metadata key.</param>
		/// <param name="value">The metadata value.</param>
		void Set(string key, object value);
	}
}