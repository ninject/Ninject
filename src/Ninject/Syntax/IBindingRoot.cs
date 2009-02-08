using System;
using Ninject.Events;
using Ninject.Planning.Bindings;

namespace Ninject.Syntax
{
	public interface IBindingRoot
	{
		event EventHandler<BindingEventArgs> BindingAdded;
		event EventHandler<BindingEventArgs> BindingRemoved;

		IBindingToSyntax<T> Bind<T>();
		IBindingToSyntax<object> Bind(Type service);

		void AddBinding(IBinding binding);
		void RemoveBinding(IBinding binding);
	}
}