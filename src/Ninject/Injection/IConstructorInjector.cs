using System;

namespace Ninject.Injection
{
	/// <summary>
	/// Injects values into a constructor.
	/// </summary>
	public interface IConstructorInjector
	{
		/// <summary>
		/// Calls the associated constructor, injecting the specified values.
		/// </summary>
		/// <param name="values">The values to inject.</param>
		/// <returns>The object created by the constructor.</returns>
		object Invoke(params object[] values);
	}
}