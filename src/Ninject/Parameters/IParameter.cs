using System;
using Ninject.Activation;

namespace Ninject.Parameters
{
	/// <summary>
	/// Modifies an activation process in some way.
	/// </summary>
	public interface IParameter : IEquatable<IParameter>
	{
		/// <summary>
		/// Gets the name of the parameter.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the value for the parameter within the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The value for the parameter.</returns>
		object GetValue(IContext context);
	}
}