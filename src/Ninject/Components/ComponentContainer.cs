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
    using Ninject.Infrastructure.Language;

    /// <summary>
    /// An internal container that manages and resolves components that contribute to Ninject.
    /// </summary>
    public class ComponentContainer : DisposableObject, IComponentContainer
    {
        /// <summary>
        /// The mappings for ninject components.
        /// </summary>
        private readonly Multimap<Type, Type> mappings = new Multimap<Type, Type>();

        /// <summary>
        /// The mappings for ninject components with transient scope.
        /// </summary>
        private readonly HashSet<KeyValuePair<Type, Type>> transients = new HashSet<KeyValuePair<Type, Type>>();

        /// <summary>
        /// The ninject component instances.
        /// </summary>
        private readonly Dictionary<Type, INinjectComponent> instances = new Dictionary<Type, INinjectComponent>();

        /// <summary>
        /// The ninject settings.
        /// </summary>
        private readonly INinjectSettings settings;

        /// <summary>
        /// The <see cref="IExceptionFormatter"/> component.
        /// </summary>
        private readonly IExceptionFormatter exceptionFormatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentContainer"/> class.
        /// </summary>
        public ComponentContainer()
            : this(new NinjectSettings(), new ExceptionFormatter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentContainer"/> class.
        /// </summary>
        /// <param name="settings">The ninject settings.</param>
        public ComponentContainer(INinjectSettings settings)
            : this(settings, new ExceptionFormatter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentContainer"/> class.
        /// </summary>
        /// <param name="settings">The ninject settings.</param>
        /// <param name="exceptionFormatter">The <see cref="IExceptionFormatter"/> component.</param>
        public ComponentContainer(INinjectSettings settings, IExceptionFormatter exceptionFormatter)
        {
            this.settings = settings;
            this.exceptionFormatter = exceptionFormatter;
            this.Add<IExceptionFormatter, IExceptionFormatter>(exceptionFormatter);
        }

        /// <summary>
        /// Gets or sets the kernel configuration that owns the component container.
        /// </summary>
        public IKernelConfiguration KernelConfiguration { get; set; }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if called manually, otherwise by GC.</param>
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

            if (this.instances.TryGetValue(implementation, out var instance))
            {
                instance.Dispose();
                this.instances.Remove(implementation);
            }

            this.mappings.Remove(typeof(T), typeof(TImplementation));
        }

        /// <summary>
        /// Removes all registrations for the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="component"/> is <see langword="null"/>.</exception>
        public void RemoveAll(Type component)
        {
            Ensure.ArgumentNotNull(component, nameof(component));

            foreach (Type implementation in this.mappings[component])
            {
                if (this.instances.TryGetValue(implementation, out var instance))
                {
                    instance.Dispose();
                    this.instances.Remove(implementation);
                }
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
            var component = typeof(T);

            var implementations = this.mappings[component];
            if (implementations.Count == 0)
            {
                throw new InvalidOperationException(this.exceptionFormatter.NoSuchComponentRegistered(component));
            }

            return (T)this.ResolveInstance(component, implementations[0]);
        }

        /// <summary>
        /// Gets all available instances of the specified component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>
        /// A series of instances of the specified component.
        /// </returns>
        public IEnumerable<T> GetAll<T>()
            where T : INinjectComponent
        {
            return this.GetAll(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Gets one instance of the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        /// <returns>
        /// The instance of the component.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="component"/> is <see langword="null"/>.</exception>
        public object Get(Type component)
        {
            Ensure.ArgumentNotNull(component, nameof(component));

            if (component == typeof(IKernelConfiguration))
            {
                return this.KernelConfiguration;
            }

            if (component == typeof(INinjectSettings))
            {
                return this.settings;
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

            var implementations = this.mappings[component];
            if (implementations.Count == 0)
            {
                throw new InvalidOperationException(this.exceptionFormatter.NoSuchComponentRegistered(component));
            }

            return this.ResolveInstance(component, implementations[0]);
        }

        /// <summary>
        /// Gets all available instances of the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        /// <returns>
        /// A series of instances of the specified component.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="component"/> is <see langword="null"/>.</exception>
        public IEnumerable<object> GetAll(Type component)
        {
            Ensure.ArgumentNotNull(component, nameof(component));

            return this.mappings[component]
                .Select(implementation => this.ResolveInstance(component, implementation));
        }

        private static ConstructorInfo SelectConstructor(Type component, Type implementation)
        {
            return implementation.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
        }

        private object ResolveInstance(Type component, Type implementation)
        {
            lock (this.instances)
            {
                if (this.instances.TryGetValue(implementation, out var instance))
                {
                    return instance;
                }

                return this.CreateNewInstance(component, implementation);
            }
        }

        private object CreateNewInstance(Type component, Type implementation)
        {
            var constructor = SelectConstructor(component, implementation);
            if (constructor == null)
            {
                throw new InvalidOperationException(this.exceptionFormatter.NoConstructorsAvailableForComponent(component, implementation));
            }

            var arguments = this.GetConstructorArguments(constructor.GetParameters());

            try
            {
                var instance = constructor.Invoke(arguments) as INinjectComponent;

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

        private object[] GetConstructorArguments(ParameterInfo[] parameters)
        {
            if (parameters.Length == 0)
            {
                return Array.Empty<object>();
            }

            var arguments = new object[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                arguments[i] = this.Get(parameters[i].ParameterType);
            }

            return arguments;
        }

        /// <summary>
        /// Registers an instance of a component in the container.
        /// </summary>
        /// <typeparam name="TComponent">The component type.</typeparam>
        /// <typeparam name="TImplementation">The component's implementation type.</typeparam>
        /// <param name="instance">THe instance of <typeparamref name="TImplementation"/> to register.</param>
        private void Add<TComponent, TImplementation>(TImplementation instance)
            where TComponent : INinjectComponent
            where TImplementation : TComponent, INinjectComponent
        {
            this.mappings.Add(typeof(TComponent), typeof(TImplementation));
            this.instances.Add(typeof(TImplementation), instance);
        }
    }
}