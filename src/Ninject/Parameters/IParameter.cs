using System;
using Ninject.Activation;

namespace Ninject.Parameters
{
	public interface IParameter : IEquatable<IParameter>
	{
		string Name { get; }
		object GetValue(IContext context);
	}
}