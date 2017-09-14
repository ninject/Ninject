namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
    using FluentAssertions;
    using Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes;
    using Xunit;

    public class WhenServiceRequestsUnconstrainedArrayOfDependencies : UnconstrainedDependenciesContext
    {
        [Fact]
        public void ServiceIsInjectedWithArrayOfAllAvailableDependencies()
        {
            this.Configuration.Bind<IParent>().To<RequestsArray>();
            this.Configuration.Bind<IChild>().To<ChildA>();
            this.Configuration.Bind<IChild>().To<ChildB>();

            var parent = this.Configuration.BuildReadOnlyKernel().Get<IParent>();

            VerifyInjection(parent);
        }

        [Fact]
        public void ServiceIsInjectedWithArrayOfAllAvailableDependenciesWhenDefaultCtorIsAvailable()
        {
            this.Configuration.Bind<IParent>().To<RequestsArrayWithDefaultCtor>();
            this.Configuration.Bind<IChild>().To<ChildA>();
            this.Configuration.Bind<IChild>().To<ChildB>();

            var parent = this.Configuration.BuildReadOnlyKernel().Get<IParent>();

            VerifyInjection(parent);
        }

        [Fact]
        public void EmptyArrayIsInjectedWhenNoBindingIsAvailable()
        {
            this.Configuration.Bind<IParent>().To<RequestsArray>();

            var parent = this.Configuration.BuildReadOnlyKernel().Get<IParent>();

            parent.Should().NotBeNull();
            parent.Children.Count.Should().Be(0);
        }
    }
}