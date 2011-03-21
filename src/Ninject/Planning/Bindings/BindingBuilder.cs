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
using Ninject.Activation;
using Ninject.Activation.Providers;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Introspection;
using Ninject.Infrastructure.Language;
using Ninject.Parameters;
using Ninject.Syntax;
#endregion

namespace Ninject.Planning.Bindings
{
    using Ninject.Planning.Targets;

    /// <summary>
    /// Provides a root for the fluent syntax associated with an <see cref="Binding"/>.
    /// </summary>
    public class BindingBuilder<T> : IBindingToSyntax<T>, IBindingWhenInNamedWithOrOnSyntax<T>, IBindingInNamedWithOrOnSyntax<T>, IBindingNamedWithOrOnSyntax<T>, IBindingWithOrOnSyntax<T>
    {
        /// <summary>
        /// Gets the binding being built.
        /// </summary>
        public IBinding Binding { get; private set; }

        /// <summary>
        /// Gets the kernel.
        /// </summary>
        public IKernel Kernel { get; private set; }

        /// <summary>
        /// Initializes a new instance of the BindingBuilder&lt;T&gt; class.
        /// </summary>
        /// <param name="binding">The binding to build.</param>
        /// <param name="kernel">The kernel.</param>
        public BindingBuilder(IBinding binding, IKernel kernel)
        {
            Ensure.ArgumentNotNull(binding, "binding");
            Ensure.ArgumentNotNull(kernel, "kernel");
            Binding = binding;
            Kernel = kernel;
        }

        /// <summary>
        /// Indicates that the service should be self-bound.
        /// </summary>
        public IBindingWhenInNamedWithOrOnSyntax<T> ToSelf()
        {
            Binding.ProviderCallback = StandardProvider.GetCreationCallback(Binding.Service);
            Binding.Target = BindingTarget.Self;

            return this;
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        public IBindingWhenInNamedWithOrOnSyntax<T> To<TImplementation>()
            where TImplementation : T
        {
            Binding.ProviderCallback = StandardProvider.GetCreationCallback(typeof(TImplementation));
            Binding.Target = BindingTarget.Type;

            return this;
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <param name="implementation">The implementation type.</param>
        public IBindingWhenInNamedWithOrOnSyntax<T> To(Type implementation)
        {
            Binding.ProviderCallback = StandardProvider.GetCreationCallback(implementation);
            Binding.Target = BindingTarget.Type;

            return this;
        }

        /// <summary>
        /// Indicates that the service should be bound to an instance of the specified provider type.
        /// The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <typeparam name="TProvider">The type of provider to activate.</typeparam>
        public IBindingWhenInNamedWithOrOnSyntax<T> ToProvider<TProvider>()
            where TProvider : IProvider
        {
            Binding.ProviderCallback = ctx => ctx.Kernel.Get<TProvider>();
            Binding.Target = BindingTarget.Provider;

            return this;
        }

        /// <summary>
        /// Indicates that the service should be bound to an instance of the specified provider type.
        /// The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <param name="providerType">The type of provider to activate.</param>
        public IBindingWhenInNamedWithOrOnSyntax<T> ToProvider(Type providerType)
        {
            Binding.ProviderCallback = ctx => ctx.Kernel.Get(providerType) as IProvider;
            Binding.Target = BindingTarget.Provider;

            return this;
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public IBindingWhenInNamedWithOrOnSyntax<T> ToProvider(IProvider provider)
        {
            Binding.ProviderCallback = ctx => provider;
            Binding.Target = BindingTarget.Provider;

            return this;
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified callback method.
        /// </summary>
        /// <param name="method">The method.</param>
        public IBindingWhenInNamedWithOrOnSyntax<T> ToMethod(Func<IContext, T> method)
        {
            Binding.ProviderCallback = ctx => new CallbackProvider<T>(method);
            Binding.Target = BindingTarget.Method;

            return this;
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified constant value.
        /// </summary>
        /// <param name="value">The constant value.</param>
        public IBindingWhenInNamedWithOrOnSyntax<T> ToConstant(T value)
        {
            Binding.ProviderCallback = ctx => new ConstantProvider<T>(value);
            Binding.Target = BindingTarget.Constant;
            Binding.ScopeCallback = StandardScopeCallbacks.Singleton;

            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only for requests that support the specified condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        public IBindingInNamedWithOrOnSyntax<T> When(Func<IRequest, bool> condition)
        {
            Binding.Condition = condition;
            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        public IBindingInNamedWithOrOnSyntax<T> WhenInjectedInto<TParent>()
        {
            return WhenInjectedInto(typeof(TParent));
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// </summary>
        /// <param name="parent">The type.</param>
        public IBindingInNamedWithOrOnSyntax<T> WhenInjectedInto(Type parent)
        {
            Binding.Condition = r => r.Target != null && r.Target.Member.ReflectedType == parent;
            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        public IBindingInNamedWithOrOnSyntax<T> WhenClassHas<TAttribute>() where TAttribute : Attribute
        {
            return WhenClassHas(typeof(TAttribute));
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        public IBindingInNamedWithOrOnSyntax<T> WhenMemberHas<TAttribute>() where TAttribute : Attribute
        {
            return WhenMemberHas(typeof(TAttribute));
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        public IBindingInNamedWithOrOnSyntax<T> WhenTargetHas<TAttribute>() where TAttribute : Attribute
        {
            return WhenTargetHas(typeof(TAttribute));
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        public IBindingInNamedWithOrOnSyntax<T> WhenClassHas(Type attributeType)
        {
            if (!typeof(Attribute).IsAssignableFrom(attributeType))
                throw new InvalidOperationException(ExceptionFormatter.InvalidAttributeTypeUsedInBindingCondition(Binding, "WhenClassHas", attributeType));

            Binding.Condition = r => r.Target != null && r.Target.Member.ReflectedType.HasAttribute(attributeType);

            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        public IBindingInNamedWithOrOnSyntax<T> WhenMemberHas(Type attributeType)
        {
            if (!typeof(Attribute).IsAssignableFrom(attributeType))
                throw new InvalidOperationException(ExceptionFormatter.InvalidAttributeTypeUsedInBindingCondition(Binding, "WhenMemberHas", attributeType));

            Binding.Condition = r => r.Target != null && r.Target.Member.HasAttribute(attributeType);

            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        public IBindingInNamedWithOrOnSyntax<T> WhenTargetHas(Type attributeType)
        {
            if (!typeof(Attribute).IsAssignableFrom(attributeType))
                throw new InvalidOperationException(ExceptionFormatter.InvalidAttributeTypeUsedInBindingCondition(Binding, "WhenTargetHas", attributeType));

            this.Binding.Condition = r => r.Target != null && r.Target.HasAttribute(attributeType);

            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only when the service is being requested
        /// by a service bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        public IBindingInNamedWithOrOnSyntax<T> WhenParentNamed(string name)
        {
            String.Intern(name);
            Binding.Condition = r => r.ParentContext != null && string.Equals(r.ParentContext.Binding.Metadata.Name, name, StringComparison.Ordinal);
            return this;
        }

        /// <summary>
        /// Indicates that the binding should be registered with the specified name. Names are not
        /// necessarily unique; multiple bindings for a given service may be registered with the same name.
        /// </summary>
        /// <param name="name">The name to give the binding.</param>
        public IBindingWithOrOnSyntax<T> Named(string name)
        {
            String.Intern(name);
            Binding.Metadata.Name = name;
            return this;
        }

        /// <summary>
        /// Indicates that only a single instance of the binding should be created, and then
        /// should be re-used for all subsequent requests.
        /// </summary>
        public IBindingNamedWithOrOnSyntax<T> InSingletonScope()
        {
            Binding.ScopeCallback = StandardScopeCallbacks.Singleton;
            return this;
        }

        /// <summary>
        /// Indicates that instances activated via the binding should not be re-used, nor have
        /// their lifecycle managed by Ninject.
        /// </summary>
        public IBindingNamedWithOrOnSyntax<T> InTransientScope()
        {
            Binding.ScopeCallback = StandardScopeCallbacks.Transient;
            return this;
        }

        /// <summary>
        /// Indicates that instances activated via the binding should be re-used within the same thread.
        /// </summary>
        public IBindingNamedWithOrOnSyntax<T> InThreadScope()
        {
            Binding.ScopeCallback = StandardScopeCallbacks.Thread;
            return this;
        }

        /// <summary>
        /// Indicates that instances activated via the binding should be re-used as long as the object
        /// returned by the provided callback remains alive (that is, has not been garbage collected).
        /// </summary>
        /// <param name="scope">The callback that returns the scope.</param>
        public IBindingNamedWithOrOnSyntax<T> InScope(Func<IContext, object> scope)
        {
            Binding.ScopeCallback = scope;
            return this;
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="value">The value for the argument.</param>
        public IBindingWithOrOnSyntax<T> WithConstructorArgument(string name, object value)
        {
            Binding.Parameters.Add(new ConstructorArgument(name, value));
            return this;
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        public IBindingWithOrOnSyntax<T> WithConstructorArgument(string name, Func<IContext, object> callback)
        {
            Binding.Parameters.Add(new ConstructorArgument(name, callback));
            return this;
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        public IBindingWithOrOnSyntax<T> WithConstructorArgument(string name, Func<IContext, ITarget, object> callback)
        {
            Binding.Parameters.Add(new ConstructorArgument(name, callback));
            return this;
        }
        
        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="value">The value for the property.</param>
        public IBindingWithOrOnSyntax<T> WithPropertyValue(string name, object value)
        {
            Binding.Parameters.Add(new PropertyValue(name, value));
            return this;
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        public IBindingWithOrOnSyntax<T> WithPropertyValue(string name, Func<IContext, object> callback)
        {
            Binding.Parameters.Add(new PropertyValue(name, callback));
            return this;
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        public IBindingWithOrOnSyntax<T> WithPropertyValue(string name, Func<IContext, ITarget, object> callback)
        {
            Binding.Parameters.Add(new PropertyValue(name, callback));
            return this;
        }
        
        /// <summary>
        /// Adds a custom parameter to the binding.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public IBindingWithOrOnSyntax<T> WithParameter(IParameter parameter)
        {
            Binding.Parameters.Add(parameter);
            return this;
        }

        /// <summary>
        /// Sets the value of a piece of metadata on the binding.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        public IBindingWithOrOnSyntax<T> WithMetadata(string key, object value)
        {
            Binding.Metadata.Set(key, value);
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        public IBindingOnSyntax<T> OnActivation(Action<T> action)
        {
            Binding.ActivationActions.Add((context, instance) => action((T)instance));
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        public IBindingOnSyntax<T> OnActivation(Action<IContext, T> action)
        {
            Binding.ActivationActions.Add((context, instance) => action(context, (T)instance));
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        public IBindingOnSyntax<T> OnDeactivation(Action<T> action)
        {
            Binding.DeactivationActions.Add((context, instance) => action((T)instance));
            return this;
        }
        
        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        public IBindingOnSyntax<T> OnDeactivation(Action<IContext, T> action)
        {
            Binding.DeactivationActions.Add((context, instance) => action(context, (T)instance));
            return this;
        }
    }
}