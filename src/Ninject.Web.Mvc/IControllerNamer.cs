using System;

namespace Ninject.Web.Mvc
{
	public interface IControllerNamer
	{
		string GetNameForController(Type type);
		string NormalizeControllerName(string name);
	}
}