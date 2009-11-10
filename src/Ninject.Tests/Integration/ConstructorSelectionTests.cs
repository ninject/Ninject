using Ninject.Parameters;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Integration
{
	public class ConstructorSelectionTests
	{
		[Fact]
		public void DefaultCtorIsUsedWhenNoBindingAreAvailable()
		{
			using ( IKernel kernel = new StandardKernel() )
			{
				kernel.Bind<Barracks>().ToSelf();

				var barracks = kernel.Get<Barracks>();
				Assert.NotNull( barracks );
				Assert.Null( barracks.Warrior );
				Assert.Null( barracks.Weapon );
			}
		}

		[Fact]
		public void CtorIsUsedWhenParameterIsSupplied()
		{
			using(IKernel kernel = new StandardKernel())
			{
				kernel.Bind<Barracks>().ToSelf();
				var constructorArgument = new ConstructorArgument("warrior", new Samurai(new Sword()));
				var barracks = kernel.Get<Barracks>(constructorArgument);
				
				Assert.NotNull(barracks);
				Assert.NotNull(barracks.Warrior);
				Assert.NotNull(barracks.Warrior.Weapon);
				Assert.Null(barracks.Weapon);
			}
		}

		[Fact]
		public void FirstAvailableWithBindingAvailableIsUsed()
		{
			using ( IKernel kernel = new StandardKernel() )
			{
				kernel.Bind<Barracks>().ToSelf();
				kernel.Bind<IWeapon>().To<Sword>();

				var barracks = kernel.Get<Barracks>();
				Assert.NotNull( barracks );
				Assert.Null( barracks.Warrior );
				Assert.NotNull( barracks.Weapon );
			}
		}

		[Fact]
		public void CtorWithMostDependenciesIsUsedWhenBindingsAreAvailable()
		{
			using ( IKernel kernel = new StandardKernel() )
			{
				kernel.Bind<Barracks>().ToSelf();
				kernel.Bind<IWeapon>().To<Sword>();
				kernel.Bind<IWarrior>().To<Samurai>();

				var barracks = kernel.Get<Barracks>();
				Assert.NotNull( barracks );
				Assert.NotNull( barracks.Warrior );
				Assert.NotNull( barracks.Warrior.Weapon );
				Assert.NotNull( barracks.Weapon );
			}
		}

		[Fact]
		public void CreationWillFailIfAllDepenciesAreMissingAndInjectAttributeIsApplied()
		{
			using ( IKernel kernel = new StandardKernel() )
			{
				kernel.Bind<NinjaBarracks>().ToSelf();

				Assert.Throws<ActivationException>( () => kernel.Get<NinjaBarracks>() );

				kernel.Bind<IWeapon>().To<Sword>();
				Assert.Throws<ActivationException>( () => kernel.Get<NinjaBarracks>() );
				kernel.Unbind<IWeapon>();

				kernel.Bind<IWarrior>().To<Samurai>();
				Assert.Throws<ActivationException>( () => kernel.Get<NinjaBarracks>() );
				kernel.Unbind<IWarrior>();
			}
		}
	}
}