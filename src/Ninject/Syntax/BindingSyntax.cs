using System;
using Ninject.Activation;
using Ninject.Creation;
using Ninject.Parameters;

namespace Ninject.Syntax
{
	public interface IBindingToSyntax : IFluentSyntax
	{
		IBindingWhenInNamedOrWithSyntax ToSelf();
		IBindingWhenInNamedOrWithSyntax To<TImplementation>();
		IBindingWhenInNamedOrWithSyntax To(Type implementation);
		IBindingWhenInNamedOrWithSyntax ToProvider<TProvider>() where TProvider : IProvider;
		IBindingWhenInNamedOrWithSyntax ToProvider(IProvider provider);
		IBindingWhenInNamedOrWithSyntax ToMethod<T>(Func<IContext, T> provider);
		IBindingWhenInNamedOrWithSyntax ToConstant<T>(T value);
	}

	public interface IBindingWhenSyntax : IFluentSyntax
	{
		IBindingInNamedOrWithSyntax When(Func<IRequest, bool> condition);
	}

	public interface IBindingInSyntax : IFluentSyntax
	{
		IBindingNamedOrWithSyntax InSingletonScope();
		IBindingNamedOrWithSyntax InTransientScope();
		IBindingNamedOrWithSyntax InThreadScope();
		IBindingNamedOrWithSyntax InRequestScope();
		IBindingNamedOrWithSyntax InScope(Func<IContext, object> scope);
	}

	public interface IBindingNamedSyntax : IFluentSyntax
	{
		IBindingWithSyntax Named(string name);
	}

	public interface IBindingWithSyntax : IFluentSyntax
	{
		IBindingWithSyntax WithConstructorArgument(string name, object value);
		IBindingWithSyntax WithConstructorArgument(string name, Func<IContext, object> valueCallback);
		IBindingWithSyntax WithPropertyValue(string name, object value);
		IBindingWithSyntax WithPropertyValue(string name, Func<IContext, object> valueCallback);
		IBindingWithSyntax WithParameter(IParameter parameter);
		IBindingWithSyntax WithMetadata(string key, object value);
	}

	public interface IBindingWhenInNamedOrWithSyntax : IBindingWhenSyntax, IBindingInSyntax, IBindingNamedSyntax, IBindingWithSyntax { }
	public interface IBindingInNamedOrWithSyntax : IBindingInSyntax, IBindingNamedSyntax, IBindingWithSyntax { }
	public interface IBindingNamedOrWithSyntax : IBindingNamedSyntax, IBindingWithSyntax { }
}