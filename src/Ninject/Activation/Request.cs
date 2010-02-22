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
using System.Linq;
using Ninject.Infrastructure;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Planning.Targets;
#endregion

namespace Ninject.Activation
{
	/// <summary>
	/// Describes the request for a service resolution.
	/// </summary>
	public class Request : IRequest
	{
		/// <summary>
		/// Gets the service that was requested.
		/// </summary>
		public Type Service { get; private set; }

		/// <summary>
		/// Gets the parent request.
		/// </summary>
		public IRequest ParentRequest { get; private set; }

		/// <summary>
		/// Gets the parent context.
		/// </summary>
		public IContext ParentContext { get; private set; }

		/// <summary>
		/// Gets the target that will receive the injection, if any.
		/// </summary>
		public ITarget Target { get; private set; }

		/// <summary>
		/// Gets the constraint that will be applied to filter the bindings used for the request.
		/// </summary>
		public Func<IBindingMetadata, bool> Constraint { get; private set; }

		/// <summary>
		/// Gets the parameters that affect the resolution.
		/// </summary>
		public ICollection<IParameter> Parameters { get; private set; }

		/// <summary>
		/// Gets the stack of bindings which have been activated by either this request or its ancestors.
		/// </summary>
		public Stack<IBinding> ActiveBindings { get; private set; }

		/// <summary>
		/// Gets the recursive depth at which this request occurs.
		/// </summary>
		public int Depth { get; private set; }

		/// <summary>
		/// Gets or sets value indicating whether the request is optional.
		/// </summary>
		public bool IsOptional { get; set; }

		/// <summary>
		/// Gets or sets value indicating whether the request is for a single service.
		/// </summary>
		public bool IsUnique
		{
			get; set;
		}

		/// <summary>
		/// Gets the callback that resolves the scope for the request, if an external scope was provided.
		/// </summary>
		public Func<object> ScopeCallback { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Request"/> class.
		/// </summary>
		/// <param name="service">The service that was requested.</param>
		/// <param name="constraint">The constraint that will be applied to filter the bindings used for the request.</param>
		/// <param name="parameters">The parameters that affect the resolution.</param>
		/// <param name="scopeCallback">The scope callback, if an external scope was specified.</param>
		/// <param name="isOptional"><c>True</c> if the request is optional; otherwise, <c>false</c>.</param>
		/// <param name="isUnique"><c>True</c> if the request should return a unique result; otherwise, <c>false</c>.</param>
		public Request(Type service, Func<IBindingMetadata, bool> constraint, IEnumerable<IParameter> parameters, Func<object> scopeCallback, bool isOptional, bool isUnique)
		{
			Ensure.ArgumentNotNull(service, "service");
			Ensure.ArgumentNotNull(parameters, "parameters");

			Service = service;
			Constraint = constraint;
			Parameters = parameters.ToList();
			ScopeCallback = scopeCallback;
			ActiveBindings = new Stack<IBinding>();
			Depth = 0;
			IsOptional = isOptional;
			IsUnique = isUnique;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Request"/> class.
		/// </summary>
		/// <param name="parentContext">The parent context.</param>
		/// <param name="service">The service that was requested.</param>
		/// <param name="target">The target that will receive the injection.</param>
		/// <param name="scopeCallback">The scope callback, if an external scope was specified.</param>
		public Request(IContext parentContext, Type service, ITarget target, Func<object> scopeCallback)
		{
			Ensure.ArgumentNotNull(parentContext, "parentContext");
			Ensure.ArgumentNotNull(service, "service");
			Ensure.ArgumentNotNull(target, "target");

			ParentContext = parentContext;
			ParentRequest = parentContext.Request;
			Service = service;
			Target = target;
			Constraint = target.Constraint;
			IsOptional = target.IsOptional;
			Parameters = parentContext.Parameters.Where(p => p.ShouldInherit).ToList();
			ScopeCallback = scopeCallback;
			ActiveBindings = new Stack<IBinding>(ParentRequest.ActiveBindings);
			Depth = ParentRequest.Depth + 1;
		}

		/// <summary>
		/// Determines whether the specified binding satisfies the constraints defined on this request.
		/// </summary>
		/// <param name="binding">The binding.</param>
		/// <returns><c>True</c> if the binding satisfies the constraints; otherwise <c>false</c>.</returns>
		public bool Matches(IBinding binding)
		{
			return Constraint == null || Constraint(binding.Metadata);
		}

		/// <summary>
		/// Gets the scope if one was specified in the request.
		/// </summary>
		/// <returns>The object that acts as the scope.</returns>
		public object GetScope()
		{
			return ScopeCallback == null ? null : ScopeCallback();
		}

		/// <summary>
		/// Creates a child request.
		/// </summary>
		/// <param name="service">The service that is being requested.</param>
		/// <param name="parentContext">The context in which the request was made.</param>
		/// <param name="target">The target that will receive the injection.</param>
		/// <returns>The child request.</returns>
		public IRequest CreateChild(Type service, IContext parentContext, ITarget target)
		{
			return new Request(parentContext, service, target, ScopeCallback);
		}
	}
}