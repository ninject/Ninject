using System;
using System.Collections.Generic;
using Ninject.Activation;
using Ninject.Infrastructure.Tracing;
using Ninject.Parameters;

namespace Ninject.Planning.Bindings
{
	/// <summary>
	/// Contains information about a service registration.
	/// </summary>
	public interface IBinding : IHaveTraceInfo
	{
		/// <summary>
		/// Gets the service type that is controlled by the binding.
		/// </summary>
		Type Service { get; }

		/// <summary>
		/// Gets the binding's metadata.
		/// </summary>
		IBindingMetadata Metadata { get; }

		/// <summary>
		/// Gets the conditions defined for the binding.
		/// </summary>
		ICollection<Func<IRequest, bool>> Conditions { get; }

		/// <summary>
		/// Gets the parameters defined for the binding.
		/// </summary>
		ICollection<IParameter> Parameters { get; }

		/// <summary>
		/// Gets the actions that should be called after instances are activated via the binding.
		/// </summary>
		ICollection<Action<IContext>> ActivationActions { get; }

		/// <summary>
		/// Gets the actions that should be called before instances are deactivated via the binding.
		/// </summary>
		ICollection<Action<IContext>> DeactivationActions { get; }

		/// <summary>
		/// Gets the provider for the binding.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The provider to use.</returns>
		IProvider GetProvider(IContext context);

		/// <summary>
		/// Gets the scope for the binding, if any.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The object that will act as the scope, or <see langword="null"/> if the service is transient.</returns>
		object GetScope(IContext context);

		/// <summary>
		/// Determines whether the specified request satisfies the conditions defined on this binding.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns><c>True</c> if the request satisfies the conditions; otherwise <c>false</c>.</returns>
		bool ConditionsSatisfiedBy(IRequest request);
	}
}