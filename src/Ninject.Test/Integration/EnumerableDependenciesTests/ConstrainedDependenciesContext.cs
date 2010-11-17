namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
    using Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes;
#if SILVERLIGHT
#if SILVERLIGHT_MSTEST
    using MsTest.Should;
#else
    using UnitDriven.Should;
#endif
#else
    using Xunit.Should;
#endif
    public class ConstrainedDependenciesContext : EnumerableDependenciesContext
    {
        protected override void VerifyInjection(IParent parent)
        {
            parent.ShouldNotBeNull();
            parent.Children.ShouldNotBeNull();
            parent.Children.Count.ShouldBe(1);
            parent.Children[0].ShouldBeInstanceOf<ChildB>();
        }
    }
}