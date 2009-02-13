using System;
using System.Reflection;

namespace Ninject.Injection.Linq
{
	/// <summary>
	/// An injector that injects values into methods.
	/// </summary>
	public class MethodInjector : MethodInjectorBase<Func<object, object[], object>>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MethodInjector"/> class.
		/// </summary>
		/// <param name="method">The method that will be injected.</param>
		public MethodInjector(MethodInfo method) : base(method) { }

		/// <summary>
		/// Calls the associated method, injecting the specified values.
		/// </summary>
		/// <param name="target">The target object on which to call the method.</param>
		/// <param name="values">The values to inject.</param>
		/// <returns>The return value of the method, or <see langword="null"/> if the method returns <see type="void"/>.</returns>
		public override object Invoke(object target, object[] values)
		{
			return Callback.Invoke(target, values);
		}
	}
}