using System;
using Ninject.Messaging.Messages;
using Ninject.Planning.Bindings;

namespace Ninject.Syntax
{
	public interface IBindingRoot
	{
		event EventHandler<BindingMessage> BindingAdded;
		event EventHandler<BindingMessage> BindingRemoved;

		IBindingToSyntax<T> Bind<T>();
		IBindingToSyntax<object> Bind(Type service);

		void AddBinding(IBinding binding);
		void RemoveBinding(IBinding binding);
	}
}