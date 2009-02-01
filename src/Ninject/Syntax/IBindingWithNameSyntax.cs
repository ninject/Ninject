using System;

namespace Ninject.Syntax
{
	public interface IBindingWithNameSyntax : IFluentSyntax
	{
		IBindingMetadataWhenOrInScopeSyntax WithName(string name);
	}
}