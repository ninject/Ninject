using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ninject.Web.Mvc
{
	public class NinjectControllerFactory : IControllerFactory
	{
		public IKernel Kernel { get; private set; }
		public IControllerRegistry Registry { get; private set; }

		public NinjectControllerFactory(IKernel kernel, IControllerRegistry registry)
		{
			Kernel = kernel;
			Registry = registry;
		}

		public IController CreateController(RequestContext requestContext, string controllerName)
		{
			Type type = Registry.GetController(requestContext, controllerName);
			return type == null ? null : Kernel.Get(type) as IController;
		}

		public void ReleaseController(IController controller)
		{
		}
	}
}