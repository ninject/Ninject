using System;
using System.Collections.Generic;
using Ninject.Planning.Directives;

namespace Ninject.Planning
{
	public interface IPlan
	{
		Type Type { get; }

		void Add(IDirective directive);
		bool Has<TDirective>() where TDirective : IDirective;
		TDirective GetOne<TDirective>() where TDirective : IDirective;
		IEnumerable<TDirective> GetAll<TDirective>() where TDirective : IDirective;
	}
}