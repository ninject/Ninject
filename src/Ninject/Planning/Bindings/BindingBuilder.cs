using System;
using System.Threading;
using System.Web;
using Ninject.Activation;
using Ninject.Activation.Providers;
using Ninject.Parameters;
using Ninject.Syntax;

namespace Ninject.Planning.Bindings
{
	public class BindingBuilder : IBindingToSyntax, IBindingWhenInNamedOrWithSyntax, IBindingInNamedOrWithSyntax, IBindingNamedOrWithSyntax
	{
		public Binding Binding { get; set; }

		public BindingBuilder(Binding binding)
		{
			Binding = binding;
		}

		public IBindingWhenInNamedOrWithSyntax ToSelf()
		{
			Binding.ProviderCallback = StandardProvider.GetCreationCallback(Binding.Service);
			return this;
		}

		public IBindingWhenInNamedOrWithSyntax To<TImplementation>()
		{
			Binding.ProviderCallback = StandardProvider.GetCreationCallback(typeof(TImplementation));
			return this;
		}

		public IBindingWhenInNamedOrWithSyntax To(Type implementation)
		{
			Binding.ProviderCallback = StandardProvider.GetCreationCallback(implementation);
			return this;
		}

		public IBindingWhenInNamedOrWithSyntax ToMethod<T>(Func<IContext, T> method)
		{
			Binding.ProviderCallback = ctx => new CallbackProvider<T>(method);
			return this;
		}

		public IBindingWhenInNamedOrWithSyntax ToProvider<TProvider>()
			where TProvider : IProvider
		{
			Binding.ProviderCallback = ctx => ctx.Kernel.Get<TProvider>();
			return this;
		}

		public IBindingWhenInNamedOrWithSyntax ToProvider(IProvider provider)
		{
			Binding.ProviderCallback = ctx => provider;
			return this;
		}

		public IBindingWhenInNamedOrWithSyntax ToConstant<T>(T value)
		{
			Binding.ProviderCallback = ctx => new ConstantProvider<T>(value);
			return this;
		}

		public IBindingInNamedOrWithSyntax When(Func<IRequest, bool> condition)
		{
			Binding.ConditionCallback = condition;
			return this;
		}

		public IBindingWithSyntax Named(string name)
		{
			String.Intern(name);
			Binding.Metadata.Name = name;
			return this;
		}

		public IBindingNamedOrWithSyntax InSingletonScope()
		{
			Binding.ScopeCallback = ctx => ctx.Kernel;
			return this;
		}

		public IBindingNamedOrWithSyntax InTransientScope()
		{
			Binding.ScopeCallback = null;
			return this;
		}

		public IBindingNamedOrWithSyntax InThreadScope()
		{
			Binding.ScopeCallback = ctx => Thread.CurrentThread;
			return this;
		}

		public IBindingNamedOrWithSyntax InRequestScope()
		{
			Binding.ScopeCallback = ctx => HttpContext.Current;
			return this;
		}

		public IBindingNamedOrWithSyntax InScope(Func<IContext, object> scope)
		{
			Binding.ScopeCallback = scope;
			return this;
		}

		public IBindingWithSyntax WithConstructorArgument(string name, object value)
		{
			Binding.Parameters.Add(new ConstructorArgument(name, value));
			return this;
		}

		public IBindingWithSyntax WithConstructorArgument(string name, Func<IContext, object> valueCallback)
		{
			Binding.Parameters.Add(new ConstructorArgument(name, valueCallback));
			return this;
		}

		public IBindingWithSyntax WithPropertyValue(string name, object value)
		{
			Binding.Parameters.Add(new PropertyValue(name, value));
			return this;
		}

		public IBindingWithSyntax WithPropertyValue(string name, Func<IContext, object> valueCallback)
		{
			Binding.Parameters.Add(new PropertyValue(name, valueCallback));
			return this;
		}

		public IBindingWithSyntax WithParameter(IParameter parameter)
		{
			Binding.Parameters.Add(parameter);
			return this;
		}

		public IBindingWithSyntax WithMetadata(string key, object value)
		{
			Binding.Metadata.Set(key, value);
			return this;
		}
	}
}