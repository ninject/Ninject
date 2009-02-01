using System;

namespace Ninject.Syntax
{
	public interface IBindingMetadataSyntax : IFluentSyntax
	{
		IBindingMetadataWhenOrInScopeSyntax WithName(string name);
		IBindingMetadataWhenOrInScopeSyntax WithMetadata(string key, object value);
	}
}