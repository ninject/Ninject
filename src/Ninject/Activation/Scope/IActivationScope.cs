using System;
using Ninject.Infrastructure.Disposal;
using Ninject.Syntax;

namespace Ninject.Activation.Scope
{
	/// <summary>
	/// A scope used for deterministic disposal of activated instances. When the scope is
	/// disposed, all instances activated via it will be deactivated.
	/// </summary>
	public interface IActivationScope : IResolutionRoot, INotifyWhenDisposed { }
}