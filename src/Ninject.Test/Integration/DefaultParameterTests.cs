#if !SILVERLIGHT
namespace Ninject.Tests.Integration
{
    using Ninject.Tests.Fakes;
#if SILVERLIGHT
#if SILVERLIGHT_MSTEST
    using MsTest.Should;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Assert = AssertWithThrows;
    using Fact = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
#else
    using UnitDriven;
    using UnitDriven.Should;
    using Assert = AssertWithThrows;
    using Fact = UnitDriven.TestMethodAttribute;
#endif
#else
    using Ninject.Tests.MSTestAttributes;
    using Xunit;
    using Xunit.Should;
#endif

    [TestClass]
    public class DefaultParameterTests
    {
        [Fact]
        public void DefaultValueShouldBeUsedWhenNoneSupplied()
        {
            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<Shield>().ToSelf();

                var shield = kernel.Get<Shield>();
                shield.ShouldNotBeNull();
                shield.Color.ShouldBe(ShieldColor.Red);
            }
        }

        [Fact]
        public void SpecificValueShouldBeUsedWhenMapped()
        {
            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<Shield>().ToSelf();
                kernel.Bind<ShieldColor>().ToConstant(ShieldColor.Blue);

                var shield = kernel.Get<Shield>();
                shield.ShouldNotBeNull();
                shield.Color.ShouldBe(ShieldColor.Blue);
            }
        }

        [Fact]
        public void SpecificValueShouldBeUsedWhenSupplied()
        {
            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<Shield>().ToSelf().WithConstructorArgument("color", ShieldColor.Orange);

                var shield = kernel.Get<Shield>();
                shield.ShouldNotBeNull();
                shield.Color.ShouldBe(ShieldColor.Orange);
            }
        }

        [Fact]
        public void DefaultValuesShouldNotInflunceInjectionsToOtherTypes()
        {
            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<Shield>().ToSelf();
                kernel.Bind<KiteShield>().ToSelf();

                var shield1 = kernel.Get<Shield>();
                var shield2 = kernel.Get<KiteShield>();

                shield1.ShouldNotBeNull();
                shield1.Color.ShouldBe(ShieldColor.Red);

                shield2.ShouldNotBeNull();
                shield2.Color.ShouldBe(ShieldColor.Orange);
            }
        }
    }
}
#endif