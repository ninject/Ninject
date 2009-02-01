using System;
using Ninject.Activation;
using Ninject.Creation;

namespace Ninject.Bindings
{
	public class Binding : IBinding
	{
		public Type Service { get; set; }
		public IBindingMetadata Metadata { get; set; }

		public Func<IContext, IProvider> ProviderCallback { get; set; }
		public Func<IRequest, bool> ConditionCallback { get; set; }
		public Func<IContext, object> ScopeCallback { get; set; }

		public Binding(Type service)
		{
			Service = service;
			Metadata = new BindingMetadata();
		}

		public IProvider GetProvider(IContext context)
		{
			return ProviderCallback == null ? null : ProviderCallback(context);
		}

		public object GetScope(IContext context)
		{
			return ScopeCallback == null ? null : ScopeCallback(context);
		}

		public bool Matches(IRequest request)
		{
			return ConditionCallback == null || ConditionCallback(request);
		}
	}
}