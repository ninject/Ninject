namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
    using FluentAssertions;
    using Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes;

    public class UnconstrainedDependenciesContext : EnumerableDependenciesContext
    {
        protected override void VerifyInjection(IParent parent)
        {
            parent.Should().NotBeNull();
            parent.Children.Should().NotBeNull();
            parent.Children.Count.Should().Be(2);
            parent.Children[0].Should().BeOfType<ChildA>();
            parent.Children[1].Should().BeOfType<ChildB>();
        }
    }
}