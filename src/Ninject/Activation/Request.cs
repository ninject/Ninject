using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Activation.Constraints;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Planning.Targets;

namespace Ninject.Activation
{
	public class Request : IRequest
	{
		public IRequest Parent { get; set; }
		public ITarget Target { get; set; }
		public Type Service { get; set; }

		public ICollection<IConstraint> Constraints { get; set; }
		public ICollection<IParameter> Parameters { get; set; }

		public Func<object> ScopeCallback { get; set; }

		public Request(Type service, IEnumerable<IConstraint> constraints, IEnumerable<IParameter> parameters, Func<object> scopeCallback)
		{
			Service = service;
			Constraints = constraints.ToList();
			Parameters = parameters.ToList();
			ScopeCallback = scopeCallback;
		}

		public Request(IRequest parent, Type service, ITarget target, Func<object> scopeCallback)
		{
			Parent = parent;
			Service = service;
			Target = target;
			Constraints = target.GetConstraints().ToList();
			Parameters = new List<IParameter>();
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

		public IRequest CreateChild(Type service, ITarget target)
		{
			return new Request(this, service, target, ScopeCallback);
		}
	}
}