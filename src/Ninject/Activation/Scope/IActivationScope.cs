using System;
using Ninject.Infrastructure.Disposal;
using Ninject.Syntax;

namespace Ninject.Activation.Scope
{
	public interface IActivationScope : IResolutionRoot, INotifyWhenDisposed { }
}