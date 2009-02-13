using System;

namespace Ninject.Activation.Hooks
{
	/// <summary>
	/// A hook to resolve an instance via Ninject.
	/// </summary>
	public interface IHook
	{
		/// <summary>
		/// Resolves the instance associated with this hook.
		/// </summary>
		/// <returns>The resolved instance.</returns>
		object Resolve();
	}
}