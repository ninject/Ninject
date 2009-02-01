using System;
using Ninject.Bindings;
using Ninject.Syntax;

namespace Ninject.Infrastructure
{
	public interface IBindingRoot
	{
		IBindingToSyntax Bind(Type service);
		void AddBinding(IBinding binding);
		void RemoveBinding(IBinding binding);
	}
}