using System;
using Ninject.Planning.Bindings;

namespace Ninject.Activation.Constraints
{
	public interface IConstraint
	{
		bool Matches(IBindingMetadata metadata);
	}
}