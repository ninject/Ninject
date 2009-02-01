using System;
using System.Collections.Generic;
using System.Reflection;
using Ninject.Injection.Injectors;
using Ninject.Injection.Injectors.Linq;

namespace Ninject.Injection
{
	public class StandardInjectorFactory : IInjectorFactory
	{
		private readonly Dictionary<ConstructorInfo, IConstructorInjector> _constructorInjectors = new Dictionary<ConstructorInfo, IConstructorInjector>();
		private readonly Dictionary<MethodInfo, IMethodInjector> _methodInjectors = new Dictionary<MethodInfo, IMethodInjector>();
		private readonly Dictionary<PropertyInfo, IPropertyInjector> _propertyInjectors = new Dictionary<PropertyInfo, IPropertyInjector>();

		public IConstructorInjector GetConstructorInjector(ConstructorInfo constructor)
		{
			return GetOrCreate(_constructorInjectors, constructor, c => new ConstructorInjector(c));
		}

		public IMethodInjector GetMethodInjector(MethodInfo method)
		{
			return GetOrCreate(_methodInjectors, method, m => method.ReturnType == typeof(void) ? (IMethodInjector)new VoidMethodInjector(m) : (IMethodInjector)new MethodInjector(m));
		}

		public IPropertyInjector GetPropertyInjector(PropertyInfo property)
		{
			return GetOrCreate(_propertyInjectors, property, p => new PropertyInjector(p));
		}

		private static V GetOrCreate<K, V>(IDictionary<K, V> dictionary, K key, Func<K, V> createCallback)
		{
			V value;

			if (dictionary.ContainsKey(key))
			{
				value = dictionary[key];
			}
			else
			{
				value = createCallback(key);
				dictionary.Add(key, value);
			}

			return value;
		}
	}
}