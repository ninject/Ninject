using System;
using Ninject.Planning.Bindings;

namespace Ninject.Resolution
{
	public interface IConstraint
	{
		bool Matches(IBindingMetadata metadata);
	}
}