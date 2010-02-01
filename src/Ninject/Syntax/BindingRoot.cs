#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
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
			return RegisterBindingAndCreateBuilder<T>(typeof(T));
		}

		/// <summary>
		/// Declares a binding for the specified service.
		/// </summary>
		/// <param name="service">The service to bind.</param>
		public IBindingToSyntax<object> Bind(Type service)
		{
			Ensure.ArgumentNotNull(service, "service");
			return RegisterBindingAndCreateBuilder<object>(service);
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

		private BindingBuilder<T> RegisterBindingAndCreateBuilder<T>(Type service)
		{
			var binding = new Binding(service);
			AddBinding(binding);
			return new BindingBuilder<T>(binding, Kernel);
		}

		/// <summary>
		/// Gets the kernel.
		/// </summary>
		public abstract IKernel Kernel { get; protected set; }
	}
}