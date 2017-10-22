// -------------------------------------------------------------------------------------------------
// <copyright file="ComponentContainer.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2017 Ninject Project Contributors. All rights reserved.
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   You may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Disposal;
    using Ninject.Infrastructure.Introspection;
    using Ninject.Infrastructure.Language;

    /// <summary>
    /// An internal container that manages and resolves components that contribute to Ninject.
    /// </summary>
    public class ComponentContainer : DisposableObject, IComponentContainer
    {
        private readonly Multimap<Type, Type> mappings = new Multimap<Type, Type>();
        private readonly Dictionary<Type, INinjectComponent> instances = new Dictionary<Type, INinjectComponent>();
        private readonly HashSet<KeyValuePair<Type, Type>> transients = new HashSet<KeyValuePair<Type, Type>>();

        /// <summary>
        /// Gets or sets the kernel that owns the component container.
        /// </summary>
        public IKernel Kernel { get; set; }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        /// <param name="disposing"><c>True</c> if called manually, otherwise by GC.</param>
        public override void Dispose(bool disposing)
        {
            if (disposing && !this.IsDisposed)
            {
                foreach (INinjectComponent instance in this.instances.Values)
                {
                    instance.Dispose();
                }

                this.mappings.Clear();
                this.instances.Clear();
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
            this.mappings.Add(typeof(TComponent), typeof(TImplementation));
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
            this.RemoveAll(typeof(T));
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
            if (this.instances.ContainsKey(implementation))
            {
                this.instances[implementation].Dispose();
            }

            this.instances.Remove(implementation);

            this.mappings.Remove(typeof(T), typeof(TImplementation));
        }

        /// <summary>
        /// Removes all registrations for the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        public void RemoveAll(Type component)
        {
            Ensure.ArgumentNotNull(component, "component");

            foreach (Type implementation in this.mappings[component])
            {
                if (this.instances.ContainsKey(implementation))
                {
                    this.instances[implementation].Dispose();
                }

                this.instances.Remove(implementation);
            }

            this.mappings.RemoveAll(component);
        }

        /// <summary>
        /// Gets one instance of the specified component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>The instance of the component.</returns>
        public T Get<T>()
            where T : INinjectComponent
        {
            return (T)this.Get(typeof(T));
        }

        /// <summary>
        /// Gets all available instances of the specified component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>A series of instances of the specified component.</returns>
        public IEnumerable<T> GetAll<T>()
            where T : INinjectComponent
        {
            return this.GetAll(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Gets one instance of the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        /// <returns>The instance of the component.</returns>
        public object Get(Type component)
        {
            Ensure.ArgumentNotNull(component, "component");

            if (component == typeof(IKernel))
            {
                return this.Kernel;
            }

            if (component.IsGenericType)
            {
                var gtd = component.GetGenericTypeDefinition();
                var argument = component.GenericTypeArguments[0];

                if (gtd.IsInterface && typeof(IEnumerable<>).IsAssignableFrom(gtd))
                {
                    return this.GetAll(argument).CastSlow(argument);
                }
            }

            var implementation = this.mappings[component].FirstOrDefault();

            if (implementation == null)
            {
                throw new InvalidOperationException(ExceptionFormatter.NoSuchComponentRegistered(component));
            }

            return this.ResolveInstance(component, implementation);
        }

        /// <summary>
        /// Gets all available instances of the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        /// <returns>A series of instances of the specified component.</returns>
        public IEnumerable<object> GetAll(Type component)
        {
            Ensure.ArgumentNotNull(component, "component");

            return this.mappings[component]
                .Select(implementation => this.ResolveInstance(component, implementation));
        }

        private static ConstructorInfo SelectConstructor(Type component, Type implementation)
        {
            var constructor = implementation.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();

            if (constructor == null)
            {
                throw new InvalidOperationException(ExceptionFormatter.NoConstructorsAvailableForComponent(component, implementation));
            }

            return constructor;
        }

        private object ResolveInstance(Type component, Type implementation)
        {
            lock (this.instances)
            {
                return this.instances.ContainsKey(implementation) ? this.instances[implementation] : this.CreateNewInstance(component, implementation);
            }
        }

        private object CreateNewInstance(Type component, Type implementation)
        {
            var constructor = SelectConstructor(component, implementation);
            var arguments = constructor.GetParameters().Select(parameter => this.Get(parameter.ParameterType)).ToArray();

            try
            {
                var instance = constructor.Invoke(arguments) as INinjectComponent;

                instance.Settings = this.Kernel.Settings;

                if (!this.transients.Contains(new KeyValuePair<Type, Type>(component, implementation)))
                {
                    this.instances.Add(implementation, instance);
                }

                return instance;
            }
            catch (TargetInvocationException ex)
            {
                ex.RethrowInnerException();
                return null;
            }
        }
    }
}