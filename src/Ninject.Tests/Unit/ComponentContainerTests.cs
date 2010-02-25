using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Ninject.Components;
using Ninject.Infrastructure.Disposal;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Unit.ComponentContainerTests
{
	public class ComponentContainerContext
	{
		protected readonly ComponentContainer container;
		protected readonly Mock<IKernel> kernelMock;

		public ComponentContainerContext()
		{
			container = new ComponentContainer();
			kernelMock = new Mock<IKernel>();

			container.Kernel = kernelMock.Object;
		}
	}

	public class WhenGetIsCalled : ComponentContainerContext
	{
		[Fact]
		public void ThrowsExceptionIfNoImplementationRegisteredForService()
		{
			Assert.Throws<InvalidOperationException>(() => container.Get<ITestService>());
		}

		[Fact]
		public void ReturnsInstanceWhenOneImplementationIsRegistered()
		{
			container.Add<ITestService, TestServiceA>();

			var service = container.Get<ITestService>();

			service.ShouldNotBeNull();
			service.ShouldBeInstanceOf<TestServiceA>();
		}

		[Fact]
		public void ReturnsInstanceOfFirstRegisteredImplementation()
		{
			container.Add<ITestService, TestServiceA>();
			container.Add<ITestService, TestServiceB>();

			var service = container.Get<ITestService>();

			service.ShouldNotBeNull();
			service.ShouldBeInstanceOf<TestServiceA>();
		}

		[Fact]
		public void InjectsEnumeratorOfServicesWhenConstructorArgumentIsIEnumerable()
		{
			container.Add<ITestService, TestServiceA>();
			container.Add<ITestService, TestServiceB>();
			container.Add<IAsksForEnumerable, AsksForEnumerable>();

			var asks = container.Get<IAsksForEnumerable>();

			asks.ShouldNotBeNull();
			asks.SecondService.ShouldNotBeNull();
			asks.SecondService.ShouldBeInstanceOf<TestServiceB>();
		}
	}

	public class WhenGetAllIsCalledOnComponentContainer : ComponentContainerContext
	{
		[Fact]
		public void ReturnsSeriesWithSingleItem()
		{
			container.Add<ITestService, TestServiceA>();

			var services = container.GetAll<ITestService>().ToList();

			services.ShouldNotBeNull();
			services.Count.ShouldBe(1);
			services[0].ShouldBeInstanceOf<TestServiceA>();
		}

		[Fact]
		public void ReturnsInstanceOfEachRegisteredImplementation()
		{
			container.Add<ITestService, TestServiceA>();
			container.Add<ITestService, TestServiceB>();
			var services = container.GetAll<ITestService>().ToList();

			services.ShouldNotBeNull();
			services.Count.ShouldBe(2);
			services[0].ShouldBeInstanceOf<TestServiceA>();
			services[1].ShouldBeInstanceOf<TestServiceB>();
		}

		[Fact]
		public void ReturnsSameInstanceForTwoCallsForSameService()
		{
			container.Add<ITestService, TestServiceA>();

			var service1 = container.Get<ITestService>();
			var service2 = container.Get<ITestService>();

			service1.ShouldNotBeNull();
			service2.ShouldNotBeNull();
			service1.ShouldBeSameAs(service2);
		}
	}

	public class WhenRemoveAllIsCalled : ComponentContainerContext
	{
		[Fact]
		public void RemovesAllMappings()
		{
			container.Add<ITestService, TestServiceA>();

			var service1 = container.Get<ITestService>();
			service1.ShouldNotBeNull();

			container.RemoveAll<ITestService>();
			Assert.Throws<InvalidOperationException>(() => container.Get<ITestService>());
		}

		[Fact]
		public void DisposesOfAllInstances()
		{
			container.Add<ITestService, TestServiceA>();
			container.Add<ITestService, TestServiceB>();

			var services = container.GetAll<ITestService>().ToList();
			services.ShouldNotBeNull();
			services.Count.ShouldBe(2);

			container.RemoveAll<ITestService>();

			services[0].IsDisposed.ShouldBeTrue();
			services[1].IsDisposed.ShouldBeTrue();
		}
	}

	internal class AsksForEnumerable : NinjectComponent, IAsksForEnumerable
	{
		public ITestService SecondService { get; set; }

		public AsksForEnumerable(IEnumerable<ITestService> services)
		{
			SecondService = services.Skip(1).First();
		}
	}

	internal interface IAsksForEnumerable : INinjectComponent
	{
		ITestService SecondService { get; set; }
	}

	internal class TestServiceA : NinjectComponent, ITestService { }
	internal class TestServiceB : NinjectComponent, ITestService { }

	internal interface ITestService : INinjectComponent, IDisposableObject { }
}