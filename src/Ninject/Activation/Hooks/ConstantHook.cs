using System;

namespace Ninject.Activation.Hooks
{
	/// <summary>
	/// A hook that always returns a constant value.
	/// </summary>
	public class ConstantHook : IHook
	{
		/// <summary>
		/// Gets the value that the hook will return.
		/// </summary>
		public object Value { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ConstantHook"/> class.
		/// </summary>
		/// <param name="value">The value that the hook should return.</param>
		public ConstantHook(object value)
		{
			Value = value;
		}

		/// <summary>
		/// Resolves the instance associated with this hook.
		/// </summary>
		/// <returns>The resolved instance.</returns>
		public object Resolve()
		{
			return Value;
		}
	}
}