using System;
using Ninject.Activation;

namespace Ninject.Parameters
{
	/// <summary>
	/// Overrides the injected value of a constructor argument.
	/// </summary>
	public class ConstructorArgument : Parameter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConstructorArgument"/> class.
		/// </summary>
		/// <param name="name">The name of the argument to override.</param>
		/// <param name="value">The value to inject into the property.</param>
		public ConstructorArgument(string name, object value) : base(name, value) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ConstructorArgument"/> class.
		/// </summary>
		/// <param name="name">The name of the argument to override.</param>
		/// <param name="valueCallback">The callback to invoke to get the value that should be injected.</param>
		public ConstructorArgument(string name, Func<IContext, object> valueCallback) : base(name, valueCallback) { }
	}
}