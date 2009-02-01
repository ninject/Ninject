using System;

namespace Ninject.Syntax
{
	public interface IBindingMetadataSyntax : IFluentSyntax
	{
		IBindingMetadataWhenOrInScopeSyntax WithMetadata(string key, object value);
	}
}