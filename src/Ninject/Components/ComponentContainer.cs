using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Disposal;
using Ninject.Syntax;

namespace Ninject.Components
{
	public class ComponentContainer : DisposableObject, IComponentContainer
	{
		private readonly Multimap<Type, Type> _mappings = new Multimap<Type, Type>();
		private readonly Dictionary<Type, INinjectComponent> _instances = new Dictionary<Type, INinjectComponent>();

		public INinjectSettings Settings { get; set; }

		public override void Dispose()
		{
			foreach (INinjectComponent instance in _instances.Values)
				instance.Dispose();

			_mappings.Clear();
			_instances.Clear();

			base.Dispose();
		}

		public void Add<TService, TImplementation>()
			where TService : INinjectComponent
			where TImplementation : TService, INinjectComponent
		{
			_mappings.Add(typeof(TService), typeof(TImplementation));
		}

		public void RemoveAll<T>()
			where T : INinjectComponent
		{
			RemoveAll(typeof(T));
		}

		public void RemoveAll(Type service)
		{
			foreach (Type implementation in _mappings[service])
			{
				if (_instances.ContainsKey(implementation))
					_instances[implementation].Dispose();

				_instances.Remove(implementation);
			}

			_mappings.RemoveAll(service);
		}

		public T Get<T>()
			where T : INinjectComponent
		{
			return (T) Get(typeof(T));
		}

		public IEnumerable<T> GetAll<T>()
			where T : INinjectComponent
		{
			return GetAll(typeof(T)).Cast<T>();
		}

		public object Get(Type service)
		{
			return GetAll(service).FirstOrDefault();
		}

		public IEnumerable<object> GetAll(Type service)
		{
			foreach (Type implementation in _mappings[service])
				yield return ResolveInstance(implementation);
		}

		private object ResolveInstance(Type type)
		{
			return _instances.ContainsKey(type) ? _instances[type] : CreateNewInstance(type);
		}

		private object CreateNewInstance(Type type)
		{
			var component = FormatterServices.GetSafeUninitializedObject(type) as INinjectComponent;
			_instances.Add(type, component);

			ConstructorInfo constructor = SelectConstructor(type);
			var arguments = constructor.GetParameters().Select(parameter => GetValueForParameter(parameter)).ToArray();

			try
			{
				constructor.Invoke(component, arguments);
				component.Settings = Settings;
				return component;
			}
			catch (TargetInvocationException ex)
			{
				ex.RethrowInnerException();
				return null;
			}
		}

		private object GetValueForParameter(ParameterInfo parameter)
		{
			Type service = parameter.ParameterType;

			if (service.IsArray)
			{
				Type element = service.GetElementType();
				return LinqReflection.ToArraySlow(GetAllSlow(element), element);
			}

			if (service.IsGenericType)
			{
				Type gtd = service.GetGenericTypeDefinition();
				Type argument = service.GetGenericArguments()[0];

				if (typeof(List<>).IsAssignableFrom(gtd))
					return LinqReflection.ToListSlow(GetAllSlow(argument), argument);

				if (gtd.IsInterface && typeof(ICollection<>).IsAssignableFrom(gtd))
					return LinqReflection.ToListSlow(GetAllSlow(argument), argument);

				if (gtd.IsInterface && typeof(IEnumerable<>).IsAssignableFrom(gtd))
					return GetAllSlow(argument);
			}

			return Get(service);
		}

		private ConstructorInfo SelectConstructor(Type type)
		{
			return type.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
		}

		private IEnumerable GetAllSlow(Type service)
		{
			var method = GetType().GetMethod("GetAll", Type.EmptyTypes).MakeGenericMethod(service);
			return method.Invoke(this, null) as IEnumerable;
		}
	}
}