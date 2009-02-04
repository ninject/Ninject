using System;
using Ninject.Activation.Constraints;
using Ninject.Planning.Bindings;

namespace Ninject
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
	public abstract class ConstraintAttribute : Attribute, IConstraint
	{
		public abstract bool Matches(IBindingMetadata metadata);
	}
}
