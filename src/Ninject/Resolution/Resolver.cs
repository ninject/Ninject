using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Components;
using Ninject.Planning;
using Ninject.Resolution.Strategies;

namespace Ninject.Resolution
{
	public class Resolver : NinjectComponent, IResolver
	{
		public IPlanner Planner { get; set; }
		public IPipeline Pipeline { get; set; }
		public ICache Cache { get; set; }
		public IList<IResolutionStrategy> Strategies { get; set; }

		public Resolver(IPlanner planner, IPipeline pipeline, ICache cache, IEnumerable<IResolutionStrategy> strategies)
		{
			Planner = planner;
			Pipeline = pipeline;
			Cache = cache;
			Strategies = strategies.ToList();
		}

		public bool HasStrategy(IRequest request)
		{
			return Strategies.Any(s => s.Supports(request));
		}

		public object Resolve(IContext context)
		{
			if (context.Instance != null)
			{
				Pipeline.Activate(context);
				return context.Instance;
			}

			object scope = context.GetScope();

			context.Instance = Cache.TryGet(context.Implementation, scope);

			if (context.Instance != null)
				return context.Instance;

			var strategy = Strategies.Where(s => s.Supports(context.Request)).FirstOrDefault();

			if (strategy != null)
			{
				context.Instance = strategy.Resolve(context);
				Cache.Remember(context);
			}
			else
			{
				context.Plan = Planner.GetPlan(context.Implementation);
				context.Instance = context.Provider.Create(context);
				Cache.Remember(context);
				Pipeline.Activate(context);
			}

			return context.Instance;
		}
	}
}