using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Dynamic;
using Ninject.Tests.Fakes;
using Ninject.Tests.Integration.StandardKernelTests;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Integration.RubyKernelTests
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


    public class WhenBoundToGenericServiceRegisteredViaOpenGenericType : RubyKernelContext
    {
        [Fact]
        public void GenericParametersAreInferred()
        {
            kernel.AutoLoadModulesRecursively("~", "config_open_generics.rb");

            var services = kernel.GetAll<IGeneric<int>>().ToArray();

            services.ShouldNotBeNull();
            services.Length.ShouldBe(2);
            services[0].ShouldBeInstanceOf<GenericService<int>>();
            services[1].ShouldBeInstanceOf<GenericService2<int>>();
        }
    }

    public class WhenBoundWithConstraints : RubyKernelContext
    {
        [Fact]
        public void ReturnsServiceRegisteredViaBindingWithSpecifiedName()
        {
            kernel.AutoLoadModulesRecursively("~", "config_named.rb");

            var weapon = kernel.Get<IWeapon>("sword");

            weapon.ShouldNotBeNull();
            weapon.ShouldBeInstanceOf<Sword>();
        }

        [Fact]
        public void ReturnsServiceRegisteredViaBindingThatMatchesPredicate()
        {
            kernel.AutoLoadModulesRecursively("~", "config_metadata.rb");

            var weapon = kernel.Get<IWeapon>(x => x.Get<string>("type") == "melee");

            weapon.ShouldNotBeNull();
            weapon.ShouldBeInstanceOf<Sword>();
        }
    }
}

