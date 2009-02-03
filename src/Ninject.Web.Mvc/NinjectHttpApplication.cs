using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Ninject.Web.Mvc
{
	public abstract class NinjectHttpApplication : HttpApplication, IHaveKernel
	{
		private static IKernel _kernel;
		private static readonly object _mutex = new object();

		public IKernel Kernel
		{
			get { return _kernel; }
		}

		public override void Init()
		{
			lock (_mutex)
			{
				_kernel = CreateKernel();
				_kernel.Load(new MvcModule());
				ControllerBuilder.Current.SetControllerFactory(_kernel.Get<IControllerFactory>());
			}
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