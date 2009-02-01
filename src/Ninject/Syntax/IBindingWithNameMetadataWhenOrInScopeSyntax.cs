using System;

namespace Ninject.Syntax
{
	public interface IBindingWithNameMetadataWhenOrInScopeSyntax
		: IBindingWithNameSyntax, IBindingMetadataSyntax, IBindingWhenSyntax, IBindingInScopeSyntax { }
}