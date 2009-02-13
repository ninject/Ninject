using System;
using Ninject.Events;
using Ninject.Planning.Bindings;

namespace Ninject.Syntax
{
	/// <summary>
	/// Provides a path to register bindings.
	/// </summary>
	public interface IBindingRoot
	{
		/// <summary>
		/// Occurs when a binding is added.
		/// </summary>
		event EventHandler<BindingEventArgs> BindingAdded;

		/// <summary>
		/// Occurs when a binding is removed.
		/// </summary>
		event EventHandler<BindingEventArgs> BindingRemoved;

		/// <summary>
		/// Declares a binding for the specified service.
		/// </summary>
		/// <typeparam name="T">The service to bind.</typeparam>
		IBindingToSyntax<T> Bind<T>();

		/// <summary>
		/// Declares a binding for the specified service.
		/// </summary>
		/// <param name="service">The service to bind.</param>
		IBindingToSyntax<object> Bind(Type service);

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