using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Components;
using Ninject.Infrastructure.Language;
using Ninject.Planning.Strategies;

namespace Ninject.Planning
{
	public class Planner : NinjectComponent, IPlanner
	{
		private readonly Dictionary<Type, IPlan> _plans = new Dictionary<Type, IPlan>();

		public IList<IPlanningStrategy> Strategies { get; private set; }

		public Planner(IEnumerable<IPlanningStrategy> strategies)
		{
			Strategies = strategies.ToList();
		}

		public IPlan GetPlan(Type type)
		{
			if (_plans.ContainsKey(type))
				return _plans[type];

			var plan = CreateEmptyPlan(type);
			_plans.Add(type, plan);

			Strategies.Map(s => s.Execute(plan));

			return plan;
		}

		protected virtual IPlan CreateEmptyPlan(Type type)
		{
			return new Plan(type);
		}
	}
}