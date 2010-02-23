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
using Ninject.Infrastructure;
using Ninject.Planning.Bindings;
#endregion

namespace Ninject.Syntax
{
	/// <summary>
	/// Provides a path to register bindings.
	/// </summary>
	public interface IBindingRoot
	{
		/// <summary>
		/// Declares a binding for the specified service.
		/// </summary>
		/// <typeparam name="T">The service to bind.</typeparam>
		IBindingToSyntax<T> Bind<T>();

		/// <summary>
		/// Declares a binding from the service to itself.
		/// </summary>
		/// <param name="service">The service to bind.</param>
		IBindingToSyntax<object> Bind(Type service);

		/// <summary>
		/// Unregisters all bindings for the specified service.
		/// </summary>
		/// <typeparam name="T">The service to unbind.</typeparam>
		void Unbind<T>();

		/// <summary>
		/// Unregisters all bindings for the specified service.
		/// </summary>
		/// <param name="service">The service to unbind.</param>
		void Unbind(Type service);

		/// <summary>
		/// Removes any existing bindings for the specified service, and declares a new one.
		/// </summary>
		/// <typeparam name="T">The service to re-bind.</typeparam>
		IBindingToSyntax<T> Rebind<T>();

		/// <summary>
		/// Removes any existing bindings for the specified service, and declares a new one.
		/// </summary>
		/// <param name="service">The service to re-bind.</param>
		IBindingToSyntax<object> Rebind(Type service);

		/// <summary>
		/// Registers the specified binding.
		/// </summary>
		/// <param name="binding">The binding to add.</param>
		void AddBinding(IBinding binding);

		/// <summary>
		/// Unregisters the specified binding.
		/// </summary>
		/// <param name="binding">The binding to remove.</param>
		void RemoveBinding(IBinding binding);
	}
}