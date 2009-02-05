using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Ninject.Infrastructure;

namespace Ninject.Web.Mvc
{
	public abstract class NinjectHttpApplication : HttpApplication, IHaveKernel
	{
		private static IKernel _kernel;

		IKernel IHaveKernel.Kernel
		{
			get { return _kernel; }
		}

		public void Application_Start()
		{
			_kernel = CreateKernel();
			_kernel.Load(new MvcModule());
			ControllerBuilder.Current.SetControllerFactory(_kernel.Get<IControllerFactory>());
		}

		public void RegisterAllControllersIn(string assemblyName)
		{
			_kernel.Get<IControllerRegistry>().RegisterAllControllersIn(assemblyName);
		}

		public void RegisterAllControllersIn(Assembly assembly)
		{
			_kernel.Get<IControllerRegistry>().RegisterAllControllersIn(assembly);
		}

		protected abstract IKernel CreateKernel();
	}
}