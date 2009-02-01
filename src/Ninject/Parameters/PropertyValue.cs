using System;
using Ninject.Activation;

namespace Ninject.Parameters
{
	public class PropertyValue : Parameter
	{
		public PropertyValue(string name, object value) : base(name, value) { }
		public PropertyValue(string name, Func<IContext, object> valueCallback) : base(name, valueCallback) { }
	}
}