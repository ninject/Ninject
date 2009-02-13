using System;
using System.Collections.Generic;
using System.Reflection;
using Ninject.Activation;
using Ninject.Planning.Bindings;

namespace Ninject.Planning.Targets
{
	/// <summary>
	/// Represents a site on a type where a value can be injected.
	/// </summary>
	public interface ITarget : ICustomAttributeProvider
	{
		/// <summary>
		/// Gets the type of the target.
		/// </summary>
		Type Type { get; }

		/// <summary>
		/// Gets the name of the target.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the member that contains the target.
		/// </summary>
		MemberInfo Member { get; }

		/// <summary>
		/// Reads the constraints from the target.
		/// </summary>
		/// <returns>A series of constraints read from the target.</returns>
		IEnumerable<Func<IBindingMetadata, bool>> GetConstraints();

		/// <summary>
		/// Resolves a value for the target within the specified parent context.
		/// </summary>
		/// <param name="parent">The parent context.</param>
		/// <returns>The resolved value.</returns>
		object ResolveWithin(IContext parent);
	}
}