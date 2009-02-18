#region License
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Disposal;
using Ninject.Infrastructure.Introspection;
using Ninject.Infrastructure.Language;
#endregion

namespace Ninject.Components
{
	/// <summary>
	/// An internal container that manages and resolves components that contribute to Ninject.
	/// </summary>
	public class ComponentContainer : DisposableObject, IComponentContainer
	{
		private readonly Multimap<Type, Type> _mappings = new Multimap<Type, Type>();
		private readonly Dictionary<Type, INinjectComponent> _instances = new Dictionary<Type, INinjectComponent>();

		/// <summary>
		/// Gets or sets the kernel that owns the component container.
		/// </summary>
		public IKernel Kernel { get; set; }

		/// <summary>
		/// Releases resources held by the object.
		/// </summary>
		public override void Dispose()
		{
			foreach (INinjectComponent instance in _instances.Values)
				instance.Dispose();

			_mappings.Clear();
			_instances.Clear();

			base.Dispose();
		}

		/// <summary>
		/// Registers a component in the container.
		/// </summary>
		/// <typeparam name="TComponent">The component type.</typeparam>
		/// <typeparam name="TImplementation">The component's implementation type.</typeparam>
		public void Add<TComponent, TImplementation>()
			where TComponent : INinjectComponent
			where TImplementation : TComponent, INinjectComponent
		{
			_mappings.Add(typeof(TComponent), typeof(TImplementation));
		}

		/// <summary>
		/// Removes all registrations for the specified component.
		/// </summary>
		/// <typeparam name="T">The component type.</typeparam>
		public void RemoveAll<T>()
			where T : INinjectComponent
		{
			RemoveAll(typeof(T));
		}

		/// <summary>
		/// Removes all registrations for the specified component.
		/// </summary>
		/// <param name="component">The component type.</param>
		public void RemoveAll(Type component)
		{
			foreach (Type implementation in _mappings[component])
			{
				if (_instances.ContainsKey(implementation))
					_instances[implementation].Dispose();

				_instances.Remove(implementation);
			}

			_mappings.RemoveAll(component);
		}

		/// <summary>
		/// Gets one instance of the specified component.
		/// </summary>
		/// <typeparam name="T">The component type.</typeparam>
		/// <returns>The instance of the component.</returns>
		public T Get<T>()
			where T : INinjectComponent
		{
			return (T) Get(typeof(T));
		}

		/// <summary>
		/// Gets all available instances of the specified component.
		/// </summary>
		/// <typeparam name="T">The component type.</typeparam>
		/// <returns>A series of instances of the specified component.</returns>
		public IEnumerable<T> GetAll<T>()
			where T : INinjectComponent
		{
			return GetAll(typeof(T)).Cast<T>();
		}

		/// <summary>
		/// Gets one instance of the specified component.
		/// </summary>
		/// <param name="component">The component type.</param>
		/// <returns>The instance of the component.</returns>
		public object Get(Type component)
		{
			if (component == typeof(IKernel))
				return Kernel;

			if (component.IsGenericType)
			{
				Type gtd = component.GetGenericTypeDefinition();
				Type argument = component.GetGenericArguments()[0];

				if (gtd.IsInterface && typeof(IEnumerable<>).IsAssignableFrom(gtd))
					return LinqReflection.CastSlow(GetAll(argument), argument);
			}

			Type implementation = _mappings[component].FirstOrDefault();

			if (implementation == null)
				throw new InvalidOperationException(ExceptionFormatter.NoSuchComponentRegistered(component));

			return ResolveInstance(component, implementation);
		}

		/// <summary>
		/// Gets all available instances of the specified component.
		/// </summary>
		/// <param name="component">The component type.</param>
		/// <returns>A series of instances of the specified component.</returns>
		public IEnumerable<object> GetAll(Type component)
		{
			foreach (Type implementation in _mappings[component])
				yield return ResolveInstance(component, implementation);
		}

		private object ResolveInstance(Type component, Type implementation)
		{
			return _instances.ContainsKey(implementation) ? _instances[implementation] : CreateNewInstance(component, implementation);
		}

		private object CreateNewInstance(Type component, Type implementation)
		{
			ConstructorInfo constructor = SelectConstructor(component, implementation);
			var arguments = constructor.GetParameters().Select(parameter => Get(parameter.ParameterType)).ToArray();

			try
			{
				var instance = constructor.Invoke(arguments) as INinjectComponent;
				instance.Settings = Kernel.Settings;
				_instances.Add(implementation, instance);

				return instance;
			}
			catch (TargetInvocationException ex)
			{
				ex.RethrowInnerException();
				return null;
			}
		}

		private ConstructorInfo SelectConstructor(Type component, Type implementation)
		{
			var constructor = implementation.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();

			if (constructor == null)
				throw new InvalidOperationException(ExceptionFormatter.NoConstructorsAvailableForComponent(component, implementation));

			return constructor;
		}
	}
}