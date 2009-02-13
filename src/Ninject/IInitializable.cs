using System;

namespace Ninject
{
	/// <summary>
	/// A service that requires initialization after it is activated.
	/// </summary>
	public interface IInitializable
	{
		/// <summary>
		/// Initializes the instance. Called during activation.
		/// </summary>
		void Initialize();
	}
}
