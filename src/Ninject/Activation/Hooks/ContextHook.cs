#region License
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
#region Using Directives
using System;
using Ninject.Activation.Caching;
using Ninject.Infrastructure.Introspection;
using Ninject.Planning;
#endregion

namespace Ninject.Activation.Hooks
{
	/// <summary>
	/// A hook that resolves a context.
	/// </summary>
	public class ContextHook : IHook
	{
		/// <summary>
		/// Gets the context that will be resolved by the hook.
		/// </summary>
		public IContext Context { get; private set; }

		/// <summary>
		/// Gets or the cache component.
		/// </summary>
		public ICache Cache { get; private set; }

		/// <summary>
		/// Gets or the planner component.
		/// </summary>
		public IPlanner Planner { get; private set; }

		/// <summary>
		/// Gets or the planner component.
		/// </summary>
		public IPipeline Pipeline { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ContextHook"/> class.
		/// </summary>
		/// <param name="context">The context to resolve.</param>
		/// <param name="cache">The cache to use to look up instances for re-use.</param>
		/// <param name="planner">The planner used to generate activation plans for instances.</param>
		/// <param name="pipeline">The pipeline used to activate instances.</param>
		public ContextHook(IContext context, ICache cache, IPlanner planner, IPipeline pipeline)
		{
			Context = context;
			Cache = cache;
			Planner = planner;
			Pipeline = pipeline;
		}

		/// <summary>
		/// Resolves the instance associated with this hook.
		/// </summary>
		/// <returns>The resolved instance.</returns>
		public object Resolve()
		{
			lock (Context.Binding)
			{
				if (Context.Request.ActiveBindings.Contains(Context.Binding))
					throw new ActivationException(ExceptionFormatter.CyclicalDependenciesDetected(Context));

				Context.Request.ActiveBindings.Push(Context.Binding);

				Context.Instance = Cache.TryGet(Context);

				if (Context.Instance != null)
					return Context.Instance;

				Context.Instance = Context.GetProvider().Create(Context);

				if (Context.GetScope() != null)
					Cache.Remember(Context);

				Context.Request.ActiveBindings.Pop();

				if (Context.Plan == null)
					Context.Plan = Planner.GetPlan(Context.Instance.GetType());

				Pipeline.Activate(Context);

				return Context.Instance;
			}
		}
	}
}