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

    public class UnconstrainedDependenciesContext : EnumerableDependenciesContext
    {
        protected override void VerifyInjection(IParent parent)
        {
            parent.ShouldNotBeNull();
            parent.Children.ShouldNotBeNull();
            parent.Children.Count.ShouldBe(2);
            parent.Children[0].ShouldBeInstanceOf<ChildA>();
            parent.Children[1].ShouldBeInstanceOf<ChildB>();
        }
    }
}