using System;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Components;
using Ninject.Planning;

namespace Ninject.Resolution
{
	public class Resolver : NinjectComponent, IResolver
	{
		public IPlanner Planner { get; set; }
		public IPipeline Pipeline { get; set; }
		public ICache Cache { get; set; }

		public Resolver(IPlanner planner, IPipeline pipeline, ICache cache)
		{
			Planner = planner;
			Pipeline = pipeline;
			Cache = cache;
		}

		public object Resolve(IContext context)
		{
			lock (context.Binding)
			{
				if (context.Instance != null)
				{
					Pipeline.Activate(context);
					return context.Instance;
				}

				object scope = context.GetScope();

				context.Instance = Cache.TryGet(context.Binding, scope);

				if (context.Instance != null)
					return context.Instance;

				context.Plan = Planner.GetPlan(context.Implementation);
				context.Instance = context.Provider.Create(context);

				Cache.Remember(context);
				Pipeline.Activate(context);

				return context.Instance;
			}
		}
	}
}