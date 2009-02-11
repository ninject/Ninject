using System;
using Ninject.Planning.Bindings;

namespace Ninject
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
	public abstract class ConstraintAttribute : Attribute
	{
		public abstract bool Matches(IBindingMetadata obj);
	}
}
