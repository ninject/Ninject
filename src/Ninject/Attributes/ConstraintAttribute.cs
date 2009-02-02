using System;
using Ninject.Planning.Bindings;
using Ninject.Resolution;

namespace Ninject
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
	public abstract class ConstraintAttribute : Attribute, IConstraint
	{
		public abstract bool Matches(IBindingMetadata metadata);
	}
}
