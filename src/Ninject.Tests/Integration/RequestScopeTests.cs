#if !NO_WEB
using System;
using System.IO;
using System.Web;
using Ninject.Activation.Caching;
using Ninject.Infrastructure.Disposal;
using Ninject.Tests.Fakes;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Integration.RequestScopeTests
{
	public class RequestScopeContext
	{
		protected readonly StandardKernel kernel;

		public RequestScopeContext()
		{
			var settings = new NinjectSettings { CachePruningInterval = TimeSpan.MaxValue };
			kernel = new StandardKernel(settings);
		}

		public void BeginNewFakeWebRequest()
		{
			HttpContext.Current = GetFakeHttpContext();
		}

		public HttpContext GetFakeHttpContext()
		{
			var request = new HttpRequest("index.html", "http://example.org/index.html", String.Empty);
			var response = new HttpResponse(new StringWriter());
			return new HttpContext(request, response);
		}
	}

	public class WhenServiceIsBoundWithRequestScope : RequestScopeContext
	{
		[Fact]
		public void InstancesAreReusedWithinSameHttpContext()
		{
			kernel.Bind<IWeapon>().To<Sword>().InRequestScope();

			BeginNewFakeWebRequest();

			var weapon1 = kernel.Get<IWeapon>();
			var weapon2 = kernel.Get<IWeapon>();

			weapon1.ShouldBeSameAs(weapon2);

			BeginNewFakeWebRequest();

			GC.Collect();
			GC.WaitForPendingFinalizers();

			var weapon3 = kernel.Get<IWeapon>();

			weapon3.ShouldNotBeSameAs(weapon1);
			weapon3.ShouldNotBeSameAs(weapon2);
		}

		[Fact]
		public void InstancesAreDisposedWhenRequestEndsAndCacheIsPruned()
		{
			kernel.Bind<INotifyWhenDisposed>().To<NotifiesWhenDisposed>().InRequestScope();
			var cache = kernel.Components.Get<ICache>();

			BeginNewFakeWebRequest();

			var instance = kernel.Get<INotifyWhenDisposed>();

			instance.ShouldNotBeNull();
			instance.ShouldBeInstanceOf<NotifiesWhenDisposed>();

			BeginNewFakeWebRequest();

			GC.Collect();
			GC.WaitForPendingFinalizers();

			cache.Prune();

			instance.IsDisposed.ShouldBeTrue();
		}

		[Fact]
		public void InstancesAreDisposedViaOnePerRequestModule()
		{
			kernel.Bind<INotifyWhenDisposed>().To<NotifiesWhenDisposed>().InRequestScope();

			BeginNewFakeWebRequest();

			var instance = kernel.Get<INotifyWhenDisposed>();

			instance.ShouldNotBeNull();
			instance.ShouldBeInstanceOf<NotifiesWhenDisposed>();

			OnePerRequestModule.DeactivateInstancesForCurrentHttpRequest();

			instance.IsDisposed.ShouldBeTrue();
		}
	}
}
#endif //!NO_WEB