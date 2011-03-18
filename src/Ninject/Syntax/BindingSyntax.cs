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
using Ninject.Infrastructure;
using Ninject.Parameters;
#endregion

namespace Ninject.Syntax
{
    using Ninject.Planning.Targets;

    /// <summary>
    /// Used to define a basic binding syntax builder.
    /// </summary>
    public interface IBindingSyntax : IFluentSyntax, IHaveBinding, IHaveKernel
    {
    }

    /// <summary>
    /// Used to define the target of a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingToSyntax<T> : IBindingSyntax
    {
        /// <summary>
        /// Indicates that the service should be self-bound.
        /// </summary>
        IBindingWhenInNamedWithOrOnSyntax<T> ToSelf();

        /// <summary>
        /// Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        IBindingWhenInNamedWithOrOnSyntax<T> To<TImplementation>() where TImplementation : T;

        /// <summary>
        /// Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <param name="implementation">The implementation type.</param>
        IBindingWhenInNamedWithOrOnSyntax<T> To(Type implementation);

        /// <summary>
        /// Indicates that the service should be bound to an instance of the specified provider type.
        /// The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <typeparam name="TProvider">The type of provider to activate.</typeparam>
        IBindingWhenInNamedWithOrOnSyntax<T> ToProvider<TProvider>() where TProvider : IProvider;

        /// <summary>
        /// Indicates that the service should be bound to an instance of the specified provider type.
        /// The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <param name="providerType">The type of provider to activate.</param>
        IBindingWhenInNamedWithOrOnSyntax<T> ToProvider(Type providerType);

        /// <summary>
        /// Indicates that the service should be bound to the specified provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        IBindingWhenInNamedWithOrOnSyntax<T> ToProvider(IProvider provider);

        /// <summary>
        /// Indicates that the service should be bound to the specified callback method.
        /// </summary>
        /// <param name="method">The method.</param>
        IBindingWhenInNamedWithOrOnSyntax<T> ToMethod(Func<IContext, T> method);

        /// <summary>
        /// Indicates that the service should be bound to the specified constant value.
        /// </summary>
        /// <param name="value">The constant value.</param>
        IBindingWhenInNamedWithOrOnSyntax<T> ToConstant(T value);
    }

    /// <summary>
    /// Used to define the conditions under which a binding should be used.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingWhenSyntax<T> : IBindingSyntax
    {
        /// <summary>
        /// Indicates that the binding should be used only for requests that support the specified condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        IBindingInNamedWithOrOnSyntax<T> When(Func<IRequest, bool> condition);

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        IBindingInNamedWithOrOnSyntax<T> WhenInjectedInto<TParent>();

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// </summary>
        /// <param name="parent">The type.</param>
        IBindingInNamedWithOrOnSyntax<T> WhenInjectedInto(Type parent);

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        IBindingInNamedWithOrOnSyntax<T> WhenClassHas<TAttribute>() where TAttribute : Attribute;

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        IBindingInNamedWithOrOnSyntax<T> WhenMemberHas<TAttribute>() where TAttribute : Attribute;

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        IBindingInNamedWithOrOnSyntax<T> WhenTargetHas<TAttribute>() where TAttribute : Attribute;

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        IBindingInNamedWithOrOnSyntax<T> WhenClassHas(Type attributeType);

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        IBindingInNamedWithOrOnSyntax<T> WhenMemberHas(Type attributeType);

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        IBindingInNamedWithOrOnSyntax<T> WhenTargetHas(Type attributeType);

        /// <summary>
        /// Indicates that the binding should be used only when the service is being requested
        /// by a service bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        IBindingInNamedWithOrOnSyntax<T> WhenParentNamed(string name);
    }

    /// <summary>
    /// Used to define the scope in which instances activated via a binding should be re-used.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingInSyntax<T> : IBindingSyntax
    {
        /// <summary>
        /// Indicates that only a single instance of the binding should be created, and then
        /// should be re-used for all subsequent requests.
        /// </summary>
        IBindingNamedWithOrOnSyntax<T> InSingletonScope();

        /// <summary>
        /// Indicates that instances activated via the binding should not be re-used, nor have
        /// their lifecycle managed by Ninject.
        /// </summary>
        IBindingNamedWithOrOnSyntax<T> InTransientScope();

        /// <summary>
        /// Indicates that instances activated via the binding should be re-used within the same thread.
        /// </summary>
        IBindingNamedWithOrOnSyntax<T> InThreadScope();

        /// <summary>
        /// Indicates that instances activated via the binding should be re-used as long as the object
        /// returned by the provided callback remains alive (that is, has not been garbage collected).
        /// </summary>
        /// <param name="scope">The callback that returns the scope.</param>
        IBindingNamedWithOrOnSyntax<T> InScope(Func<IContext, object> scope);
    }

    /// <summary>
    /// Used to define the name of a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingNamedSyntax<T> : IBindingSyntax
    {
        /// <summary>
        /// Indicates that the binding should be registered with the specified name. Names are not
        /// necessarily unique; multiple bindings for a given service may be registered with the same name.
        /// </summary>
        /// <param name="name">The name to give the binding.</param>
        IBindingWithOrOnSyntax<T> Named(string name);
    }

    /// <summary>
    /// Used to add additional information to a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingWithSyntax<T> : IBindingSyntax
    {
        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="value">The value for the argument.</param>
        IBindingWithOrOnSyntax<T> WithConstructorArgument(string name, object value);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        IBindingWithOrOnSyntax<T> WithConstructorArgument(string name, Func<IContext, object> callback);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        IBindingWithOrOnSyntax<T> WithConstructorArgument(string name, Func<IContext, ITarget, object> callback);

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="value">The value for the property.</param>
        IBindingWithOrOnSyntax<T> WithPropertyValue(string name, object value);

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        IBindingWithOrOnSyntax<T> WithPropertyValue(string name, Func<IContext, object> callback);

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        IBindingWithOrOnSyntax<T> WithPropertyValue(string name, Func<IContext, ITarget, object> callback);

        /// <summary>
        /// Adds a custom parameter to the binding.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        IBindingWithOrOnSyntax<T> WithParameter(IParameter parameter);

        /// <summary>
        /// Sets the value of a piece of metadata on the binding.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        IBindingWithOrOnSyntax<T> WithMetadata(string key, object value);
    }

    /// <summary>
    /// Used to add additional actions to be performed during activation or deactivation of instances via a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingOnSyntax<T> : IBindingSyntax
    {
        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        IBindingOnSyntax<T> OnActivation(Action<T> action);
        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        IBindingOnSyntax<T> OnActivation(Action<IContext, T> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        IBindingOnSyntax<T> OnDeactivation(Action<T> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        IBindingOnSyntax<T> OnDeactivation(Action<IContext, T> action);
    }

    /// <summary>
    /// Used to set the condition, scope, name, or add additional information or actions to a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingWhenInNamedWithOrOnSyntax<T> : IBindingWhenSyntax<T>, IBindingInSyntax<T>, IBindingNamedSyntax<T>, IBindingWithSyntax<T>, IBindingOnSyntax<T> { }

    /// <summary>
    /// Used to set the scope, name, or add additional information or actions to a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingInNamedWithOrOnSyntax<T> : IBindingInSyntax<T>, IBindingNamedSyntax<T>, IBindingWithSyntax<T>, IBindingOnSyntax<T> { }

    /// <summary>
    /// Used to set the name, or add additional information or actions to a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingNamedWithOrOnSyntax<T> : IBindingNamedSyntax<T>, IBindingWithSyntax<T>, IBindingOnSyntax<T> { }

    /// <summary>
    /// Used to add additional information or actions to a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingWithOrOnSyntax<T> : IBindingWithSyntax<T>, IBindingOnSyntax<T> { }
}