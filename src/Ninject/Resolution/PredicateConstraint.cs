using System;
using Ninject.Bindings;

namespace Ninject.Resolution
{
	public class PredicateConstraint : IConstraint
	{
		public Func<IBinding, bool> Predicate { get; set; }

		public PredicateConstraint(Func<IBinding, bool> predicate)
		{
			Predicate = predicate;
		}

		public bool Matches(IBinding binding)
		{
			return Predicate(binding);
		}

		public static implicit operator PredicateConstraint(Func<IBinding, bool> predicate)
		{
			return new PredicateConstraint(predicate);
		}
	}
}