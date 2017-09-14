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
            this.Configuration.Bind<IParent>().To<RequestsConstrainedList>();
            this.Configuration.Bind<IChild>().To<ChildA>().Named("joe");
            this.Configuration.Bind<IChild>().To<ChildB>().Named("bob");

            var parent = this.Configuration.BuildReadOnlyKernel().Get<IParent>();

            VerifyInjection(parent);
        }

        [Fact]
        public void WhenNoMatchingBindingExistsEmptyEnumerableIsInjected()
        {
            this.Configuration.Bind<IParent>().To<RequestsConstrainedList>();
            this.Configuration.Bind<IChild>().To<ChildA>().Named("joe");
            this.Configuration.Bind<IChild>().To<ChildB>().Named("ian");

            var parent = this.Configuration.BuildReadOnlyKernel().Get<IParent>();

            parent.Should().NotBeNull();
            parent.Children.Count.Should().Be(0);
        }
    }
}