using System;

namespace Ninject.Web.Mvc
{
	public class ControllerNamer : IControllerNamer
	{
		public string GetNameForController(Type type)
		{
			string name = type.Name;

			if (name.EndsWith("Controller"))
				name = name.Substring(0, name.IndexOf("Controller"));

			return NormalizeControllerName(name);
		}

		public string NormalizeControllerName(string name)
		{
			return String.IsNullOrEmpty(name) ? name : name.ToLowerInvariant();
		}
	}
}