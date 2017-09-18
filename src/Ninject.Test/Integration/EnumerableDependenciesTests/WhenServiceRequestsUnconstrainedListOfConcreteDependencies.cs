namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
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
        public void ServiceIsInjectedWithNonEmptyListIfElementTypeIsExplictlyBinded()
        {
            this.Kernel.Bind<RequestsListWithConcreteClass>().ToSelf();
            this.Kernel.Bind<ChildA>().ToSelf();

            var parent = this.Kernel.Get<RequestsListWithConcreteClass>();

            parent.Children.Should().NotBeEmpty();
        }
    }
}