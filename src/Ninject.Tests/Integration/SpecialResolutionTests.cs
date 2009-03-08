using System;
using Xunit;
using Xunit.Should;

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

			instance.ShouldNotBeNull();
			instance.Kernel.ShouldNotBeNull();
			instance.Kernel.ShouldBeSameAs(kernel);
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