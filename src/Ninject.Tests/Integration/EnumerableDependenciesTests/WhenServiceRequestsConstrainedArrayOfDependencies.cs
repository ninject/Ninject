namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
    using FluentAssertions;
    using Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes;
    using Xunit;

    public class WhenServiceRequestsConstrainedArrayOfDependencies : ConstrainedDependenciesContext
    {
        [Fact]
        public void ServiceIsInjectedWithAllDependenciesThatMatchTheConstraint()
        {
            this.Kernel.Bind<IParent>().To<RequestsConstrainedArray>();
            this.Kernel.Bind<IChild>().To<ChildA>().Named("joe");
            this.Kernel.Bind<IChild>().To<ChildB>().Named("bob");

            var parent = this.Kernel.Get<IParent>();

            VerifyInjection(parent);
        }

        [Fact]
        public void WhenNoMatchingBindingExistsEmptyArrayIsInjected()
        {
            this.Kernel.Bind<IParent>().To<RequestsConstrainedArray>();
            this.Kernel.Bind<IChild>().To<ChildA>().Named("joe");
            this.Kernel.Bind<IChild>().To<ChildB>().Named("ian");

            var parent = this.Kernel.Get<IParent>();

            parent.Should().NotBeNull();
            parent.Children.Count.Should().Be(0);
        }
    }
}