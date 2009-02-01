using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Activation.Strategies;
using Ninject.Syntax;

namespace Ninject.Activation
{
	public class Pipeline : IPipeline
	{
		public IList<IActivationStrategy> Strategies { get; private set; }

		public Pipeline(IEnumerable<IActivationStrategy> strategies)
		{
			Strategies = strategies.ToList();
		}

		public void Activate(IContext context)
		{
			Strategies.Map(s => s.Activate(context));
		}

		public void Deactivate(IContext context)
		{
			Strategies.Map(s => s.Deactivate(context));
		}
	}
}