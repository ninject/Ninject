using System;
using Ninject.Planning.Bindings;

namespace Ninject.Syntax
{
	public interface IBindingRoot
	{
		IBindingToSyntax Bind(Type service);
		void AddBinding(IBinding binding);
		void RemoveBinding(IBinding binding);
	}
}