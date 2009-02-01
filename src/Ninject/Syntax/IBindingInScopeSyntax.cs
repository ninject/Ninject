using System;
using Ninject.Activation;

namespace Ninject.Syntax
{
	public interface IBindingInScopeSyntax : IFluentSyntax
	{
		void InSingletonScope();
		void InTransientScope();
		void InThreadScope();
		void InRequestScope();
		void InScope(Func<IContext, object> scope);
	}
}