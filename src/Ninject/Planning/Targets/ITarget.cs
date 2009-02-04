using System;
using System.Collections.Generic;
using Ninject.Activation;
using Ninject.Activation.Constraints;

namespace Ninject.Planning.Targets
{
	public interface ITarget
	{
		Type Type { get; }
		string Name { get; }
		IEnumerable<IConstraint> GetConstraints();
		object ResolveWithin(IContext parent);
	}
}