namespace Ninject.Tests.Integration
{
    using FluentAssertions;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class WhenParentContext
    {
        protected StandardKernel kernel;

        public WhenParentContext()
        {
            this.kernel = new StandardKernel();
            this.kernel.Bind<Sword>().ToSelf().Named("Broken");
            this.kernel.Bind<Sword>().ToSelf().WhenParentNamed("Something");
        }
    }

    public class WhenParentNamed : WhenParentContext
    {
        [Fact]
        public void NamedInstanceAvailableEvenWithWhenBinding()
        {
            var instance = kernel.Get<Sword>("Broken");

            instance.Should().NotBeNull();
            instance.Should().BeOfType<Sword>();
        }
    }
}