using System;
using System.Collections.Generic;
using Ninject.Activation;
using Ninject.Planning.Bindings;

namespace Ninject.Planning.Targets
{
	public interface ITarget
	{
		Type Type { get; }
		string Name { get; }
		IEnumerable<Func<IBindingMetadata, bool>> GetConstraints();
		object ResolveWithin(IContext parent);
	}
}