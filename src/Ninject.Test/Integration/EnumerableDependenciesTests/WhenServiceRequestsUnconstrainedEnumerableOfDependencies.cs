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
    public class WhenServiceRequestsUnconstrainedEnumerableOfDependencies : UnconstrainedDependenciesContext
    {
        [Fact]
        public void ServiceIsInjectedWithEnumeratorOfAllAvailableDependencies()
        {
            this.Kernel.Bind<IParent>().To<RequestsEnumerable>();
            this.Kernel.Bind<IChild>().To<ChildA>();
            this.Kernel.Bind<IChild>().To<ChildB>();

            var parent = this.Kernel.Get<IParent>();

            VerifyInjection(parent);
        }

        [Fact]
        public void EmptyEnumerableIsInjectedWhenNoBindingIsAvailable()
        {
            this.Kernel.Bind<IParent>().To<RequestsEnumerable>();

            var parent = this.Kernel.Get<IParent>();

            parent.ShouldNotBeNull();
            parent.Children.Count.ShouldBe(0);
        }
    }
}