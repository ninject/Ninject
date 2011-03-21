namespace Ninject.Tests.Integration.ConstantTests
{
    using Fakes;
#if SILVERLIGHT
#if SILVERLIGHT_MSTEST
    using MsTest.Should;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Fact = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
#else
    using UnitDriven;
    using UnitDriven.Should;
    using Fact = UnitDriven.TestMethodAttribute;
#endif
#else
    using Ninject.Tests.MSTestAttributes;
    using Xunit;
    using Xunit.Should;
#endif

    public class ConstantContext
    {
        protected StandardKernel kernel;

        public ConstantContext()
        {
            this.SetUp();
        }

        [TestInitialize]
        public void SetUp()
        {
            this.kernel = new StandardKernel();
        }
    }

    [TestClass]
    public class WhenTypeIsBoundToAConstant : ConstantContext
    {
        [Fact]
        public void TheSameInstanceShouldBeReturned()
        {
            var sword = new Sword();
            kernel.Bind<IWeapon>().ToConstant(sword);

            var instance = kernel.Get<IWeapon>();
            instance.ShouldBeSameAs(sword);
        }

        [Fact]
        public void ConditionalBindingShouldNotAffectUnconditionalBinding()
        {
            var sword = new Sword();
            kernel.Bind<IWeapon>().ToConstant(sword).WhenInjectedInto<Samurai>();
            kernel.Bind<IWeapon>().To<Shuriken>();

            var samurai = kernel.Get<Samurai>();
            samurai.Weapon.ShouldBeSameAs(sword);
            var weapon = kernel.Get<IWeapon>();
            weapon.ShouldBeInstanceOf<Shuriken>();
        }

        [Fact]
        public void TheBindingShouldOnlyBeResolvedOnce()
        {
            var builder = kernel.Bind<IWeapon>().ToConstant(new Sword());
            var provider = new ResolveCountingProvider(builder.Binding.ProviderCallback);
            builder.Binding.ProviderCallback = ctx => provider.Callback(ctx);


            kernel.Get<IWeapon>();
            kernel.Get<IWeapon>();

            provider.Count.ShouldBe(1);
        }
    }
}
