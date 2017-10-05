namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes;
    using Xunit;

    public class WhenServiceRequestsUnconstrainedListOfConcreteDependencies : UnconstrainedDependenciesContext
    {
        [Fact]
        public void ServiceIsInjectedWithEmptyListIfElementTypeIsMissingBinding()
        {
            this.Kernel.Bind<RequestsListWithConcreteClass>().ToSelf();

            var parent = this.Kernel.Get<RequestsListWithConcreteClass>();

            parent.Children.Should().BeEmpty();
        }

        [Fact]
        public void ServiceIsInjectedWithNonEmptyListIfElementTypeIsExplicitlyBinded()
        {
            this.Kernel.Bind<RequestsListWithConcreteClass>().ToSelf();
            this.Kernel.Bind<ChildA>().ToSelf();

            var parent = this.Kernel.Get<RequestsListWithConcreteClass>();

            parent.Children.Should().NotBeEmpty();
        }

        [Fact]
        public void NonEmptyListIsResolvedIfElementTypeIsExplicitlyBinded()
        {
            this.Kernel.Bind<ChildA>().ToSelf();

            var children = this.Kernel.Get<IList<ChildA>>();

            children.Should().NotBeEmpty();
        }

        [Fact]
        public void EmptyListIsResolvedIfElementTypeIsMissingBinding()
        {
            var children = this.Kernel.Get<IList<ChildA>>();

            children.Should().BeEmpty();
        }
    }
}