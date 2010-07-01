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
	}
}