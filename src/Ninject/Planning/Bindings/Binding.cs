using System;
using System.Collections.Generic;
using System.Diagnostics;
using Ninject.Activation;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Tracing;
using Ninject.Parameters;

namespace Ninject.Planning.Bindings
{
	[DebuggerDisplay("{IntrospectionInfo} from {TraceInfo}")]
	public class Binding : TraceInfoProvider, IBinding
	{
		public Type Service { get; set; }
		public IBindingMetadata Metadata { get; set; }
		public ICollection<IParameter> Parameters { get; private set; }

		public Func<IContext, IProvider> ProviderCallback { get; set; }
		public Func<IRequest, bool> ConditionCallback { get; set; }
		public Func<IContext, object> ScopeCallback { get; set; }

		public string IntrospectionInfo { get; set; }

		public Binding(Type service) : this(service, new BindingMetadata()) { }

		public Binding(Type service, IBindingMetadata metadata)
		{
			Service = service;
			Metadata = metadata;
			Parameters = new List<IParameter>();
			IntrospectionInfo = "Binding from " + service.Format();
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