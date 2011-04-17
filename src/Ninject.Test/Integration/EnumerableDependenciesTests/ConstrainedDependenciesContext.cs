namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
    using Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes;
    using Xunit.Should;

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