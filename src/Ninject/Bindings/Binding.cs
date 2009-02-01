using System;
using System.Collections.Generic;
using Ninject.Activation;
using Ninject.Creation;

namespace Ninject.Bindings
{
	public class Binding : IBinding
	{
		private readonly Dictionary<string, object> _metadata = new Dictionary<string, object>();

		public Type Service { get; set; }
		public string Name { get; set; }

		public Func<IContext, IProvider> ProviderCallback { get; set; }
		public Func<IRequest, bool> ConditionCallback { get; set; }
		public Func<IContext, object> ScopeCallback { get; set; }

		public Binding(Type service)
		{
			Service = service;
		}

		public object GetMetadata(string key)
		{
			return _metadata.ContainsKey(key) ? _metadata[key] : null;
		}

		public void SetMetadata(string key, object value)
		{
			_metadata[key] = value;
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