using System;
using System.Reflection;
using Ninject.Components;

namespace Ninject.Injection
{
	/// <summary>
	/// Creates injectors from members.
	/// </summary>
	public interface IInjectorFactory : INinjectComponent
	{
		/// <summary>
		/// Gets or creates an injector for the specified constructor.
		/// </summary>
		/// <param name="constructor">The constructor.</param>
		/// <returns>The created injector.</returns>
		IConstructorInjector GetConstructorInjector(ConstructorInfo constructor);

		/// <summary>
		/// Gets or creates an injector for the specified property.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns>The created injector.</returns>
		IPropertyInjector GetPropertyInjector(PropertyInfo property);

		/// <summary>
		/// Gets or creates an injector for the specified method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>The created injector.</returns>
		IMethodInjector GetMethodInjector(MethodInfo method);
	}
}