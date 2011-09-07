#if !NO_MOQ
namespace Ninject.Tests.Unit.ComponentContainerTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Moq;
    using Ninject.Components;
    using Ninject.Infrastructure.Disposal;
    using Xunit;

    public class ComponentContainerContext
    {
        protected ComponentContainer container;
        protected Mock<IKernel> kernelMock;

        public ComponentContainerContext()
        {
            this.SetUp();
        }

        public void SetUp()
        {
            this.container = new ComponentContainer();
            this.kernelMock = new Mock<IKernel>();

            this.container.Kernel = this.kernelMock.Object;
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

            service.Should().NotBeNull();
            service.Should().BeOfType<TestServiceA>();
        }

        [Fact]
        public void ReturnsInstanceOfFirstRegisteredImplementation()
        {
            container.Add<ITestService, TestServiceA>();
            container.Add<ITestService, TestServiceB>();

            var service = container.Get<ITestService>();

            service.Should().NotBeNull();
            service.Should().BeOfType<TestServiceA>();
        }

        [Fact]
        public void InjectsEnumeratorOfServicesWhenConstructorArgumentIsIEnumerable()
        {
            container.Add<ITestService, TestServiceA>();
            container.Add<ITestService, TestServiceB>();
            container.Add<IAsksForEnumerable, AsksForEnumerable>();

            var asks = container.Get<IAsksForEnumerable>();

            asks.Should().NotBeNull();
            asks.SecondService.Should().NotBeNull();
            asks.SecondService.Should().BeOfType<TestServiceB>();
        }

        [Fact]
        public void SameInstanceIsReturnedByDefault()
        {
            container.Add<ITestService, TestServiceA>();

            var service1 = container.Get<ITestService>();
            var service2 = container.Get<ITestService>();

            service1.Should().BeSameAs(service2);
        }

        [Fact]
        public void DifferentInstanceAreReturnedForTransients()
        {
            container.AddTransient<ITestService, TestServiceA>();

            var service1 = container.Get<ITestService>();
            var service2 = container.Get<ITestService>();

            service1.Should().NotBeSameAs(service2);
        }
    }

    public class WhenGetAllIsCalledOnComponentContainer : ComponentContainerContext
    {
        [Fact]
        public void ReturnsSeriesWithSingleItem()
        {
            container.Add<ITestService, TestServiceA>();

            var services = container.GetAll<ITestService>().ToList();

            services.Should().NotBeNull();
            services.Count.Should().Be(1);
            services[0].Should().BeOfType<TestServiceA>();
        }

        [Fact]
        public void ReturnsInstanceOfEachRegisteredImplementation()
        {
            container.Add<ITestService, TestServiceA>();
            container.Add<ITestService, TestServiceB>();
            var services = container.GetAll<ITestService>().ToList();

            services.Should().NotBeNull();
            services.Count.Should().Be(2);
            services[0].Should().BeOfType<TestServiceA>();
            services[1].Should().BeOfType<TestServiceB>();
        }

        [Fact]
        public void ReturnsSameInstanceForTwoCallsForSameService()
        {
            container.Add<ITestService, TestServiceA>();

            var service1 = container.Get<ITestService>();
            var service2 = container.Get<ITestService>();

            service1.Should().NotBeNull();
            service2.Should().NotBeNull();
            service1.Should().BeSameAs(service2);
        }
    }

    public class WhenRemoveAllIsCalled : ComponentContainerContext
    {
        [Fact]
        public void RemovesAllMappings()
        {
            container.Add<ITestService, TestServiceA>();

            var service1 = container.Get<ITestService>();
            service1.Should().NotBeNull();

            container.RemoveAll<ITestService>();
            Assert.Throws<InvalidOperationException>(() => container.Get<ITestService>());
        }

        [Fact]
        public void DisposesOfAllInstances()
        {
            container.Add<ITestService, TestServiceA>();
            container.Add<ITestService, TestServiceB>();

            var services = container.GetAll<ITestService>().ToList();
            services.Should().NotBeNull();
            services.Count.Should().Be(2);

            container.RemoveAll<ITestService>();

            services[0].IsDisposed.Should().BeTrue();
            services[1].IsDisposed.Should().BeTrue();
        }
    }

    public class AsksForEnumerable : NinjectComponent, IAsksForEnumerable
    {
        public ITestService SecondService { get; set; }

        public AsksForEnumerable(IEnumerable<ITestService> services)
        {
            SecondService = services.Skip(1).First();
        }
    }

    public interface IAsksForEnumerable : INinjectComponent
    {
        ITestService SecondService { get; set; }
    }

    public class TestServiceA : NinjectComponent, ITestService { }
    public class TestServiceB : NinjectComponent, ITestService { }

    public interface ITestService : INinjectComponent, IDisposableObject { }
}
#endif