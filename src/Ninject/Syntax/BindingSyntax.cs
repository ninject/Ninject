using System;
using Ninject.Activation;
using Ninject.Parameters;

namespace Ninject.Syntax
{
	public interface IBindingToSyntax<T> : IFluentSyntax
	{
		IBindingWhenInNamedOrWithSyntax<T> ToSelf();
		IBindingWhenInNamedOrWithSyntax<T> To<TImplementation>() where TImplementation : T;
		IBindingWhenInNamedOrWithSyntax<T> To(Type implementation);
		IBindingWhenInNamedOrWithSyntax<T> ToProvider<TProvider>() where TProvider : IProvider;
		IBindingWhenInNamedOrWithSyntax<T> ToProvider(IProvider provider);
		IBindingWhenInNamedOrWithSyntax<T> ToMethod(Func<IContext, T> provider);
		IBindingWhenInNamedOrWithSyntax<T> ToConstant(T value);
	}

	public interface IBindingWhenSyntax<T> : IFluentSyntax
	{
		IBindingWhenInNamedOrWithSyntax<T> When(Func<IRequest, bool> condition);
	}

	public interface IBindingInSyntax<T> : IFluentSyntax
	{
		IBindingNamedOrWithSyntax<T> InSingletonScope();
		IBindingNamedOrWithSyntax<T> InTransientScope();
		IBindingNamedOrWithSyntax<T> InThreadScope();
#if !NO_WEB
		IBindingNamedOrWithSyntax<T> InRequestScope();
#endif
		IBindingNamedOrWithSyntax<T> InScope(Func<IContext, object> scope);
	}

	public interface IBindingNamedSyntax<T> : IFluentSyntax
	{
		IBindingWithSyntax<T> Named(string name);
	}

	public interface IBindingWithSyntax<T> : IFluentSyntax
	{
		IBindingWithSyntax<T> WithConstructorArgument(string name, object value);
		IBindingWithSyntax<T> WithConstructorArgument(string name, Func<IContext, object> valueCallback);
		IBindingWithSyntax<T> WithPropertyValue(string name, object value);
		IBindingWithSyntax<T> WithPropertyValue(string name, Func<IContext, object> valueCallback);
		IBindingWithSyntax<T> WithParameter(IParameter parameter);
		IBindingWithSyntax<T> WithMetadata(string key, object value);
	}

	public interface IBindingWhenInNamedOrWithSyntax<T> : IBindingWhenSyntax<T>, IBindingInSyntax<T>, IBindingNamedSyntax<T>, IBindingWithSyntax<T> { }
	public interface IBindingInNamedOrWithSyntax<T> : IBindingInSyntax<T>, IBindingNamedSyntax<T>, IBindingWithSyntax<T> { }
	public interface IBindingNamedOrWithSyntax<T> : IBindingNamedSyntax<T>, IBindingWithSyntax<T> { }
}