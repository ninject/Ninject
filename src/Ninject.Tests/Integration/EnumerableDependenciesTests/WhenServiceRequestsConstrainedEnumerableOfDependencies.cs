namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
    using FluentAssertions;
    using Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes;
    using System.Collections.Generic;
    using Xunit;

    public class WhenServiceRequestsConstrainedEnumerableOfDependencies : ConstrainedDependenciesContext
    {
        [Fact]
        public void ServiceIsInjectedWithAllDependenciesThatMatchTheConstraint()
        {
            this.Kernel.Bind<IParent>().To<RequestsConstrainedEnumerable>();
            this.Kernel.Bind<IChild>().To<ChildA>().Named("joe");
            this.Kernel.Bind<IChild>().To<ChildB>().Named("bob");

            var parent = this.Kernel.Get<IParent>();

            VerifyInjection(parent);
            parent.Children.Should().BeOfType<List<IChild>>();
        }

        [Fact]
        public void WhenNoMatchingBindingExistsEmptyEnumerableIsInjected()
        {
            this.Kernel.Bind<IParent>().To<RequestsConstrainedEnumerable>();
            this.Kernel.Bind<IChild>().To<ChildA>().Named("joe");
            this.Kernel.Bind<IChild>().To<ChildB>().Named("ian");

            var parent = this.Kernel.Get<IParent>();

            parent.Should().NotBeNull();
            parent.Children.Should().BeOfType<List<IChild>>();
            parent.Children.Count.Should().Be(0);
        }
    }
}