#if !SILVERLIGHT
namespace Ninject.Tests.Integration
{
    using FluentAssertions;
    using Ninject.Tests.Fakes;
    using Xunit;

#if MSTEST
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
#endif
    public class DefaultParameterTests
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

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void DefaultValueShouldBeUsedWhenNoneSupplied()
        {
            kernel.Bind<Shield>().ToSelf();

            var shield = kernel.Get<Shield>();
            shield.Should().NotBeNull();
            shield.Color.Should().Be(ShieldColor.Red);
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void SpecificValueShouldBeUsedWhenMapped()
        {
            kernel.Bind<Shield>().ToSelf();
            kernel.Bind<ShieldColor>().ToConstant(ShieldColor.Blue);

            var shield = kernel.Get<Shield>();
            shield.Should().NotBeNull();
            shield.Color.Should().Be(ShieldColor.Blue);
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void SpecificValueShouldBeUsedWhenSupplied()
        {
            kernel.Bind<Shield>().ToSelf().WithConstructorArgument("color", ShieldColor.Orange);

            var shield = kernel.Get<Shield>();
            shield.Should().NotBeNull();
            shield.Color.Should().Be(ShieldColor.Orange);
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void DefaultValuesShouldNotInflunceInjectionsToOtherTypes()
        {
            kernel.Bind<Shield>().ToSelf();
            kernel.Bind<KiteShield>().ToSelf();

            var shield1 = kernel.Get<Shield>();
            var shield2 = kernel.Get<KiteShield>();

            shield1.Should().NotBeNull();
            shield1.Color.Should().Be(ShieldColor.Red);

            shield2.Should().NotBeNull();
            shield2.Color.Should().Be(ShieldColor.Orange);
        }
    }
}
#endif