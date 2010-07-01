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
using Ninject.Activation;
using Ninject.Parameters;
#endregion

namespace Ninject.Planning.Bindings
{
	/// <summary>
	/// Contains information about a service registration.
	/// </summary>
	public interface IBinding
	{
		/// <summary>
		/// Gets the service type that is controlled by the binding.
		/// </summary>
		Type Service { get; }

		/// <summary>
		/// Gets the binding's metadata.
		/// </summary>
		IBindingMetadata Metadata { get; }

		/// <summary>
		/// Gets or sets the type of target for the binding.
		/// </summary>
		BindingTarget Target { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the binding was implicitly registered.
		/// </summary>
		bool IsImplicit { get; set; }

		/// <summary>
		/// Gets a value indicating whether the binding has a condition associated with it.
		/// </summary>
		bool IsConditional { get; }

		/// <summary>
		/// Gets or sets the condition defined for the binding.
		/// </summary>
		Func<IRequest, bool> Condition { get; set; }

		/// <summary>
		/// Gets or sets the callback that returns the provider that should be used by the binding.
		/// </summary>
		Func<IContext, IProvider> ProviderCallback { get; set; }

		/// <summary>
		/// Gets or sets the callback that returns the object that will act as the binding's scope.
		/// </summary>
		Func<IContext, object> ScopeCallback { get; set; }

		/// <summary>
		/// Gets the parameters defined for the binding.
		/// </summary>
		ICollection<IParameter> Parameters { get; }

		/// <summary>
		/// Gets the actions that should be called after instances are activated via the binding.
		/// </summary>
        ICollection<Action<IContext, object>> ActivationActions { get; }

		/// <summary>
		/// Gets the actions that should be called before instances are deactivated via the binding.
		/// </summary>
		ICollection<Action<IContext, object>> DeactivationActions { get; }

		/// <summary>
		/// Gets the provider for the binding.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The provider to use.</returns>
		IProvider GetProvider(IContext context);

		/// <summary>
		/// Gets the scope for the binding, if any.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The object that will act as the scope, or <see langword="null"/> if the service is transient.</returns>
		object GetScope(IContext context);

		/// <summary>
		/// Determines whether the specified request satisfies the condition defined on the binding,
		/// if one was defined.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns><c>True</c> if the request satisfies the condition; otherwise <c>false</c>.</returns>
		bool Matches(IRequest request);
	}
}