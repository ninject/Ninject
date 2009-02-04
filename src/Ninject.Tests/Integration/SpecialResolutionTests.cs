using System;
using Xunit;

namespace Ninject.Tests.Integration.SpecialResolutionTests
{
	public class SpecialResolutionContext
	{
		protected readonly StandardKernel kernel;

		public SpecialResolutionContext()
		{
			kernel = new StandardKernel();
		}
	}

	public class WhenServiceRequestsKernel : SpecialResolutionContext
	{
		[Fact]
		public void InstanceOfKernelIsInjected()
		{
			kernel.Bind<RequestsKernel>().ToSelf();
			var instance = kernel.Get<RequestsKernel>();

			Assert.NotNull(instance);
			Assert.NotNull(instance.Kernel);
			Assert.Same(kernel, instance.Kernel);
		}
	}

	public class RequestsKernel
	{
		public IKernel Kernel { get; set; }

		public RequestsKernel(IKernel kernel)
		{
			Kernel = kernel;
		}
	}
}