using System;

namespace Ninject.Injection
{
	/// <summary>
	/// Injects values into a property.
	/// </summary>
	public interface IPropertyInjector
	{
		/// <summary>
		/// Injects the specified value into the property.
		/// </summary>
		/// <param name="target">The target object to inject.</param>
		/// <param name="value">The value to inject.</param>
		void Invoke(object target, object value);
	}
}