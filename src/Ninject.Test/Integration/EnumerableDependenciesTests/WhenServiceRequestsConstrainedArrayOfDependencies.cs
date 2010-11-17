namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
    using Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes;
#if SILVERLIGHT
#if SILVERLIGHT_MSTEST
    using MsTest.Should;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Fact = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
#else
    using UnitDriven;
    using UnitDriven.Should;
    using Fact = UnitDriven.TestMethodAttribute;
#endif
#else
    using Ninject.Tests.MSTestAttributes;
    using Xunit;
    using Xunit.Should;
#endif

    [TestClass]
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

            parent.ShouldNotBeNull();
            parent.Children.Count.ShouldBe(0);
        }
    }
}