using System;
using Ninject.Activation;
using Ninject.Creation;

namespace Ninject.Syntax
{
	public interface IBindingToSyntax : IFluentSyntax
	{
		IBindingMetadataWhenOrInScopeSyntax ToSelf();
		IBindingMetadataWhenOrInScopeSyntax To<TImplementation>();
		IBindingMetadataWhenOrInScopeSyntax To(Type implementation);
		IBindingMetadataWhenOrInScopeSyntax ToProvider<TProvider>() where TProvider : IProvider;
		IBindingMetadataWhenOrInScopeSyntax ToProvider(IProvider provider);
		IBindingMetadataWhenOrInScopeSyntax ToMethod<T>(Func<IContext, T> provider);
		IBindingMetadataWhenOrInScopeSyntax ToConstant<T>(T value);
	}
}