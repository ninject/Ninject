#region Usings

using System.Collections;
using System.Linq;
using Ninject.Dynamic;
using Ninject.Dynamic.Extensions;
using Ninject.Dynamic.Modules;
using Ninject.Modules;
using Ninject.Parameters;
using Ninject.Tests.Fakes;
using Ninject.Tests.Integration.StandardKernelTests;
using Xunit;
using Xunit.Should;

#endregion

namespace Ninject.Tests.Integration.RubyKernelTests
{
    public class RubyKernelContext
    {
        protected readonly RubyKernel kernel;

        public RubyKernelContext()
        {
            kernel = new RubyKernel();
        }

        protected void SetPath(string path)
        {
            IEnumerableExtensions.ForEach((IEnumerable)kernel.Components.GetAll<IModuleLoaderPlugin>(), mod =>
            {
                if (mod is RubyModuleLoaderPlugin)
                    ((RubyModuleLoaderPlugin)mod).SupportedPatterns = new[] { path };
            });
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
            SetPath("config_single.rb");
            kernel.AutoLoadModulesRecursively("~");

            var weapon = kernel.Get<IWeapon>();

            weapon.ShouldNotBeNull();
            weapon.ShouldBeInstanceOf<Sword>();
        }

        [Fact]
        public void FirstInstanceIsReturnedWhenMultipleBindingsAreRegistered()
        {
            SetPath("config_double.rb");
            kernel.AutoLoadModulesRecursively("~");

            var weapon = kernel.Get<IWeapon>();

            weapon.ShouldNotBeNull();
            weapon.ShouldBeInstanceOf<Sword>();
        }

        [Fact]
        public void DependenciesAreInjectedViaConstructor()
        {
            SetPath("config_two_types.rb");
            kernel.AutoLoadModulesRecursively("~");

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
            SetPath("config_to_self.rb");
            kernel.AutoLoadModulesRecursively("~");

            var weapon = kernel.Get<Sword>();

            weapon.ShouldNotBeNull();
            weapon.ShouldBeInstanceOf<Sword>();
        }

        

        [Fact]
        public void DependenciesAreInjectedViaConstructor()
        {
            SetPath("config_two_types_to_self.rb");
            kernel.AutoLoadModulesRecursively("~");

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
            SetPath("config_open_generics.rb");
            kernel.AutoLoadModulesRecursively("~");

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
            SetPath("config_named.rb");
            kernel.AutoLoadModulesRecursively("~");

            var weapon = kernel.Get<IWeapon>("sword");

            weapon.ShouldNotBeNull();
            weapon.ShouldBeInstanceOf<Sword>();
        }

        [Fact]
        public void ReturnsServiceRegisteredViaBindingThatMatchesPredicate()
        {
            SetPath("config_metadata.rb");
            kernel.AutoLoadModulesRecursively("~");

            var weapon = kernel.Get<IWeapon>(x => x.Get<string>("type") == "melee");

            weapon.ShouldNotBeNull();
            weapon.ShouldBeInstanceOf<Sword>();
        }
    }

    public class WhenBoundWithConstructorArguments : RubyKernelContext
    {
        [Fact]
        public void ReturnsServiceRegistered()
        {
            SetPath("config_constructor_arguments.rb");
            kernel.AutoLoadModulesRecursively("~");

            var weapon = kernel.Get<IWeapon>();

            weapon.ShouldNotBeNull();
            weapon.ShouldBeInstanceOf<Knife>();
            weapon.Name.ShouldBe("Blunt knife");
        }

        [Fact]
        public void ReturnsServiceWhenRegisteredAsDSL()
        {
            SetPath("config_constructor_arguments_dsl.rb");
            kernel.AutoLoadModulesRecursively("~");

            var weapon = kernel.Get<IWeapon>();

            weapon.ShouldNotBeNull();
            weapon.ShouldBeInstanceOf<Knife>();
            weapon.Name.ShouldBe("Blunt knife");
        }
    }


}