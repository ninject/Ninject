using System;
using Ninject.Activation;

namespace Ninject.Parameters
{
	public class Parameter : IParameter
	{
		public string Name { get; private set; }
		public Func<IContext, object> ValueCallback { get; private set; }

		public Parameter(string name, object value) : this(name, ctx => value) { }

		public Parameter(string name, Func<IContext, object> valueCallback)
		{
			Name = name;
			ValueCallback = valueCallback;
		}

		public object GetValue(IContext context)
		{
			return ValueCallback(context);
		}
	}
}