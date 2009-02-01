using System;
using Ninject.Activation;

namespace Ninject.Syntax
{
	public interface IBindingWhenSyntax : IFluentSyntax
	{
		IBindingInScopeSyntax When(Func<IRequest, bool> condition);
	}
}