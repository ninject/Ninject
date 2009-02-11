using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Planning.Directives;

namespace Ninject.Planning
{
	public class Plan : IPlan
	{
		public Type Type { get; private set; }
		public ICollection<IDirective> Directives { get; private set; }

		public Plan(Type type)
		{
			Type = type;
			Directives = new List<IDirective>();
		}

		public void Add(IDirective directive)
		{
			Directives.Add(directive);
		}

		public bool Has<TDirective>()
			where TDirective : IDirective
		{
			return GetAll<TDirective>().Count() > 0;
		}

		public TDirective GetOne<TDirective>()
			where TDirective : IDirective
		{
			return GetAll<TDirective>().SingleOrDefault();
		}

		public IEnumerable<TDirective> GetAll<TDirective>()
			where TDirective : IDirective
		{
			return Directives.OfType<TDirective>();
		}
	}
}