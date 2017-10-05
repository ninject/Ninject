namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes;
    using Xunit;

    public class WhenServiceRequestsUnconstrainedArrayOfDependencies : UnconstrainedDependenciesContext
    {
        [Fact]
        public void ServiceIsInjectedWithArrayOfAllAvailableDependencies()
        {
            this.Kernel.Bind<IParent>().To<RequestsArray>();
            this.Kernel.Bind<IChild>().To<ChildA>();
            this.Kernel.Bind<IChild>().To<ChildB>();

            var parent = this.Kernel.Get<IParent>();

            VerifyInjection(parent);
        }

        [Fact]
        public void ServiceIsInjectedWithArrayOfAllAvailableDependenciesWhenDefaultCtorIsAvailable()
        {
            this.Kernel.Bind<IParent>().To<RequestsArrayWithDefaultCtor>();
            this.Kernel.Bind<IChild>().To<ChildA>();
            this.Kernel.Bind<IChild>().To<ChildB>();

            var parent = this.Kernel.Get<IParent>();

            VerifyInjection(parent);
        }

        [Fact]
        public void EmptyArrayIsInjectedWhenNoBindingIsAvailable()
        {
            this.Kernel.Bind<IParent>().To<RequestsArray>();

            var parent = this.Kernel.Get<IParent>();

            parent.Should().NotBeNull();
            parent.Children.Count.Should().Be(0);
        }

        [Fact]
        public void ArrayIsResolvedIfElementTypeIsExplicitlyBinded()
        {
            this.Kernel.Bind<IChild>().To<ChildA>();

            var children = this.Kernel.Get<IChild[]>();

            children.Should().NotBeEmpty();
        }

        [Fact]
        public void EmptyArrayIsResolvedIfElementTypeIsMissingBinding()
        {
            var children = this.Kernel.Get<IChild[]>();

            children.Should().BeEmpty();
        }
    }
}