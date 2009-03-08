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

		public IKernel Kernel
		{
			get { return _kernel; }
		}

		public void Application_Start()
		{
			_kernel = CreateKernel();
			_kernel.LoadModule(new MvcModule());
			ControllerBuilder.Current.SetControllerFactory(_kernel.Get<IControllerFactory>());
			OnApplicationStarted();
		}

		public void Application_Stop()
		{
			if (_kernel != null)
			{
				_kernel.Dispose();
				_kernel = null;
			}

			OnApplicationStopped();
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

		protected virtual void OnApplicationStarted()
		{
		}

		protected virtual void OnApplicationStopped()
		{
		}
	}
}