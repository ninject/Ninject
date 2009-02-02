using System;
using System.Collections.Generic;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Planning.Targets;
using Ninject.Resolution;

namespace Ninject.Activation
{
	public interface IRequest
	{
		ITarget Target { get; }
		Type Service { get; }

		ICollection<IConstraint> Constraints { get; }
		ICollection<IParameter> Parameters { get; }

		bool ConstraintsSatisfiedBy(IBinding binding);
		object GetScope();
	}
}