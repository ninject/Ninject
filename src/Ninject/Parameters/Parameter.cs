using System;
using Ninject.Activation;

namespace Ninject.Parameters
{
	public class Parameter : IParameter
	{
		private readonly Func<IContext, object> _valueCallback;

		public string Name { get; private set; }

		public Parameter(string name, object value) : this(name, ctx => value) { }

		public Parameter(string name, Func<IContext, object> valueCallback)
		{
			Name = name;
			_valueCallback = valueCallback;
		}

		public object GetValue(IContext context)
		{
			return _valueCallback(context);
		}
	}
}