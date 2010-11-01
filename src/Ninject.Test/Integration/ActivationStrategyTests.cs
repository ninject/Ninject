namespace Ninject.Tests.Integration
{
    using System.Linq;
    using Ninject.Activation.Strategies;
    using Ninject.Tests.Fakes;
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

    [TestClass]
    public class ActivationStrategyTests
    {
        [Fact]
        public void InstanceIsActivatedOnCreation()
        {
            using ( var kernel = new StandardKernel() )
            {
                kernel.Bind<Barracks>()
                    .ToSelf()
                    .OnActivation(instance =>
                                    {
                                        instance.Warrior = new FootSoldier();
                                        instance.Weapon = new Shuriken();
                                    });

                var barracks = kernel.Get<Barracks>();
                barracks.Warrior.ShouldNotBeNull();
                barracks.Warrior.ShouldBeInstanceOf<FootSoldier>();
                barracks.Weapon.ShouldNotBeNull();
                barracks.Weapon.ShouldBeInstanceOf<Shuriken>();
            }
        }

        [Fact]
        public void InstanceIsActivatedOnCreationWithContext()
        {
            using (var kernel = new StandardKernel())
            {
                kernel.Bind<Barracks>()
                    .ToSelf()
                    .OnActivation((ctx, instance) =>
                    {
                        instance.Warrior = new FootSoldier();
                        instance.Weapon = new Shuriken();
                    });

                var barracks = kernel.Get<Barracks>();
                barracks.Warrior.ShouldNotBeNull();
                barracks.Warrior.ShouldBeInstanceOf<FootSoldier>();
                barracks.Weapon.ShouldNotBeNull();
                barracks.Weapon.ShouldBeInstanceOf<Shuriken>();
            }
        }

        [Fact]
        public void InstanceIsDeactivatedWhenItLeavesScope()
        {
            Barracks barracks;
            using ( var kernel = new StandardKernel() )
            {
                kernel.Bind<Barracks>()
                    .ToSelf()
                    .InSingletonScope()
                    .OnActivation(instance =>
                                    {
                                        instance.Warrior = new FootSoldier();
                                        instance.Weapon = new Shuriken();
                                    })
                    .OnDeactivation(instance =>
                                    {
                                        instance.Warrior = null;
                                        instance.Weapon = null;
                                    });

                barracks = kernel.Get<Barracks>();
                barracks.Warrior.ShouldNotBeNull();
                barracks.Warrior.ShouldBeInstanceOf<FootSoldier>();
                barracks.Weapon.ShouldNotBeNull();
                barracks.Weapon.ShouldBeInstanceOf<Shuriken>();
            }
            barracks.Warrior.ShouldBeNull();
            barracks.Weapon.ShouldBeNull();
        }

        [Fact]
        public void InstanceIsDeactivatedWhenItLeavesScopeWithContext()
        {
            Barracks barracks;
            using (var kernel = new StandardKernel())
            {
                kernel.Bind<Barracks>()
                    .ToSelf()
                    .InSingletonScope()
                    .OnActivation((ctx, instance) =>
                    {
                        instance.Warrior = new FootSoldier();
                        instance.Weapon = new Shuriken();
                    })
                    .OnDeactivation(instance =>
                    {
                        instance.Warrior = null;
                        instance.Weapon = null;
                    });

                barracks = kernel.Get<Barracks>();
                barracks.Warrior.ShouldNotBeNull();
                barracks.Warrior.ShouldBeInstanceOf<FootSoldier>();
                barracks.Weapon.ShouldNotBeNull();
                barracks.Weapon.ShouldBeInstanceOf<Shuriken>();
            }
            barracks.Warrior.ShouldBeNull();
            barracks.Weapon.ShouldBeNull();
        }

        [Fact]
        public void ObjectsActivatedOnlyOnce()
        {
            using (var kernel = new StandardKernel())
            {
                kernel.Components.Add<IActivationStrategy, TestActivationStrategy>();
                kernel.Bind<IWarrior>().To<Samurai>();
                kernel.Bind<Sword>().ToSelf();
                kernel.Bind<IWeapon>().ToMethod(ctx => ctx.Kernel.Get<Sword>());
                var testActivationStrategy = kernel.Components.GetAll<IActivationStrategy>().OfType<TestActivationStrategy>().Single();

                kernel.Get<IWarrior>();
                
                testActivationStrategy.ActivationCount.ShouldBe(2);
            }            
        }
        
        [Fact]
        public void NullIsNotActivated()
        {
            using (var kernel = new StandardKernel(new NinjectSettings { AllowNullInjection = true }))
            {
                kernel.Components.Add<IActivationStrategy, TestActivationStrategy>();
                kernel.Bind<IWarrior>().To<Samurai>();
                kernel.Bind<IWeapon>().ToConstant(null);
                var testActivationStrategy = kernel.Components.GetAll<IActivationStrategy>().OfType<TestActivationStrategy>().Single();

                kernel.Get<IWarrior>();

                testActivationStrategy.ActivationCount.ShouldBe(1);
            }
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