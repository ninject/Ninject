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
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Planning.Targets;
using Ninject.Syntax;
#endregion

namespace Ninject.Activation
{
	/// <summary>
	/// Describes the request for a service resolution.
	/// </summary>
	public interface IRequest
	{
		/// <summary>
		/// Gets the service that was requested.
		/// </summary>
		Type Service { get; }

		/// <summary>
		/// Gets the parent request.
		/// </summary>
		IRequest ParentRequest { get; }

		/// <summary>
		/// Gets the parent context.
		/// </summary>
		IContext ParentContext { get; }

		/// <summary>
		/// Gets the target that will receive the injection, if any.
		/// </summary>
		ITarget Target { get; }

		/// <summary>
		/// Gets the constraint that will be applied to filter the bindings used for the request.
		/// </summary>
		Func<IBindingMetadata, bool> Constraint { get; }

		/// <summary>
		/// Gets the parameters that affect the resolution.
		/// </summary>
		ICollection<IParameter> Parameters { get; }

		/// <summary>
		/// Gets the stack of bindings which have been activated by either this request or its ancestors.
		/// </summary>
		Stack<IBinding> ActiveBindings { get; }

		/// <summary>
		/// Gets the recursive depth at which this request occurs.
		/// </summary>
		int Depth { get; }

		/// <summary>
		/// Gets or sets value indicating whether the request is optional.
		/// </summary>
		bool IsOptional { get; set; }

		/// <summary>
		/// Gets or sets value indicating whether the request should return a unique result.
		/// </summary>
		bool IsUnique { get; set; }

		/// <summary>
		/// Determines whether the specified binding satisfies the constraint defined on this request.
		/// </summary>
		/// <param name="binding">The binding.</param>
		/// <returns><c>True</c> if the binding satisfies the constraint; otherwise <c>false</c>.</returns>
		bool Matches(IBinding binding);

		/// <summary>
		/// Gets the scope if one was specified in the request.
		/// </summary>
		/// <returns>The object that acts as the scope.</returns>
		object GetScope();

		/// <summary>
		/// Creates a child request.
		/// </summary>
		/// <param name="service">The service that is being requested.</param>
		/// <param name="parentContext">The context in which the request was made.</param>
		/// <param name="target">The target that will receive the injection.</param>
		/// <returns>The child request.</returns>
		IRequest CreateChild(Type service, IContext parentContext, ITarget target);
	}
}