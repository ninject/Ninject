using System;
using System.Web;
using System.Web.Mvc;
using Ninject.Modules;

namespace Ninject.Web.Mvc
{
	public class MvcModule : StandardModule
	{
		public override void Load()
		{
			Bind<IControllerFactory>().To<NinjectControllerFactory>().InSingletonScope();
			Bind<IControllerRegistry>().To<ControllerRegistry>().InSingletonScope();
			Bind<IControllerNamer>().To<ControllerNamer>().InSingletonScope();
			Bind<HttpContext>().ToProvider<HttpContextProvider>().InRequestScope();
			Bind<HttpContextBase>().ToProvider<HttpContextBaseProvider>().InRequestScope();
		}
	}
}