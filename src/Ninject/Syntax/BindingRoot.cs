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
using Ninject.Infrastructure.Disposal;
using Ninject.Planning.Bindings;
#endregion

namespace Ninject.Syntax
{
	/// <summary>
	/// Provides a path to register bindings.
	/// </summary>
	public abstract class BindingRoot : DisposableObject, IBindingRoot
	{
		/// <summary>
		/// Declares a binding for the specified service.
		/// </summary>
		/// <typeparam name="T">The service to bind.</typeparam>
		public IBindingToSyntax<T> Bind<T>()
		{
			Type service = typeof(T);

			var binding = new Binding(service);
			AddBinding(binding);

			return CreateBindingBuilder<T>(binding);
		}

		/// <summary>
		/// Declares a binding for the specified service.
		/// </summary>
		/// <param name="service">The service to bind.</param>
		public IBindingToSyntax<object> Bind(Type service)
		{
			Ensure.ArgumentNotNull(service, "service");

			var binding = new Binding(service);
			AddBinding(binding);

			return CreateBindingBuilder<object>(binding);
		}

		/// <summary>
		/// Unregisters all bindings for the specified service.
		/// </summary>
		/// <typeparam name="T">The service to unbind.</typeparam>
		public void Unbind<T>()
		{
			Unbind(typeof(T));
		}

		/// <summary>
		/// Unregisters all bindings for the specified service.
		/// </summary>
		/// <param name="service">The service to unbind.</param>
		public abstract void Unbind(Type service);

		/// <summary>
		/// Removes any existing bindings for the specified service, and declares a new one.
		/// </summary>
		/// <typeparam name="T">The service to re-bind.</typeparam>
		public IBindingToSyntax<T> Rebind<T>()
		{
			Unbind<T>();
			return Bind<T>();
		}

		/// <summary>
		/// Removes any existing bindings for the specified service, and declares a new one.
		/// </summary>
		/// <param name="service">The service to re-bind.</param>
		public IBindingToSyntax<object> Rebind(Type service)
		{
			Unbind(service);
			return Bind(service);
		}

		/// <summary>
		/// Registers the specified binding.
		/// </summary>
		/// <param name="binding">The binding to add.</param>
		public abstract void AddBinding(IBinding binding);

		/// <summary>
		/// Unregisters the specified binding.
		/// </summary>
		/// <param name="binding">The binding to remove.</param>
		public abstract void RemoveBinding(IBinding binding);

		/// <summary>
		/// Creates a new builder for the specified binding.
		/// </summary>
		/// <typeparam name="T">The type restriction to apply to the binding builder.</typeparam>
		/// <param name="binding">The binding that will be built.</param>
		/// <returns>The created builder.</returns>
		protected abstract BindingBuilder<T> CreateBindingBuilder<T>(IBinding binding);
	}
}