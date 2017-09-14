namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
    using FluentAssertions;
    using Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes;
    using Xunit;

    public class WhenServiceRequestsUnconstrainedListOfDependencies : UnconstrainedDependenciesContext
    {
        [Fact]
        public void ServiceIsInjectedWithListOfAllAvailableDependencies()
        {
            this.Configuration.Bind<IParent>().To<RequestsList>();
            this.Configuration.Bind<IChild>().To<ChildA>();
            this.Configuration.Bind<IChild>().To<ChildB>();

            var parent = this.Configuration.BuildReadOnlyKernel().Get<IParent>();

            VerifyInjection(parent);
        }

        [Fact]
        public void ServiceIsInjectedWithListOfAllAvailableDependenciesWhenDefaultCtorIsAvailable()
        {
            this.Configuration.Bind<IParent>().To<RequestsListWithDefaultCtor>();
            this.Configuration.Bind<IChild>().To<ChildA>();
            this.Configuration.Bind<IChild>().To<ChildB>();

            var parent = this.Configuration.BuildReadOnlyKernel().Get<IParent>();

            VerifyInjection(parent);
        }

        [Fact]
        public void EmptyListIsInjectedWhenNoBindingIsAvailable()
        {
            this.Configuration.Bind<IParent>().To<RequestsList>();

            var parent = this.Configuration.BuildReadOnlyKernel().Get<IParent>();

            parent.Should().NotBeNull();
            parent.Children.Count.Should().Be(0);
        }
    }
}