using System;

namespace Ninject.Activation
{
	/// <summary>
	/// Creates instances of services.
	/// </summary>
	public interface IProvider
	{
		/// <summary>
		/// Gets the type (or prototype) of instances the provider creates.
		/// </summary>
		Type Type { get; }

		/// <summary>
		/// Creates an instance within the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The created instance.</returns>
		object Create(IContext context);
	}
}