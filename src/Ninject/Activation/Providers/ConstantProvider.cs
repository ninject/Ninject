using System;

namespace Ninject.Activation.Providers
{
	/// <summary>
	/// A provider that always returns the same constant value.
	/// </summary>
	/// <typeparam name="T">The type of value that is returned.</typeparam>
	public class ConstantProvider<T> : Provider<T>
	{
		/// <summary>
		/// Gets the value that the provider will return.
		/// </summary>
		public T Value { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ConstantProvider&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="value">The value that the provider should return.</param>
		public ConstantProvider(T value)
		{
			Value = value;
		}

		/// <summary>
		/// Creates an instance within the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The constant value this provider returns.</returns>
		protected override T CreateInstance(IContext context)
		{
			return Value;
		}
	}
}