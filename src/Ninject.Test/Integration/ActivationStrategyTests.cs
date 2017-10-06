namespace Ninject.Tests.Integration
{
    using System;
    using System.Linq;
    using FluentAssertions;

    using Ninject.Activation.Caching;
    using Ninject.Activation.Strategies;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class ActivationStrategyTests : IDisposable
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
            this.kernel.Bind<Barracks>().ToSelf().OnActivation(
                instance =>
                    {
                        instance.Warrior = new FootSoldier();
                        instance.Weapon = new Shuriken();
                    });

            var barracks = this.kernel.Get<Barracks>();
            barracks.Warrior.Should().NotBeNull();
            barracks.Warrior.Should().BeOfType<FootSoldier>();
            barracks.Weapon.Should().NotBeNull();
            barracks.Weapon.Should().BeOfType<Shuriken>();
        }

        [Fact]
        public void InstanceIsActivatedOnCreationWithContext()
        {
            this.kernel.Bind<Barracks>().ToSelf().OnActivation(
                (ctx, instance) =>
                    {
                        instance.Warrior = new FootSoldier();
                        instance.Weapon = new Shuriken();
                    });

            var barracks = this.kernel.Get<Barracks>();
            barracks.Warrior.Should().NotBeNull();
            barracks.Warrior.Should().BeOfType<FootSoldier>();
            barracks.Weapon.Should().NotBeNull();
            barracks.Weapon.Should().BeOfType<Shuriken>();
        }

        [Fact]
        public void InstanceIsDeactivatedWhenItLeavesScope()
        {
            Barracks barracks;
            this.kernel.Bind<Barracks>().ToSelf().InSingletonScope()
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

            barracks = this.kernel.Get<Barracks>();
            barracks.Warrior.Should().BeOfType<FootSoldier>();
            barracks.Weapon.Should().BeOfType<Shuriken>();

            this.kernel.Components.Get<ICache>().Release(barracks);
            barracks.Warrior.Should().BeNull();
            barracks.Weapon.Should().BeNull();
        }

        [Fact]
        public void InstanceIsDeactivatedWhenItLeavesScopeWithContext()
        {
            Barracks barracks;
            this.kernel.Bind<Barracks>().ToSelf().InSingletonScope()
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

            barracks = this.kernel.Get<Barracks>();
            barracks.Warrior.Should().BeOfType<FootSoldier>();
            barracks.Weapon.Should().BeOfType<Shuriken>();

            this.kernel.Components.Get<ICache>().Release(barracks);
            barracks.Warrior.Should().BeNull();
            barracks.Weapon.Should().BeNull();
        }

        [Fact]
        public void ObjectsActivatedOnlyOnce()
        {
            this.kernel.Components.Add<IActivationStrategy, TestActivationStrategy>();
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<Sword>().ToSelf();
            this.kernel.Bind<IWeapon>().ToMethod(ctx => ctx.Kernel.Get<Sword>());
            var testActivationStrategy = this.kernel.Components.GetAll<IActivationStrategy>().OfType<TestActivationStrategy>().Single();

            this.kernel.Get<IWarrior>();

            testActivationStrategy.ActivationCount.Should().Be(2);
        }

        [Fact]
        public void NullIsNotActivated()
        {
            this.kernel.Settings.AllowNullInjection = true;
            this.kernel.Components.Add<IActivationStrategy, TestActivationStrategy>();
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<IWeapon>().ToConstant((IWeapon)null);
            var testActivationStrategy = this.kernel.Components.GetAll<IActivationStrategy>().OfType<TestActivationStrategy>().Single();

            this.kernel.Get<IWarrior>();

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