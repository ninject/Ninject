using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ninject.Activation;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Tracing;
using Ninject.Parameters;

namespace Ninject.Planning.Bindings
{
	[DebuggerDisplay("{IntrospectionInfo} from {TraceInfo}")]
	public class Binding : TraceInfoProvider, IBinding
	{
		private IProvider _provider;

		public Type Service { get; private set; }
		public IBindingMetadata Metadata { get; private set; }

		public ICollection<Func<IRequest, bool>> Conditions { get; private set; }
		public ICollection<IParameter> Parameters { get; private set; }

		public ICollection<Action<IContext>> ActivationActions { get; private set; }
		public ICollection<Action<IContext>> DeactivationActions { get; private set; }

		public Func<IContext, IProvider> ProviderCallback { get; set; }
		public Func<IContext, object> ScopeCallback { get; set; }

		public string IntrospectionInfo { get; set; }

		public Binding(Type service) : this(service, new BindingMetadata()) { }

		public Binding(Type service, IBindingMetadata metadata)
		{
			Service = service;
			Metadata = metadata;
			Conditions = new List<Func<IRequest, bool>>();
			Parameters = new List<IParameter>();
			ActivationActions = new List<Action<IContext>>();
			DeactivationActions = new List<Action<IContext>>();
			IntrospectionInfo = "Binding from " + service.Format();
		}

		public IProvider GetProvider(IContext context)
		{
			if (_provider == null)
				_provider = ProviderCallback(context);

			return _provider;
		}

		public object GetScope(IContext context)
		{
			return ScopeCallback == null ? null : ScopeCallback(context);
		}

		public bool ConditionsSatisfiedBy(IRequest request)
		{
			return Conditions.All(condition => condition(request));
		}
	}
}