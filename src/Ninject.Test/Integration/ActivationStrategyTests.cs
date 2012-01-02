namespace Ninject.Tests.Integration
{
    using System.Linq;
    using FluentAssertions;

    using Ninject.Activation.Caching;
    using Ninject.Activation.Strategies;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class ActivationStrategyTests
    {
        private readonly StandardKernel kernel;

        public ActivationStrategyTests()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }

        [Fact]
        public void InstanceIsActivatedOnCreation()
        {
            kernel.Bind<Barracks>().ToSelf().OnActivation(
                instance =>
                    {
                        instance.Warrior = new FootSoldier();
                        instance.Weapon = new Shuriken();
                    });

            var barracks = kernel.Get<Barracks>();
            barracks.Warrior.Should().NotBeNull();
            barracks.Warrior.Should().BeOfType<FootSoldier>();
            barracks.Weapon.Should().NotBeNull();
            barracks.Weapon.Should().BeOfType<Shuriken>();
        }

        [Fact]
        public void InstanceIsActivatedOnCreationWithContext()
        {
            kernel.Bind<Barracks>().ToSelf().OnActivation(
                (ctx, instance) =>
                    {
                        instance.Warrior = new FootSoldier();
                        instance.Weapon = new Shuriken();
                    });

            var barracks = kernel.Get<Barracks>();
            barracks.Warrior.Should().NotBeNull();
            barracks.Warrior.Should().BeOfType<FootSoldier>();
            barracks.Weapon.Should().NotBeNull();
            barracks.Weapon.Should().BeOfType<Shuriken>();
        }

        [Fact]
        public void InstanceIsDeactivatedWhenItLeavesScope()
        {
            Barracks barracks;
            kernel.Bind<Barracks>().ToSelf().InSingletonScope()
                  .OnActivation(
                      instance =>
                      {
                          instance.Warrior = new FootSoldier();
                          instance.Weapon = new Shuriken();
                      })
                  .OnDeactivation(
                      instance =>
                      {
                          instance.Warrior = null;
                          instance.Weapon = null;
                      });

            barracks = kernel.Get<Barracks>();
            barracks.Warrior.Should().BeOfType<FootSoldier>();
            barracks.Weapon.Should().BeOfType<Shuriken>();

            kernel.Components.Get<ICache>().Release(barracks);
            barracks.Warrior.Should().BeNull();
            barracks.Weapon.Should().BeNull();
        }

        [Fact]
        public void InstanceIsDeactivatedWhenItLeavesScopeWithContext()
        {
            Barracks barracks;
            kernel.Bind<Barracks>().ToSelf().InSingletonScope()
                  .OnActivation(
                      instance =>
                      {
                          instance.Warrior = new FootSoldier();
                          instance.Weapon = new Shuriken();
                      })
                  .OnDeactivation(
                      instance =>
                      {
                          instance.Warrior = null;
                          instance.Weapon = null;
                      });

            barracks = kernel.Get<Barracks>();
            barracks.Warrior.Should().BeOfType<FootSoldier>();
            barracks.Weapon.Should().BeOfType<Shuriken>();

            kernel.Components.Get<ICache>().Release(barracks);
            barracks.Warrior.Should().BeNull();
            barracks.Weapon.Should().BeNull();
        }

        [Fact]
        public void ObjectsActivatedOnlyOnce()
        {
            kernel.Components.Add<IActivationStrategy, TestActivationStrategy>();
            kernel.Bind<IWarrior>().To<Samurai>();
            kernel.Bind<Sword>().ToSelf();
            kernel.Bind<IWeapon>().ToMethod(ctx => ctx.Kernel.Get<Sword>());
            var testActivationStrategy = kernel.Components.GetAll<IActivationStrategy>().OfType<TestActivationStrategy>().Single();

            kernel.Get<IWarrior>();

            testActivationStrategy.ActivationCount.Should().Be(2);
        }

        [Fact]
        public void NullIsNotActivated()
        {
            kernel.Settings.AllowNullInjection = true;
            kernel.Components.Add<IActivationStrategy, TestActivationStrategy>();
            kernel.Bind<IWarrior>().To<Samurai>();
            kernel.Bind<IWeapon>().ToConstant((IWeapon)null);
            var testActivationStrategy = kernel.Components.GetAll<IActivationStrategy>().OfType<TestActivationStrategy>().Single();

            kernel.Get<IWarrior>();

            testActivationStrategy.ActivationCount.Should().Be(1);
        }

        public class TestActivationStrategy : ActivationStrategy
        {
            private int activationCount = 0;

            public int ActivationCount
            {
                get
                {
                    return this.activationCount;
                }
            }

            public override void Activate(Activation.IContext context, Activation.InstanceReference reference)
            {
                this.activationCount++;
                base.Activate(context, reference);
            }
        }
    }
}