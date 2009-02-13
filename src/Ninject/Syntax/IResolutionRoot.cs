using System;
using System.Collections.Generic;
using Ninject.Activation;
using Ninject.Activation.Hooks;
using Ninject.Parameters;
using Ninject.Planning.Bindings;

namespace Ninject.Syntax
{
	/// <summary>
	/// Provides a path to resolve instances.
	/// </summary>
	public interface IResolutionRoot : IServiceProvider
	{
		/// <summary>
		/// Determines whether the specified request can be resolved.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns><c>True</c> if the request can be resolved; otherwise, <c>false</c>.</returns>
		bool CanResolve(IRequest request);

		/// <summary>
		/// Resolves the specified request.
		/// </summary>
		/// <param name="service">The service to resolve.</param>
		/// <param name="constraints">The constraints to apply to the bindings to determine if they match the request.</param>
		/// <param name="parameters">The parameters to pass to the resolution.</param>
		/// <returns>A series of hooks that can be used to resolve instances that match the request.</returns>
		IEnumerable<IHook> Resolve(Type service, IEnumerable<Func<IBindingMetadata, bool>> constraints, IEnumerable<IParameter> parameters);

		/// <summary>
		/// Resolves the specified request.
		/// </summary>
		/// <param name="request">The request to resolve.</param>
		/// <returns>A series of hooks that can be used to resolve instances that match the request.</returns>
		IEnumerable<IHook> Resolve(IRequest request);
	}
}