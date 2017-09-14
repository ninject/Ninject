namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
    using FluentAssertions;
    using Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes;
    using Xunit;

    public class WhenServiceRequestsUnconstrainedEnumerableOfDependencies : UnconstrainedDependenciesContext
    {
        [Fact]
        public void ServiceIsInjectedWithEnumeratorOfAllAvailableDependencies()
        {
            this.Configuration.Bind<IParent>().To<RequestsEnumerable>();
            this.Configuration.Bind<IChild>().To<ChildA>();
            this.Configuration.Bind<IChild>().To<ChildB>();

            var parent = this.Configuration.BuildReadOnlyKernel().Get<IParent>();

            VerifyInjection(parent);
        }

        [Fact]
        public void EmptyEnumerableIsInjectedWhenNoBindingIsAvailable()
        {
            this.Configuration.Bind<IParent>().To<RequestsEnumerable>();

            var parent = this.Configuration.BuildReadOnlyKernel().Get<IParent>();

            parent.Should().NotBeNull();
            parent.Children.Count.Should().Be(0);
        }
    }
}