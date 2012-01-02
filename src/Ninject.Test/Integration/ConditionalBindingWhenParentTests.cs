namespace Ninject.Tests.Integration
{
    using System;

    using FluentAssertions;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class WhenParentContext : IDisposable
    {
        protected StandardKernel kernel;

        public WhenParentContext()
        {
            this.kernel = new StandardKernel();
            this.kernel.Bind<Sword>().ToSelf().Named("Broken");
            this.kernel.Bind<Sword>().ToSelf().WhenParentNamed("Something");
        }

        public void Dispose()
        {
            this.kernel = new StandardKernel();
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