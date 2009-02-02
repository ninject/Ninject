using System;
using Ninject.Planning.Bindings;

namespace Ninject.Resolution
{
	public class PredicateConstraint : IConstraint
	{
		public Func<IBindingMetadata, bool> Predicate { get; set; }

		public PredicateConstraint(Func<IBindingMetadata, bool> predicate)
		{
			Predicate = predicate;
		}

		public bool Matches(IBindingMetadata metadata)
		{
			return Predicate(metadata);
		}

		public static implicit operator PredicateConstraint(Func<IBindingMetadata, bool> predicate)
		{
			return new PredicateConstraint(predicate);
		}
	}
}