using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Planning.Directives;

namespace Ninject.Planning
{
	public class Plan : IPlan
	{
		private readonly List<IDirective> _directives = new List<IDirective>();

		public Type Type { get; private set; }

		public Plan(Type type)
		{
			Type = type;
		}

		public void Add(IDirective directive)
		{
			_directives.Add(directive);
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
			return _directives.OfType<TDirective>();
		}
	}
}