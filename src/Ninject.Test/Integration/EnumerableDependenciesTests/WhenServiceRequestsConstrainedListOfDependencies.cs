namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
    using FluentAssertions;
    using Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes;
    using Xunit;

   public class WhenServiceRequestsConstrainedListOfDependencies : ConstrainedDependenciesContext
    {
        [Fact]
        public void ServiceIsInjectedWithAllDependenciesThatMatchTheConstraint()
        {
            this.Kernel.Bind<IParent>().To<RequestsConstrainedList>();
            this.Kernel.Bind<IChild>().To<ChildA>().Named("joe");
            this.Kernel.Bind<IChild>().To<ChildB>().Named("bob");

            var parent = this.Kernel.Get<IParent>();

            VerifyInjection(parent);
        }

        [Fact]
        public void WhenNoMatchingBindingExistsEmptyEmumerableIsInjected()
        {
            this.Kernel.Bind<IParent>().To<RequestsConstrainedList>();
            this.Kernel.Bind<IChild>().To<ChildA>().Named("joe");
            this.Kernel.Bind<IChild>().To<ChildB>().Named("ian");

            var parent = this.Kernel.Get<IParent>();

            parent.Should().NotBeNull();
            parent.Children.Count.Should().Be(0);
        }
    }
}