namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
    using FluentAssertions;
    using Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes;
    using System.Collections.Generic;
    using Xunit;

    public class WhenServiceRequestsUnconstrainedIListOfDependencies : UnconstrainedDependenciesContext
    {
        [Fact]
        public void ServiceIsInjectedWithListOfAllAvailableDependencies()
        {
            this.Kernel.Bind<IParent>().To<RequestsIList>();
            this.Kernel.Bind<IChild>().To<ChildA>();
            this.Kernel.Bind<IChild>().To<ChildB>();

            var parent = this.Kernel.Get<IParent>();

            VerifyInjection(parent);
            parent.Children.Should().BeOfType<List<IChild>>();
        }

        [Fact]
        public void ServiceIsInjectedWithEmptyListIfElementTypeIsMissingBinding()
        {
            this.Kernel.Bind<IParent>().To<RequestsIList>();

            var parent = this.Kernel.Get<IParent>();

            parent.Should().NotBeNull();
            parent.Children.Should().BeEmpty();
            parent.Children.Should().BeOfType<List<IChild>>();
        }

        [Fact]
        public void ListIsResolvedIfElementTypeIsExplicitlyBinded()
        {
            this.Kernel.Bind<IChild>().To<ChildA>();

            var children = this.Kernel.Get<IList<IChild>>();

            children.Should().NotBeEmpty();
            children.Should().BeOfType<List<IChild>>();
        }

        [Fact]
        public void EmptyListIsResolvedIfElementTypeIsMissingBinding()
        {
            var children = this.Kernel.Get<IList<IChild>>();

            children.Should().BeEmpty();
            children.Should().BeOfType<List<IChild>>();
        }
    }
}