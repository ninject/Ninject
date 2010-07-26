using Ninject.Tests.Fakes;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Integration
{
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
        public void InstanceIsActivatedOnCreationAndContextIsPassed()
        {
            using (var kernel = new StandardKernel())
            {
                kernel.Bind<Barracks>()
                    .ToSelf()
                    .OnActivation((context, instance) =>
                    {
                        kernel.Bind<IWarrior>().To<FootSoldier>();
                        kernel.Bind<IWeapon>().To<Shuriken>();

                        instance.Warrior = context.Kernel.Get<IWarrior>();
                        instance.Weapon = context.Kernel.Get<IWeapon>();
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
        public void InstanceIsDeactivatedWhenItLeavesScopeAndContextIsPassed()
        {
            Barracks barracks;
            using (var kernel = new StandardKernel())
            {
                kernel.Bind<Barracks>()
                    .ToSelf()
                    .InSingletonScope()
                    .OnActivation((context, instance) =>
                    {
                        kernel.Bind<IWarrior>().To<FootSoldier>();
                        kernel.Bind<IWeapon>().To<Shuriken>();

                        instance.Warrior = context.Kernel.Get<IWarrior>();
                        instance.Weapon = context.Kernel.Get<IWeapon>();
                    })
                    .OnDeactivation((context, instance) =>
                    {
                        kernel.Rebind<IWarrior>().To<Samurai>();
                        kernel.Rebind<IWeapon>().To<Sword>();

                        instance.Warrior = context.Kernel.Get<IWarrior>();
                        instance.Weapon = context.Kernel.Get<IWeapon>();
                    });

                barracks = kernel.Get<Barracks>();
                barracks.Warrior.ShouldNotBeNull();
                barracks.Warrior.ShouldBeInstanceOf<FootSoldier>();
                barracks.Weapon.ShouldNotBeNull();
                barracks.Weapon.ShouldBeInstanceOf<Shuriken>();
            }

            barracks.Warrior.ShouldBeInstanceOf<Samurai>();
            barracks.Weapon.ShouldBeInstanceOf<Sword>();
        }
	}
}