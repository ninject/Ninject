using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject.Modules;

namespace Ninject.Web.Mvc
{
	[IgnoreModule]
	public class MvcModule : Module
	{
		public override void Load()
		{
			Bind<RouteCollection>().ToConstant(RouteTable.Routes);
			Bind<IControllerFactory>().To<NinjectControllerFactory>().InSingletonScope();
			Bind<IControllerRegistry>().To<ControllerRegistry>().InSingletonScope();
			Bind<IControllerNamer>().To<ControllerNamer>().InSingletonScope();
			Bind<HttpContext>().ToProvider<HttpContextProvider>().InRequestScope();
			Bind<HttpContextBase>().ToProvider<HttpContextBaseProvider>().InRequestScope();
		}
	}
}