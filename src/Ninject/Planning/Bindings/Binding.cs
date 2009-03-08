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
using Ninject.Activation;
using Ninject.Infrastructure;
using Ninject.Parameters;
#endregion

namespace Ninject.Planning.Bindings
{
	/// <summary>
	/// Contains information about a service registration.
	/// </summary>
	public class Binding : IBinding
	{
		/// <summary>
		/// Gets the service type that is controlled by the binding.
		/// </summary>
		public Type Service { get; private set; }

		/// <summary>
		/// Gets the binding's metadata.
		/// </summary>
		public IBindingMetadata Metadata { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether the binding was implicitly registered.
		/// </summary>
		public bool IsImplicit { get; set; }

		/// <summary>
		/// Gets or sets the type of target for the binding.
		/// </summary>
		public BindingTarget Target { get; set; }

		/// <summary>
		/// Gets or sets the condition defined for the binding.
		/// </summary>
		public Func<IRequest, bool> Condition { get; set; }

		/// <summary>
		/// Gets the parameters defined for the binding.
		/// </summary>
		public ICollection<IParameter> Parameters { get; private set; }

		/// <summary>
		/// Gets the actions that should be called after instances are activated via the binding.
		/// </summary>
		public ICollection<Action<IContext>> ActivationActions { get; private set; }

		/// <summary>
		/// Gets the actions that should be called before instances are deactivated via the binding.
		/// </summary>
		public ICollection<Action<IContext>> DeactivationActions { get; private set; }

		/// <summary>
		/// Gets or sets the callback that returns the provider that should be used by the binding.
		/// </summary>
		public Func<IContext, IProvider> ProviderCallback { get; set; }

		/// <summary>
		/// Gets or sets the callback that returns the object that will act as the binding's scope.
		/// </summary>
		public Func<IContext, object> ScopeCallback { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Binding"/> class.
		/// </summary>
		/// <param name="service">The service that is controlled by the binding.</param>
		public Binding(Type service) : this(service, new BindingMetadata()) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Binding"/> class.
		/// </summary>
		/// <param name="service">The service that is controlled by the binding.</param>
		/// <param name="metadata">The binding's metadata container.</param>
		public Binding(Type service, IBindingMetadata metadata)
		{
			Ensure.ArgumentNotNull(service, "service");
			Ensure.ArgumentNotNull(metadata, "metadata");

			Service = service;
			Metadata = metadata;
			Parameters = new List<IParameter>();
			ActivationActions = new List<Action<IContext>>();
			DeactivationActions = new List<Action<IContext>>();
			ScopeCallback = StandardScopeCallbacks.Singleton;
		}

		/// <summary>
		/// Gets the provider for the binding.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The provider to use.</returns>
		public IProvider GetProvider(IContext context)
		{
			Ensure.ArgumentNotNull(context, "context");
			return ProviderCallback(context);
		}

		/// <summary>
		/// Gets the scope for the binding, if any.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The object that will act as the scope, or <see langword="null"/> if the service is transient.</returns>
		public object GetScope(IContext context)
		{
			Ensure.ArgumentNotNull(context, "context");
			return ScopeCallback(context);
		}

		/// <summary>
		/// Determines whether the specified request satisfies the conditions defined on this binding.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns><c>True</c> if the request satisfies the conditions; otherwise <c>false</c>.</returns>
		public bool Matches(IRequest request)
		{
			Ensure.ArgumentNotNull(request, "request");
			return Condition == null || Condition(request);
		}
	}
}