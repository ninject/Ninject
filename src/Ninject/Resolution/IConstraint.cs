using System;
using Ninject.Bindings;

namespace Ninject.Resolution
{
	public interface IConstraint
	{
		bool Matches(IBinding binding);
	}
}