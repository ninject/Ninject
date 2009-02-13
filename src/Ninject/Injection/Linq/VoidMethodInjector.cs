using System;
using System.Reflection;

namespace Ninject.Injection.Linq
{
	/// <summary>
	/// A method injector that injects methods that return <see type="void"/>.
	/// </summary>
	public class VoidMethodInjector : MethodInjectorBase<Action<object, object[]>>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VoidMethodInjector"/> class.
		/// </summary>
		/// <param name="method">The method that will be injected.</param>
		public VoidMethodInjector(MethodInfo method) : base(method) { }

		/// <summary>
		/// Calls the associated method, injecting the specified values.
		/// </summary>
		/// <param name="target">The target object on which to call the method.</param>
		/// <param name="values">The values to inject.</param>
		/// <returns>The return value of the method, or <see langword="null"/> if the method returns <see langword="void"/>.</returns>
		public override object Invoke(object target, object[] values)
		{
			Callback.Invoke(target, values);
			return null;
		}
	}
}