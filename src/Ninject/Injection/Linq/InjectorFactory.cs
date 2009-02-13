using System;
using System.Collections.Generic;
using System.Reflection;
using Ninject.Components;
using Ninject.Infrastructure.Language;

namespace Ninject.Injection.Linq
{
	/// <summary>
	/// Creates expression-based injectors from members.
	/// </summary>
	public class InjectorFactory : NinjectComponent, IInjectorFactory
	{
		private readonly Dictionary<ConstructorInfo, IConstructorInjector> _constructorInjectors = new Dictionary<ConstructorInfo, IConstructorInjector>();
		private readonly Dictionary<MethodInfo, IMethodInjector> _methodInjectors = new Dictionary<MethodInfo, IMethodInjector>();
		private readonly Dictionary<PropertyInfo, IPropertyInjector> _propertyInjectors = new Dictionary<PropertyInfo, IPropertyInjector>();

		/// <summary>
		/// Gets or creates an injector for the specified constructor.
		/// </summary>
		/// <param name="constructor">The constructor.</param>
		/// <returns>The created injector.</returns>
		public IConstructorInjector GetConstructorInjector(ConstructorInfo constructor)
		{
			return _constructorInjectors.GetOrAddNew(constructor, c => new ConstructorInjector(c));
		}

		/// <summary>
		/// Gets or creates an injector for the specified method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>The created injector.</returns>
		public IMethodInjector GetMethodInjector(MethodInfo method)
		{
			return _methodInjectors.GetOrAddNew(method, m => method.ReturnType == typeof(void) ? (IMethodInjector)new VoidMethodInjector(m) : (IMethodInjector)new MethodInjector(m));
		}

		/// <summary>
		/// Gets or creates an injector for the specified property.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns>The created injector.</returns>
		public IPropertyInjector GetPropertyInjector(PropertyInfo property)
		{
			return _propertyInjectors.GetOrAddNew(property, p => new PropertyInjector(p));
		}
	}
}