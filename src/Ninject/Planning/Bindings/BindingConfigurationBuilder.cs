//-------------------------------------------------------------------------------
// <copyright file="BindingConfigurationBuilder.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2009, Enkari, Ltd.
//   Copyright (c) 2009-2011 Ninject Project Contributors
//   Authors: Nate Kohari (nate@enkari.com)
//            Remo Gloor (remo.gloor@gmail.com)
//           
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   you may not use this file except in compliance with one of the Licenses.
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
//-------------------------------------------------------------------------------

namespace Ninject.Planning.Bindings
{
    using System;
    using System.Linq;

    using Ninject.Activation;
    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Introspection;
    using Ninject.Infrastructure.Language;
    using Ninject.Parameters;
    using Ninject.Planning.Targets;
    using Ninject.Syntax;

    /// <summary>
    /// Provides a root for the fluent syntax associated with an <see cref="BindingConfiguration"/>.
    /// </summary>
    /// <typeparam name="T">The implementation type of the built binding.</typeparam>
    public class BindingConfigurationBuilder<T> : IBindingConfigurationSyntax<T>
    {
        /// <summary>
        /// The names of the services added to the exceptions.
        /// </summary>
        private readonly string serviceNames;

        /// <summary>
        /// Gets the binding being built.
        /// </summary>
        public IBindingConfiguration BindingConfiguration { get; private set; }

        /// <summary>
        /// Gets the kernel.
        /// </summary>
        public IKernel Kernel { get; private set; }

        /// <summary>
        /// Initializes a new instance of the BindingBuilder&lt;T&gt; class.
        /// </summary>
        /// <param name="bindingConfiguration">The binding configuration to build.</param>
        /// <param name="serviceNames">The names of the configured services.</param>
        /// <param name="kernel">The kernel.</param>
        public BindingConfigurationBuilder(IBindingConfiguration bindingConfiguration, string serviceNames, IKernel kernel)
        {
            Ensure.ArgumentNotNull(bindingConfiguration, "bindingConfiguration");
            Ensure.ArgumentNotNull(kernel, "kernel");
            this.BindingConfiguration = bindingConfiguration;
            this.Kernel = kernel;
            this.serviceNames = serviceNames;
        }

        /// <summary>
        /// Indicates that the binding should be used only for requests that support the specified condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> When(Func<IRequest, bool> condition)
        {
            this.BindingConfiguration.Condition = condition;
            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenInjectedInto<TParent>()
        {
            return WhenInjectedInto(typeof(TParent));
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenInjectedInto(Type parent)
        {
            if (parent.IsGenericTypeDefinition)
            {
                if (parent.IsInterface)
                {
                    this.BindingConfiguration.Condition = r =>
                        r.Target != null &&
                        r.Target.Member.ReflectedType.GetInterfaces().Any(i => 
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == parent);
                }
                else
                {
                    this.BindingConfiguration.Condition = r => 
                        r.Target != null &&
                        r.Target.Member.ReflectedType.GetAllBaseTypes().Any(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == parent);
                }
            }
            else
            {
                this.BindingConfiguration.Condition = r => r.Target != null && parent.IsAssignableFrom(r.Target.Member.ReflectedType);
            }

            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match exactly the specified type. Types that derive from the specified type
        /// will not be considered as valid target.  
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenInjectedExactlyInto<TParent>()
        {
            return WhenInjectedExactlyInto(typeof(TParent));
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match exactly the specified type. Types that derive from the specified type
        /// will not be considered as valid target.  
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenInjectedExactlyInto(Type parent)
        {
            if (parent.IsGenericTypeDefinition)
            {
                this.BindingConfiguration.Condition = r =>
                    r.Target != null &&
                    r.Target.Member.ReflectedType.IsGenericType &&
                    parent == r.Target.Member.ReflectedType.GetGenericTypeDefinition();
            }
            else
            {
                this.BindingConfiguration.Condition = r => r.Target != null && r.Target.Member.ReflectedType == parent;
            }
            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenClassHas<TAttribute>() where TAttribute : Attribute
        {
            return WhenClassHas(typeof(TAttribute));
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenMemberHas<TAttribute>() where TAttribute : Attribute
        {
            return WhenMemberHas(typeof(TAttribute));
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenTargetHas<TAttribute>() where TAttribute : Attribute
        {
            return WhenTargetHas(typeof(TAttribute));
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenClassHas(Type attributeType)
        {
            if (!typeof(Attribute).IsAssignableFrom(attributeType))
            {
                throw new InvalidOperationException(ExceptionFormatter.InvalidAttributeTypeUsedInBindingCondition(this.serviceNames, "WhenClassHas", attributeType));
            }

            this.BindingConfiguration.Condition = r => r.Target != null && r.Target.Member.ReflectedType.HasAttribute(attributeType);

            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenMemberHas(Type attributeType)
        {
            if (!typeof(Attribute).IsAssignableFrom(attributeType))
            {
                throw new InvalidOperationException(ExceptionFormatter.InvalidAttributeTypeUsedInBindingCondition(this.serviceNames, "WhenMemberHas", attributeType));
            }

            this.BindingConfiguration.Condition = r => r.Target != null && r.Target.Member.HasAttribute(attributeType);

            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenTargetHas(Type attributeType)
        {
            if (!typeof(Attribute).IsAssignableFrom(attributeType))
            {
                throw new InvalidOperationException(ExceptionFormatter.InvalidAttributeTypeUsedInBindingCondition(this.serviceNames, "WhenTargetHas", attributeType));                
            }

            this.BindingConfiguration.Condition = r => r.Target != null && r.Target.HasAttribute(attributeType);

            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only when the service is being requested
        /// by a service bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenParentNamed(string name)
        {
            String.Intern(name);
            this.BindingConfiguration.Condition = r => r.ParentContext != null && string.Equals(r.ParentContext.Binding.Metadata.Name, name, StringComparison.Ordinal);
            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        [Obsolete("Use WhenAnyAncestorNamed(string name)")]
        public IBindingInNamedWithOrOnSyntax<T> WhenAnyAnchestorNamed(string name)
        {
            return this.WhenAnyAncestorNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenAnyAncestorNamed(string name)
        {
            return this.WhenAnyAncestorMatches(ctx => ctx.Binding.Metadata.Name == name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenNoAncestorNamed(string name)
        {
            return this.WhenNoAncestorMatches(ctx => ctx.Binding.Metadata.Name == name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            this.BindingConfiguration.Condition = r => DoesAnyAncestorMatch(r, predicate);
            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            this.BindingConfiguration.Condition = r => !DoesAnyAncestorMatch(r, predicate);
            return this;
        }

        /// <summary>
        /// Indicates that the binding should be registered with the specified name. Names are not
        /// necessarily unique; multiple bindings for a given service may be registered with the same name.
        /// </summary>
        /// <param name="name">The name to give the binding.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWithOrOnSyntax<T> Named(string name)
        {
            string.Intern(name);
            this.BindingConfiguration.Metadata.Name = name;
            return this;
        }

        /// <summary>
        /// Indicates that only a single instance of the binding should be created, and then
        /// should be re-used for all subsequent requests.
        /// </summary>
        /// <returns>The fluent syntax.</returns>
        public IBindingNamedWithOrOnSyntax<T> InSingletonScope()
        {
            this.BindingConfiguration.ScopeCallback = StandardScopeCallbacks.Singleton;
            return this;
        }

        /// <summary>
        /// Indicates that instances activated via the binding should not be re-used, nor have
        /// their lifecycle managed by Ninject.
        /// </summary>
        /// <returns>The fluent syntax.</returns>
        public IBindingNamedWithOrOnSyntax<T> InTransientScope()
        {
            this.BindingConfiguration.ScopeCallback = StandardScopeCallbacks.Transient;
            return this;
        }

        /// <summary>
        /// Indicates that instances activated via the binding should be re-used within the same thread.
        /// </summary>
        /// <returns>The fluent syntax.</returns>
        public IBindingNamedWithOrOnSyntax<T> InThreadScope()
        {
            this.BindingConfiguration.ScopeCallback = StandardScopeCallbacks.Thread;
            return this;
        }

        /// <summary>
        /// Indicates that instances activated via the binding should be re-used as long as the object
        /// returned by the provided callback remains alive (that is, has not been garbage collected).
        /// </summary>
        /// <param name="scope">The callback that returns the scope.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingNamedWithOrOnSyntax<T> InScope(Func<IContext, object> scope)
        {
            this.BindingConfiguration.ScopeCallback = scope;
            return this;
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="value">The value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWithOrOnSyntax<T> WithConstructorArgument(string name, object value)
        {
            this.BindingConfiguration.Parameters.Add(new ConstructorArgument(name, value));
            return this;
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWithOrOnSyntax<T> WithConstructorArgument(string name, Func<IContext, object> callback)
        {
            this.BindingConfiguration.Parameters.Add(new ConstructorArgument(name, callback));
            return this;
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWithOrOnSyntax<T> WithConstructorArgument(string name, Func<IContext, ITarget, object> callback)
        {
            this.BindingConfiguration.Parameters.Add(new ConstructorArgument(name, callback));
            return this;
        }
        
        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="value">The value for the property.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWithOrOnSyntax<T> WithPropertyValue(string name, object value)
        {
            this.BindingConfiguration.Parameters.Add(new PropertyValue(name, value));
            return this;
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWithOrOnSyntax<T> WithPropertyValue(string name, Func<IContext, object> callback)
        {
            this.BindingConfiguration.Parameters.Add(new PropertyValue(name, callback));
            return this;
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWithOrOnSyntax<T> WithPropertyValue(string name, Func<IContext, ITarget, object> callback)
        {
            this.BindingConfiguration.Parameters.Add(new PropertyValue(name, callback));
            return this;
        }
        
        /// <summary>
        /// Adds a custom parameter to the binding.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWithOrOnSyntax<T> WithParameter(IParameter parameter)
        {
            this.BindingConfiguration.Parameters.Add(parameter);
            return this;
        }

        /// <summary>
        /// Sets the value of a piece of metadata on the binding.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWithOrOnSyntax<T> WithMetadata(string key, object value)
        {
            this.BindingConfiguration.Metadata.Set(key, value);
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingOnSyntax<T> OnActivation(Action<T> action)
        {
            return this.OnActivation<T>(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingOnSyntax<T> OnActivation<TImplementation>(Action<TImplementation> action)
        {
            this.BindingConfiguration.ActivationActions.Add((context, instance) => action((TImplementation)instance));
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingOnSyntax<T> OnActivation(Action<IContext, T> action)
        {
            return this.OnActivation<T>(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingOnSyntax<T> OnActivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            this.BindingConfiguration.ActivationActions.Add((context, instance) => action(context, (TImplementation)instance));
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingOnSyntax<T> OnDeactivation(Action<T> action)
        {
            return this.OnDeactivation<T>(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingOnSyntax<T> OnDeactivation<TImplementation>(Action<TImplementation> action)
        {
            this.BindingConfiguration.DeactivationActions.Add((context, instance) => action((TImplementation)instance));
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingOnSyntax<T> OnDeactivation(Action<IContext, T> action)
        {
            return this.OnDeactivation<T>(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingOnSyntax<T> OnDeactivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            this.BindingConfiguration.DeactivationActions.Add((context, instance) => action(context, (TImplementation)instance));
            return this;
        }

        private static bool DoesAnyAncestorMatch(IRequest request, Predicate<IContext> predicate)
        {
            var parentContext = request.ParentContext;
            if (parentContext == null)
            {
                return false;
            }

            return
                predicate(parentContext) ||
                DoesAnyAncestorMatch(parentContext.Request, predicate);
        }
    }
}