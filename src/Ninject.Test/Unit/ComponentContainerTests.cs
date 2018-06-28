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

            this.container.KernelConfiguration = this.kernelMock.Object;
        }
    }

    public class WhenGetIsCalled : ComponentContainerContext
    {
        [Fact]
        public void ThrowsExceptionIfNoImplementationRegisteredForService()
        {
            Assert.Throws<InvalidOperationException>(() => this.container.Get<ITestService>());
        }

        [Fact]
        public void ReturnsInstanceWhenOneImplementationIsRegistered()
        {
            this.container.Add<ITestService, TestServiceA>();

            var service = this.container.Get<ITestService>();

            service.Should().NotBeNull();
            service.Should().BeOfType<TestServiceA>();
        }

        [Fact]
        public void ReturnsInstanceOfFirstRegisteredImplementation()
        {
            this.container.Add<ITestService, TestServiceA>();
            this.container.Add<ITestService, TestServiceB>();

            var service = this.container.Get<ITestService>();

            service.Should().NotBeNull();
            service.Should().BeOfType<TestServiceA>();
        }

        [Fact]
        public void InjectsEnumeratorOfServicesWhenConstructorArgumentIsIEnumerable()
        {
            this.container.Add<ITestService, TestServiceA>();
            this.container.Add<ITestService, TestServiceB>();
            this.container.Add<IAsksForEnumerable, AsksForEnumerable>();

            var asks = this.container.Get<IAsksForEnumerable>();

            asks.Should().NotBeNull();
            asks.SecondService.Should().NotBeNull();
            asks.SecondService.Should().BeOfType<TestServiceB>();
        }

        [Fact]
        public void SameInstanceIsReturnedByDefault()
        {
            this.container.Add<ITestService, TestServiceA>();

            var service1 = this.container.Get<ITestService>();
            var service2 = this.container.Get<ITestService>();

            service1.Should().BeSameAs(service2);
        }

        [Fact]
        public void DifferentInstanceAreReturnedForTransients()
        {
            this.container.AddTransient<ITestService, TestServiceA>();

            var service1 = this.container.Get<ITestService>();
            var service2 = this.container.Get<ITestService>();

            service1.Should().NotBeSameAs(service2);
        }
    }

    public class WhenGetAllIsCalledOnComponentContainer : ComponentContainerContext
    {
        [Fact]
        public void ReturnsSeriesWithSingleItem()
        {
            this.container.Add<ITestService, TestServiceA>();

            var services = this.container.GetAll<ITestService>().ToList();

            services.Should().NotBeNull();
            services.Count.Should().Be(1);
            services[0].Should().BeOfType<TestServiceA>();
        }

        [Fact]
        public void ReturnsInstanceOfEachRegisteredImplementation()
        {
            this.container.Add<ITestService, TestServiceA>();
            this.container.Add<ITestService, TestServiceB>();
            var services = this.container.GetAll<ITestService>().ToList();

            services.Should().NotBeNull();
            services.Count.Should().Be(2);
            services[0].Should().BeOfType<TestServiceA>();
            services[1].Should().BeOfType<TestServiceB>();
        }

        [Fact]
        public void ReturnsSameInstanceForTwoCallsForSameService()
        {
            this.container.Add<ITestService, TestServiceA>();

            var service1 = this.container.Get<ITestService>();
            var service2 = this.container.Get<ITestService>();

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
            this.container.Add<ITestService, TestServiceA>();

            var service1 = this.container.Get<ITestService>();
            service1.Should().NotBeNull();

            this.container.RemoveAll<ITestService>();
            Assert.Throws<InvalidOperationException>(() => this.container.Get<ITestService>());
        }

        [Fact]
        public void DisposesOfAllInstances()
        {
            this.container.Add<ITestService, TestServiceA>();
            this.container.Add<ITestService, TestServiceB>();

            var services = this.container.GetAll<ITestService>().ToList();
            services.Should().NotBeNull();
            services.Count.Should().Be(2);

            this.container.RemoveAll<ITestService>();

            services[0].IsDisposed.Should().BeTrue();
            services[1].IsDisposed.Should().BeTrue();
        }
    }

    public class AsksForEnumerable : NinjectComponent, IAsksForEnumerable
    {
        public ITestService SecondService { get; set; }

        public AsksForEnumerable(IEnumerable<ITestService> services)
        {
            this.SecondService = services.Skip(1).First();
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