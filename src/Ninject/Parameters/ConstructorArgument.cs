using System;
using Ninject.Activation;

namespace Ninject.Parameters
{
	public class ConstructorArgument : Parameter
	{
		public ConstructorArgument(string name, object value) : base(name, value) { }
		public ConstructorArgument(string name, Func<IContext, object> valueCallback) : base(name, valueCallback) { }
	}
}