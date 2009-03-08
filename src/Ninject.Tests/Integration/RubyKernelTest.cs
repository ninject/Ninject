using Ninject.Dynamic;
using Ninject.Tests.Fakes;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Integration
{

    public class RubyKernelContext
    {
        protected readonly RubyKernel kernel;

        public RubyKernelContext()
        {
            kernel = new RubyKernel();
        }
    }

    public class WhenConfiguredFromCSharp : RubyKernelContext
    {
        [Fact]
		public void SingleInstanceIsReturnedWhenOneBindingIsRegistered()
		{
			kernel.Bind<IWeapon>().To<Sword>();

			var weapon = kernel.Get<IWeapon>();

			weapon.ShouldNotBeNull();
			weapon.ShouldBeInstanceOf<Sword>();
		}

		[Fact]
		public void FirstInstanceIsReturnedWhenMultipleBindingsAreRegistered()
		{
			kernel.Bind<IWeapon>().To<Sword>();
			kernel.Bind<IWeapon>().To<Shuriken>();

			var weapon = kernel.Get<IWeapon>();

			weapon.ShouldNotBeNull();
			weapon.ShouldBeInstanceOf<Sword>();
		}

		[Fact]
		public void DependenciesAreInjectedViaConstructor()
		{
			kernel.Bind<IWeapon>().To<Sword>();
			kernel.Bind<IWarrior>().To<Samurai>();

			var warrior = kernel.Get<IWarrior>();

			warrior.ShouldNotBeNull();
			warrior.ShouldBeInstanceOf<Samurai>();
			warrior.Weapon.ShouldNotBeNull();
			warrior.Weapon.ShouldBeInstanceOf<Sword>();
		}
    }


    public class WhenConfiguredFromRuby : RubyKernelContext
    {
        [Fact]
		public void SingleInstanceIsReturnedWhenOneBindingIsRegistered()
		{
            kernel.AutoLoadModulesRecursively("~", "config_single.rb");

			var weapon = kernel.Get<IWeapon>();

			weapon.ShouldNotBeNull();
			weapon.ShouldBeInstanceOf<Sword>();
		}

        [Fact]
        public void FirstInstanceIsReturnedWhenMultipleBindingsAreRegistered()
        {
            kernel.AutoLoadModulesRecursively("~", "config_double.rb");

            var weapon = kernel.Get<IWeapon>();

            weapon.ShouldNotBeNull();
            weapon.ShouldBeInstanceOf<Sword>();
        }

        [Fact]
        public void DependenciesAreInjectedViaConstructor()
        {
            kernel.AutoLoadModulesRecursively("~", "config_two_types.rb");

            var warrior = kernel.Get<IWarrior>();

            warrior.ShouldNotBeNull();
            warrior.ShouldBeInstanceOf<Samurai>();
            warrior.Weapon.ShouldNotBeNull();
            warrior.Weapon.ShouldBeInstanceOf<Sword>();
        }
    }

    public class WhenBoundToSelf : RubyKernelContext
    {
        [Fact]
        public void SingleInstanceIsReturnedWhenOneBindingIsRegistered()
        {
            kernel.AutoLoadModulesRecursively("~", "config_to_self.rb");

            var weapon = kernel.Get<Sword>();

            weapon.ShouldNotBeNull();
            weapon.ShouldBeInstanceOf<Sword>();
        }

        [Fact]
        public void DependenciesAreInjectedViaConstructor()
        {
            kernel.AutoLoadModulesRecursively("~", "config_two_types_to_self.rb");

            var samurai = kernel.Get<Samurai>();

            samurai.ShouldNotBeNull();
            samurai.Weapon.ShouldNotBeNull();
            samurai.Weapon.ShouldBeInstanceOf<Sword>();
        }
    }
}

