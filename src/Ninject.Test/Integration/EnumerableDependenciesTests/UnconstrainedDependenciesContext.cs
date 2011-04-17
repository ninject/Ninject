namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
    using Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes;
    using Xunit.Should;

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