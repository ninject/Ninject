using System;
using System.Reflection;
using System.Web.Routing;

namespace Ninject.Web.Mvc
{
	public interface IControllerRegistry
	{
		void RegisterAllControllersIn(string assemblyName);
		void RegisterAllControllersIn(Assembly assembly);

		void Register(string controllerName, Type controllerType);
		Type GetController(RequestContext requestContext, string controllerName);
	}
}