namespace Ninject.Tests.Integration.ConstantTests
{
    using System;

    using Fakes;
    using FluentAssertions;
    using Xunit;

    public class ConstantContext : IDisposable
    {
        protected StandardKernel kernel;

        public ConstantContext()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }
    }

    public class WhenTypeIsBoundToAConstant : ConstantContext
    {
        [Fact]
        public void TheSameInstanceShouldBeReturned()
        {
            var sword = new Sword();
            kernel.Bind<IWeapon>().ToConstant(sword);

            var instance = kernel.Get<IWeapon>();
            instance.Should().BeSameAs(sword);
        }

        [Fact]
        public void ConditionalBindingShouldNotAffectUnconditionalBinding()
        {
            var sword = new Sword();
            kernel.Bind<IWeapon>().ToConstant(sword).WhenInjectedInto<Samurai>();
            kernel.Bind<IWeapon>().To<Shuriken>();

            var samurai = kernel.Get<Samurai>();
            samurai.Weapon.Should().BeSameAs(sword);
            var weapon = kernel.Get<IWeapon>();
            weapon.Should().BeOfType<Shuriken>();
        }

        [Fact]
        public void TheBindingShouldOnlyBeResolvedOnce()
        {
            var builder = kernel.Bind<IWeapon>().ToConstant(new Sword());
            var provider = new ResolveCountingProvider(builder.BindingConfiguration.ProviderCallback);
            builder.BindingConfiguration.ProviderCallback = ctx => provider.Callback(ctx);


            kernel.Get<IWeapon>();
            kernel.Get<IWeapon>();

            provider.Count.Should().Be(1);
        }
    }
}
