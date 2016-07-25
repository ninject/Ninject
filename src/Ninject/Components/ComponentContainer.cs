#region License
//
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
//
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
//
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
        private readonly HashSet<KeyValuePair<Type, Type>> transients = new HashSet<KeyValuePair<Type, Type>>();

        /// <summary>
        /// Gets or sets the kernel that owns the component container.
        /// </summary>
        public IKernel Kernel { get; set; }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        /// <param name="disposing">A Boolean indicating whether release managed resource or not.</param>
        public override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                foreach (INinjectComponent instance in _instances.Values)
                    instance.Dispose();

                _mappings.Clear();
                _instances.Clear();
            }

            base.Dispose(disposing);
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
        /// Registers a transient component in the container.
        /// </summary>
        /// <typeparam name="TComponent">The component type.</typeparam>
        /// <typeparam name="TImplementation">The component's implementation type.</typeparam>
        public void AddTransient<TComponent, TImplementation>()
            where TComponent : INinjectComponent
            where TImplementation : TComponent, INinjectComponent
        {
            this.Add<TComponent, TImplementation>();
            this.transients.Add(new KeyValuePair<Type, Type>(typeof(TComponent), typeof(TImplementation)));
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
        /// Removes the specified registration.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        public void Remove<T, TImplementation>()
            where T : INinjectComponent
            where TImplementation : T
        {
            var implementation = typeof(TImplementation);
            if (_instances.ContainsKey(implementation))
                _instances[implementation].Dispose();

            _instances.Remove(implementation);

            _mappings[typeof(T)].Remove(typeof(TImplementation));
        }
        /// <summary>
        /// Removes all registrations for the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        public void RemoveAll(Type component)
        {
            Contract.Requires(component != null);

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
            Contract.Requires(component != null);

            if (component == typeof(IKernel))
                return Kernel;

            if (component.GetTypeInfo().IsGenericType)
            {
                var gtd = component.GetGenericTypeDefinition();
                var argument = component.GetTypeInfo().GetGenericArguments()[0];

                if (gtd.GetTypeInfo().IsInterface && typeof (IEnumerable<>).GetTypeInfo().IsAssignableFrom(gtd))
                    return GetAll(argument).CastSlow(argument);
            }

            var implementation = _mappings[component].FirstOrDefault();

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
            Contract.Requires(component != null);

            return _mappings[component]
                .Select(implementation => ResolveInstance(component, implementation));
        }

        private object ResolveInstance(Type component, Type implementation)
        {
            lock (_instances)
                return _instances.ContainsKey(implementation) ? _instances[implementation] : CreateNewInstance(component, implementation);
        }

        private object CreateNewInstance(Type component, Type implementation)
        {
            var constructor = SelectConstructor(component, implementation);
            var arguments = constructor.GetParameters().Select(parameter => Get(parameter.ParameterType)).ToArray();

            try
            {
                var instance = constructor.Invoke(arguments) as INinjectComponent;
                instance.Settings = Kernel.Settings;

                if (!this.transients.Contains(new KeyValuePair<Type, Type>(component, implementation)))
                {
                    _instances.Add(implementation, instance);
                }

                return instance;
            }
            catch (TargetInvocationException ex)
            {
                ex.RethrowInnerException();
                return null;
            }
        }

        private static ConstructorInfo SelectConstructor(Type component, Type implementation)
        {
            var constructor = implementation.GetTypeInfo().GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();

            if (constructor == null)
                throw new InvalidOperationException(ExceptionFormatter.NoConstructorsAvailableForComponent(component, implementation));

            return constructor;
        }
    }
}