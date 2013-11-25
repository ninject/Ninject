
namespace Ninject.Tests.Integration
{
    using FluentAssertions;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class ConstructorArgumentInBindingConfigurationBuilderTest
    {
        private readonly StandardKernel kernel;

        public ConstructorArgumentInBindingConfigurationBuilderTest()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }

        [Fact]
        public void ConstructorArgumentWithMatchingTypeShouldBeUsed()
        {
            var expectedWeapon = new Dagger();
            this.kernel.Bind<Samurai>().ToSelf().WithConstructorArgument<IWeapon>(expectedWeapon);

            var samurai = this.kernel.Get<Samurai>();

            samurai.Weapon.Should().Be(expectedWeapon);
        }

        [Fact]
        public void ConstructorArgumentWithMatchingTypeShouldBeUsedIfUsingExplicitTypeArgumentSyntax()
        {
            var expectedWeapon = new Dagger();
            this.kernel.Bind<Samurai>().ToSelf().WithConstructorArgument(typeof(IWeapon), expectedWeapon);

            var samurai = this.kernel.Get<Samurai>();

            samurai.Weapon.Should().Be(expectedWeapon);
        }

        [Fact]
        public void ConstructorArgumentWithMatchingTypeShouldBeUsedIfUsingCallbackWithContext()
        {
            var expectedWeapon = new Shuriken();
            this.kernel.Bind<Samurai>().ToSelf().WithConstructorArgument(typeof(IWeapon), context => expectedWeapon);

            var samurai = this.kernel.Get<Samurai>();

            samurai.Weapon.Should().Be(expectedWeapon);
        }

        [Fact]
        public void ConstructorArgumentWithMatchingTypeShouldBeUsedIfUsingCallbackWithContextAndTarget()
        {
            var expectedWeapon = new Shuriken();
            this.kernel.Bind<Samurai>().ToSelf().WithConstructorArgument(typeof(IWeapon), (context, target) => expectedWeapon);

            var samurai = this.kernel.Get<Samurai>();

            samurai.Weapon.Should().Be(expectedWeapon);
        }
    }
}