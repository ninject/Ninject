using System;
using System.Collections.Generic;
using Ninject.Activation.Constraints;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Planning.Targets;

namespace Ninject.Activation
{
	public interface IRequest
	{
		IRequest Parent { get; }
		ITarget Target { get; }
		Type Service { get; }

		ICollection<IConstraint> Constraints { get; }
		ICollection<IParameter> Parameters { get; }

		bool ConstraintsSatisfiedBy(IBinding binding);
		object GetScope();
		IRequest CreateChild(Type service, ITarget target);
	}
}