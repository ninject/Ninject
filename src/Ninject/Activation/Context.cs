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
using System.Collections.Generic;
using System.Linq;
using Ninject.Activation.Caching;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Introspection;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;
#endregion

namespace Ninject.Activation
{
	/// <summary>
	/// Contains information about the activation of a single instance.
	/// </summary>
	public class Context : IContext
	{
		/// <summary>
		/// Gets the kernel that is driving the activation.
		/// </summary>
		public IKernel Kernel { get; set; }

		/// <summary>
		/// Gets the request.
		/// </summary>
		public IRequest Request { get; set; }

		/// <summary>
		/// Gets the binding.
		/// </summary>
		public IBinding Binding { get; set; }

		/// <summary>
		/// Gets or sets the activation plan.
		/// </summary>
		public IPlan Plan { get; set; }

		/// <summary>
		/// Gets the parameters that were passed to manipulate the activation process.
		/// </summary>
		public ICollection<IParameter> Parameters { get; set; }

		/// <summary>
		/// Gets or sets the activated instance.
		/// </summary>
		public object Instance { get; set; }

		/// <summary>
		/// Gets the generic arguments for the request, if any.
		/// </summary>
		public Type[] GenericArguments { get; private set; }

		/// <summary>
		/// Gets a value indicating whether the request involves inferred generic arguments.
		/// </summary>
		public bool HasInferredGenericArguments { get; private set; }

		/// <summary>
		/// Gets or sets the cache component.
		/// </summary>
		public ICache Cache { get; private set; }

		/// <summary>
		/// Gets or sets the planner component.
		/// </summary>
		public IPlanner Planner { get; private set; }

		/// <summary>
		/// Gets or sets the pipeline component.
		/// </summary>
		public IPipeline Pipeline { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Context"/> class.
		/// </summary>
		/// <param name="kernel">The kernel managing the resolution.</param>
		/// <param name="request">The context's request.</param>
		/// <param name="binding">The context's binding.</param>
		/// <param name="cache">The cache component.</param>
		/// <param name="planner">The planner component.</param>
		/// <param name="pipeline">The pipeline component.</param>
		public Context(IKernel kernel, IRequest request, IBinding binding, ICache cache, IPlanner planner, IPipeline pipeline)
		{
			Ensure.ArgumentNotNull(kernel, "kernel");
			Ensure.ArgumentNotNull(request, "request");
			Ensure.ArgumentNotNull(binding, "binding");
			Ensure.ArgumentNotNull(cache, "cache");
			Ensure.ArgumentNotNull(planner, "planner");
			Ensure.ArgumentNotNull(pipeline, "pipeline");

			Kernel = kernel;
			Request = request;
			Binding = binding;
			Parameters = request.Parameters.Union(binding.Parameters).ToList();

			Cache = cache;
			Planner = planner;
			Pipeline = pipeline;

			if (binding.Service.IsGenericTypeDefinition)
			{
				HasInferredGenericArguments = true;
				GenericArguments = request.Service.GetGenericArguments();
			}
		}

		/// <summary>
		/// Gets the scope for the context that "owns" the instance activated therein.
		/// </summary>
		/// <returns>The object that acts as the scope.</returns>
		public object GetScope()
		{
			return Request.GetScope() ?? Binding.GetScope(this);
		}

		/// <summary>
		/// Gets the provider that should be used to create the instance for this context.
		/// </summary>
		/// <returns>The provider that should be used.</returns>
		public IProvider GetProvider()
		{
			return Binding.GetProvider(this);
		}

		/// <summary>
		/// Resolves the instance associated with this hook.
		/// </summary>
		/// <returns>The resolved instance.</returns>
		public object Resolve()
		{
			lock (Binding)
			{
				if (Request.ActiveBindings.Contains(Binding))
					throw new ActivationException(ExceptionFormatter.CyclicalDependenciesDetected(this));

				Request.ActiveBindings.Push(Binding);

				Instance = Cache.TryGet(this);

				if (Instance != null)
					return Instance;

				Instance = GetProvider().Create(this);

				if (GetScope() != null)
					Cache.Remember(this);

				Request.ActiveBindings.Pop();

				if (Plan == null)
					Plan = Planner.GetPlan(Instance.GetType());

				Pipeline.Activate(this);

				return Instance;
			}
		}
	}
}