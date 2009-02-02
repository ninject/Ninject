using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Planning.Targets;
using Ninject.Resolution;

namespace Ninject.Activation
{
	public class Request : IRequest
	{
		public ITarget Target { get; set; }
		public Type Service { get; set; }

		public ICollection<IConstraint> Constraints { get; set; }
		public ICollection<IParameter> Parameters { get; set; }

		public Func<object> ScopeCallback { get; set; }

		public Request(Type service, ITarget target, Func<object> scopeCallback)
		{
			Service = service;
			Target = target;
			Constraints = target.GetConstraints().ToList();
			Parameters = new List<IParameter>();
			ScopeCallback = scopeCallback;
		}

		public Request(Type service, IEnumerable<IConstraint> constraints, IEnumerable<IParameter> parameters, Func<object> scopeCallback)
		{
			Service = service;
			Constraints = constraints.ToList();
			Parameters = parameters.ToList();
			ScopeCallback = scopeCallback;
		}

		public bool ConstraintsSatisfiedBy(IBinding binding)
		{
			return Constraints.All(c => c.Matches(binding.Metadata));
		}

		public object GetScope()
		{
			return ScopeCallback == null ? null : ScopeCallback();
		}
	}
}