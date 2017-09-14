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
            this.Kernel.Bind<IParent>().To<RequestsList>();
            this.Kernel.Bind<IChild>().To<ChildA>();
            this.Kernel.Bind<IChild>().To<ChildB>();

            var parent = this.Kernel.Get<IParent>();

            VerifyInjection(parent);
        }

        [Fact]
        public void ServiceIsInjectedWithListOfAllAvailableDependenciesWhenDefaultCtorIsAvailable()
        {
            this.Kernel.Bind<IParent>().To<RequestsListWithDefaultCtor>();
            this.Kernel.Bind<IChild>().To<ChildA>();
            this.Kernel.Bind<IChild>().To<ChildB>();

            var parent = this.Kernel.Get<IParent>();

            VerifyInjection(parent);
        }

        [Fact]
        public void EmptyListIsInjectedWhenNoBindingIsAvailable()
        {
            this.Kernel.Bind<IParent>().To<RequestsList>();

            var parent = this.Kernel.Get<IParent>();

            parent.Should().NotBeNull();
            parent.Children.Count.Should().Be(0);
        }
    }
}