namespace Ninject.Tests.Integration
{
    using System;
    using FluentAssertions;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class DefaultParameterTests : IDisposable
    {
        private readonly StandardKernel kernel;

        public DefaultParameterTests()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }

        [Fact]
        public void DefaultValueShouldBeUsedWhenNoneSupplied()
        {
            this.kernel.Bind<Shield>().ToSelf();

            var shield = this.kernel.Get<Shield>();
            shield.Should().NotBeNull();
            shield.Color.Should().Be(ShieldColor.Red);
        }

        [Fact]
        public void SpecificValueShouldBeUsedWhenMapped()
        {
            this.kernel.Bind<Shield>().ToSelf();
            this.kernel.Bind<ShieldColor>().ToConstant(ShieldColor.Blue);

            var shield = this.kernel.Get<Shield>();
            shield.Should().NotBeNull();
            shield.Color.Should().Be(ShieldColor.Blue);
        }

        [Fact]
        public void SpecificValueShouldBeUsedWhenSupplied()
        {
            this.kernel.Bind<Shield>().ToSelf().WithConstructorArgument("color", ShieldColor.Orange);

            var shield = this.kernel.Get<Shield>();
            shield.Should().NotBeNull();
            shield.Color.Should().Be(ShieldColor.Orange);
        }

        [Fact]
        public void DefaultValuesShouldNotInfluenceInjectionsToOtherTypes()
        {
            this.kernel.Bind<Shield>().ToSelf();
            this.kernel.Bind<KiteShield>().ToSelf();

            var shield1 = this.kernel.Get<Shield>();
            var shield2 = this.kernel.Get<KiteShield>();

            shield1.Should().NotBeNull();
            shield1.Color.Should().Be(ShieldColor.Red);

            shield2.Should().NotBeNull();
            shield2.Color.Should().Be(ShieldColor.Orange);
        }
    }
}