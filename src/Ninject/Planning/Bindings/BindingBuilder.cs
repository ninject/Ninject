using System;
using System.Threading;
#if !NO_WEB
using System.Web;
#endif
using Ninject.Activation;
using Ninject.Activation.Providers;
using Ninject.Infrastructure;
using Ninject.Parameters;
using Ninject.Syntax;

namespace Ninject.Planning.Bindings
{
	public class BindingBuilder<T> : IBindingToSyntax<T>, IBindingWhenInNamedOrWithSyntax<T>, IBindingInNamedOrWithSyntax<T>, IBindingNamedOrWithSyntax<T>
	{
		public Binding Binding { get; set; }

		public BindingBuilder(Binding binding)
		{
			Binding = binding;
		}

		public IBindingWhenInNamedOrWithSyntax<T> ToSelf()
		{
			Binding.ProviderCallback = StandardProvider.GetCreationCallback(Binding.Service);
			Binding.IntrospectionInfo += " to self";
			return this;
		}

		public IBindingWhenInNamedOrWithSyntax<T> To<TImplementation>() where TImplementation : T
		{
			Binding.ProviderCallback = StandardProvider.GetCreationCallback(typeof(TImplementation));
			Binding.IntrospectionInfo += " to " + typeof(TImplementation).Format();
			return this;
		}

		public IBindingWhenInNamedOrWithSyntax<T> To(Type implementation)
		{
			Binding.ProviderCallback = StandardProvider.GetCreationCallback(implementation);
			Binding.IntrospectionInfo += " to " + implementation.Format();
			return this;
		}

		public IBindingWhenInNamedOrWithSyntax<T> ToMethod(Func<IContext, T> method)
		{
			Binding.ProviderCallback = ctx => new CallbackProvider<T>(method);
			Binding.IntrospectionInfo += " to method " + method.Method;
			return this;
		}

		public IBindingWhenInNamedOrWithSyntax<T> ToProvider<TProvider>()
			where TProvider : IProvider
		{
			Binding.ProviderCallback = ctx => ctx.Kernel.Get<TProvider>();
			Binding.IntrospectionInfo += " to provider " + typeof(TProvider).Format();
			return this;
		}

		public IBindingWhenInNamedOrWithSyntax<T> ToProvider(IProvider provider)
		{
			Binding.ProviderCallback = ctx => provider;
			Binding.IntrospectionInfo += " to external instance of provider " + provider.GetType().Format();
			return this;
		}

		public IBindingWhenInNamedOrWithSyntax<T> ToConstant(T value)
		{
			Binding.ProviderCallback = ctx => new ConstantProvider<T>(value);
			Binding.IntrospectionInfo += " to constant " + value;
			return this;
		}

		public IBindingInNamedOrWithSyntax<T> When(Func<IRequest, bool> condition)
		{
			Binding.ConditionCallback = condition;
			Binding.IntrospectionInfo += " (conditionally)";
			return this;
		}

		public IBindingWithSyntax<T> Named(string name)
		{
			String.Intern(name);
			Binding.Metadata.Name = name;
			Binding.IntrospectionInfo += " with name '" + name + "'";
			return this;
		}

		public IBindingNamedOrWithSyntax<T> InSingletonScope()
		{
			Binding.ScopeCallback = ctx => ctx.Kernel;
			Binding.IntrospectionInfo += " in singleton scope";
			return this;
		}

		public IBindingNamedOrWithSyntax<T> InTransientScope()
		{
			Binding.ScopeCallback = null;
			Binding.IntrospectionInfo += " in transient scope";
			return this;
		}

		public IBindingNamedOrWithSyntax<T> InThreadScope()
		{
			Binding.ScopeCallback = ctx => Thread.CurrentThread;
			Binding.IntrospectionInfo += " in thread scope";
			return this;
		}

#if !NO_WEB
		public IBindingNamedOrWithSyntax<T> InRequestScope()
		{
			Binding.ScopeCallback = ctx => HttpContext.Current;
			Binding.IntrospectionInfo += " in request scope";
			return this;
		}
#endif

		public IBindingNamedOrWithSyntax<T> InScope(Func<IContext, object> scope)
		{
			Binding.ScopeCallback = scope;
			Binding.IntrospectionInfo += " in custom scope";
			return this;
		}

		public IBindingWithSyntax<T> WithConstructorArgument(string name, object value)
		{
			Binding.Parameters.Add(new ConstructorArgument(name, value));
			return this;
		}

		public IBindingWithSyntax<T> WithConstructorArgument(string name, Func<IContext, object> valueCallback)
		{
			Binding.Parameters.Add(new ConstructorArgument(name, valueCallback));
			return this;
		}

		public IBindingWithSyntax<T> WithPropertyValue(string name, object value)
		{
			Binding.Parameters.Add(new PropertyValue(name, value));
			return this;
		}

		public IBindingWithSyntax<T> WithPropertyValue(string name, Func<IContext, object> valueCallback)
		{
			Binding.Parameters.Add(new PropertyValue(name, valueCallback));
			return this;
		}

		public IBindingWithSyntax<T> WithParameter(IParameter parameter)
		{
			Binding.Parameters.Add(parameter);
			return this;
		}

		public IBindingWithSyntax<T> WithMetadata(string key, object value)
		{
			Binding.Metadata.Set(key, value);
			Binding.IntrospectionInfo += " with metadata " + key + " = " + value;
			return this;
		}
	}
}