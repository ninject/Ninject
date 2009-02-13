using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Planning.Targets;

namespace Ninject.Activation
{
	/// <summary>
	/// Describes the request for a service resolution.
	/// </summary>
	public class Request : IRequest
	{
		/// <summary>
		/// Gets the service that was requested.
		/// </summary>
		public Type Service { get; set; }

		/// <summary>
		/// Gets the parent request.
		/// </summary>
		public IRequest Parent { get; set; }

		/// <summary>
		/// Gets the target that will receive the injection, if any.
		/// </summary>
		public ITarget Target { get; set; }

		/// <summary>
		/// Gets the constraints that will be applied to filter the bindings used for the request.
		/// </summary>
		public ICollection<Func<IBindingMetadata, bool>> Constraints { get; set; }

		/// <summary>
		/// Gets the parameters that affect the resolution.
		/// </summary>
		public ICollection<IParameter> Parameters { get; set; }

		/// <summary>
		/// Gets or sets the callback that resolves the scope for the request, if an external scope was provided.
		/// </summary>
		public Func<object> ScopeCallback { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Request"/> class.
		/// </summary>
		/// <param name="service">The service that was requested.</param>
		/// <param name="constraints">The constraints that will be applied to filter the bindings used for the request.</param>
		/// <param name="parameters">The parameters that affect the resolution.</param>
		/// <param name="scopeCallback">The scope callback, if an external scope was specified.</param>
		public Request(Type service, IEnumerable<Func<IBindingMetadata, bool>> constraints, IEnumerable<IParameter> parameters, Func<object> scopeCallback)
		{
			Service = service;
			Constraints = constraints == null ? new List<Func<IBindingMetadata, bool>>() : constraints.ToList();
			Parameters = parameters == null ? new List<IParameter>() : parameters.ToList();
			ScopeCallback = scopeCallback;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Request"/> class.
		/// </summary>
		/// <param name="parent">The parent request.</param>
		/// <param name="service">The service that was requested.</param>
		/// <param name="target">The target that will receive the injection.</param>
		/// <param name="scopeCallback">The scope callback, if an external scope was specified.</param>
		public Request(IRequest parent, Type service, ITarget target, Func<object> scopeCallback)
		{
			Parent = parent;
			Service = service;
			Target = target;
			Constraints = target.GetConstraints().ToList();
			Parameters = new List<IParameter>();
			ScopeCallback = scopeCallback;
		}

		/// <summary>
		/// Determines whether the specified binding satisfies the constraints defined on this request.
		/// </summary>
		/// <param name="binding">The binding.</param>
		/// <returns><c>True</c> if the binding satisfies the constraints; otherwise <c>false</c>.</returns>
		public bool ConstraintsSatisfiedBy(IBinding binding)
		{
			return Constraints.All(constraint => constraint(binding.Metadata));
		}

		/// <summary>
		/// Gets the scope if one was specified in the request.
		/// </summary>
		/// <returns>The object that acts as the scope.</returns>
		public object GetScope()
		{
			return ScopeCallback == null ? null : ScopeCallback();
		}

		/// <summary>
		/// Creates a child request.
		/// </summary>
		/// <param name="service">The service that is being requested.</param>
		/// <param name="target">The target that will receive the injection.</param>
		/// <returns>The child request.</returns>
		public IRequest CreateChild(Type service, ITarget target)
		{
			return new Request(this, service, target, ScopeCallback);
		}
	}
}