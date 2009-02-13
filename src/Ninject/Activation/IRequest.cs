using System;
using System.Collections.Generic;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Planning.Targets;

namespace Ninject.Activation
{
	/// <summary>
	/// Describes the request for a service resolution.
	/// </summary>
	public interface IRequest
	{
		/// <summary>
		/// Gets the service that was requested.
		/// </summary>
		Type Service { get; }

		/// <summary>
		/// Gets the parent request.
		/// </summary>
		IRequest Parent { get; }

		/// <summary>
		/// Gets the target that will receive the injection, if any.
		/// </summary>
		ITarget Target { get; }

		/// <summary>
		/// Gets the constraints that will be applied to filter the bindings used for the request.
		/// </summary>
		ICollection<Func<IBindingMetadata, bool>> Constraints { get; }

		/// <summary>
		/// Gets the parameters that affect the resolution.
		/// </summary>
		ICollection<IParameter> Parameters { get; }

		/// <summary>
		/// Determines whether the specified binding satisfies the constraints defined on this request.
		/// </summary>
		/// <param name="binding">The binding.</param>
		/// <returns><c>True</c> if the binding satisfies the constraints; otherwise <c>false</c>.</returns>
		bool ConstraintsSatisfiedBy(IBinding binding);

		/// <summary>
		/// Gets the scope if one was specified in the request.
		/// </summary>
		/// <returns>The object that acts as the scope.</returns>
		object GetScope();

		/// <summary>
		/// Creates a child request.
		/// </summary>
		/// <param name="service">The service that is being requested.</param>
		/// <param name="target">The target that will receive the injection.</param>
		/// <returns>The child request.</returns>
		IRequest CreateChild(Type service, ITarget target);
	}
}