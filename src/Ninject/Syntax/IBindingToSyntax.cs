using System;
using Ninject.Activation;
using Ninject.Creation;

namespace Ninject.Syntax
{
	public interface IBindingToSyntax : IFluentSyntax
	{
		IBindingWithNameMetadataWhenOrInScopeSyntax ToSelf();
		IBindingWithNameMetadataWhenOrInScopeSyntax To<TImplementation>();
		IBindingWithNameMetadataWhenOrInScopeSyntax To(Type implementation);
		IBindingWithNameMetadataWhenOrInScopeSyntax ToProvider<TProvider>() where TProvider : IProvider;
		IBindingWithNameMetadataWhenOrInScopeSyntax ToProvider(IProvider provider);
		IBindingWithNameMetadataWhenOrInScopeSyntax ToMethod<T>(Func<IContext, T> provider);
		IBindingWithNameMetadataWhenOrInScopeSyntax ToConstant<T>(T value);
	}
}