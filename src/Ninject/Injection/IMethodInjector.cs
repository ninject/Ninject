using System;

namespace Ninject.Injection
{
	/// <summary>
	/// Injects values into a method.
	/// </summary>
	public interface IMethodInjector
	{
		/// <summary>
		/// Calls the associated method, injecting the specified values.
		/// </summary>
		/// <param name="target">The target object on which to call the method.</param>
		/// <param name="values">The values to inject.</param>
		/// <returns>The return value of the method, or <see langword="null"/> if the method returns <see type="void"/>.</returns>
		object Invoke(object target, params object[] values);
	}
}