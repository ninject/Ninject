using FluentAssertions;
using Ninject.Parameters;
using Ninject.Tests.Fakes;
using System;
using System.Collections.Generic;
using Xunit;

namespace Ninject.Tests.Integration
{
    public class ReadOnlyKernelTests
    {
        public class WhenTryGetIsCalledForUnboundService : ReadOnlyKernelContext
        {
            [Fact]
            public void TryGetOfT_Parameters_ImplicitSelfBindingIsRegisteredAndActivatedIfTypeIsSelfBindable()
            {
                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet<Sword>();
                weapon.Should().NotBeNull();
                weapon.Should().BeOfType<Sword>();

                var bindings = Kernel.GetBindings(typeof(Sword));
                bindings.Should().HaveCount(1);
                bindings.Should().ContainSingle(b => b.IsImplicit);
            }

            [Fact]
            public void TryGetOfT_Parameters_ReturnsNullIfTypeIsNotSelfBindable()
            {
                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet<IWeapon>();
                weapon.Should().BeNull();

                var bindings = Kernel.GetBindings(typeof(IWeapon));
                bindings.Should().BeEmpty();
            }

            [Fact]
            public void TryGetOfT_Parameters_ReturnsNullIfTypeHasOnlyUnmetConditionalBindings()
            {
                Configuration.Bind<IWeapon>().To<Sword>().When(ctx => false);

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet<IWeapon>();
                weapon.Should().BeNull();

                var bindings = Kernel.GetBindings(typeof(IWeapon));
                bindings.Should().HaveCount(1);
            }

            [Fact]
            public void TryGetOfT_Parameters_ReturnsNullIfNoBindingForADependencyExists()
            {
                Configuration.Bind<IWarrior>().To<Samurai>();

                Kernel = Configuration.BuildReadOnlyKernel();

                var warrior = Kernel.TryGet<IWarrior>();
                warrior.Should().BeNull();

                var bindings = Kernel.GetBindings(typeof(IWarrior));
                bindings.Should().HaveCount(1);
            }

            [Fact]
            public void TryGetOfT_Parameters_ReturnsNullIfMultipleBindingsExistForADependency()
            {
                Configuration.Bind<IWarrior>().To<Samurai>();
                Configuration.Bind<IWeapon>().To<Sword>();
                Configuration.Bind<IWeapon>().To<Shuriken>();

                Kernel = Configuration.BuildReadOnlyKernel();

                var warrior = Kernel.TryGet<IWarrior>();
                warrior.Should().BeNull();

                var bindings = Kernel.GetBindings(typeof(IWarrior));
                bindings.Should().HaveCount(1);
            }

            [Fact]
            public void TryGetOfT_Parameters_ReturnsNullIfOnlyUnmetConditionalBindingsExistForADependency()
            {
                Configuration.Bind<IWarrior>().To<Samurai>();
                Configuration.Bind<IWeapon>().To<Sword>().When(ctx => false);

                Kernel = Configuration.BuildReadOnlyKernel();

                var warrior = Kernel.TryGet<IWarrior>();
                warrior.Should().BeNull();

                var bindings = Kernel.GetBindings(typeof(IWarrior));
                bindings.Should().HaveCount(1);
            }

            [Fact(Skip = "https://github.com/ninject/Ninject/issues/341")]
            public void TryGetOfT_NameAndParameters_ReturnsNullWhenNoMatchingBindingExistsAndRegistersImplicitSelfBindingIfTypeIsSelfBindable()
            {
                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet<Sword>("a", Array.Empty<IParameter>());

                weapon.Should().BeNull();

                var bindings = Kernel.GetBindings(typeof(Sword));
                bindings.Should().HaveCount(1);
                bindings.Should().OnlyContain(b => b.IsImplicit);
            }

            [Fact(Skip = "https://github.com/ninject/Ninject/issues/341")]
            public void TryGet_ServiceAndNameAndParameters_ReturnsNullWhenNoMatchingBindingExistsAndRegistersImplicitSelfBindingIfTypeIsSelfBindable()
            {
                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet(typeof(Sword), "a", Array.Empty<IParameter>());

                weapon.Should().BeNull();

                var bindings = Kernel.GetBindings(typeof(Sword));
                bindings.Should().HaveCount(1);
                bindings.Should().OnlyContain(b => b.IsImplicit);
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfNoBindingExistsAndTypeIsNotSelfBindable()
            {
                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet(typeof(IWeapon), "a", Array.Empty<IParameter>());
                weapon.Should().BeNull();

                var bindings = Kernel.GetBindings(typeof(IWeapon));
                bindings.Should().BeEmpty();
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfNoMatchingBindingExistsAndTypeIsNotSelfBindable()
            {
                Configuration.Bind<IWeapon>().To<Sword>().Named("b");

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet(typeof(IWeapon), "a", Array.Empty<IParameter>());
                weapon.Should().BeNull();

                var bindings = Kernel.GetBindings(typeof(IWeapon));
                bindings.Should().HaveCount(1);
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfTypeHasOnlyUnmetConditionalBindings()
            {
                Configuration.Bind<IWeapon>().To<Sword>().When(ctx => false).Named("a");

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet(typeof(IWeapon), "a", Array.Empty<IParameter>());
                weapon.Should().BeNull();

                var bindings = Kernel.GetBindings(typeof(IWeapon));
                bindings.Should().HaveCount(1);
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfNoBindingForADependencyExists()
            {
                Configuration.Bind<IWarrior>().To<Samurai>().Named("a");

                Kernel = Configuration.BuildReadOnlyKernel();

                var warrior = Kernel.TryGet(typeof(IWarrior), "a", Array.Empty<IParameter>());
                warrior.Should().BeNull();

                var bindings = Kernel.GetBindings(typeof(IWarrior));
                bindings.Should().HaveCount(1);
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfMultipleBindingsExistForADependency()
            {
                Configuration.Bind<IWarrior>().To<Samurai>().Named("a");
                Configuration.Bind<IWeapon>().To<Sword>();
                Configuration.Bind<IWeapon>().To<Shuriken>();

                Kernel = Configuration.BuildReadOnlyKernel();

                var warrior = Kernel.TryGet(typeof(IWarrior), "a", Array.Empty<IParameter>());
                warrior.Should().BeNull();

                var bindings = Kernel.GetBindings(typeof(IWarrior));
                bindings.Should().HaveCount(1);
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfOnlyUnmetConditionalBindingsExistForADependency()
            {
                Configuration.Bind<IWarrior>().To<Samurai>();
                Configuration.Bind<IWeapon>().To<Sword>().When(ctx => false);

                Kernel = Configuration.BuildReadOnlyKernel();

                var warrior = Kernel.TryGet(typeof(IWarrior), (metadata) => true, Array.Empty<IParameter>());
                warrior.Should().BeNull();

                var bindings = Kernel.GetBindings(typeof(IWarrior));
                bindings.Should().HaveCount(1);
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ImplicitSelfBindingIsRegisteredAndActivatedIfTypeIsSelfBindable()
            {
                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet(typeof(Sword), (metadata) => true, Array.Empty<IParameter>());
                weapon.Should().NotBeNull();
                weapon.Should().BeOfType<Sword>();

                var bindings = Kernel.GetBindings(typeof(Sword));
                bindings.Should().HaveCount(1);
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullIfTypeIsNotSelfBindable()
            {
                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet(typeof(IWeapon), (metadata) => true, Array.Empty<IParameter>());
                weapon.Should().BeNull();

                var bindings = Kernel.GetBindings(typeof(IWeapon));
                bindings.Should().HaveCount(0);
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullIfTypeHasOnlyUnmetConditionalBindings()
            {
                Configuration.Bind<IWeapon>().To<Sword>().When(ctx => false);

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet(typeof(IWeapon), (metadata) => true, Array.Empty<IParameter>());
                weapon.Should().BeNull();

                var bindings = Kernel.GetBindings(typeof(IWeapon));
                bindings.Should().HaveCount(1);
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullIfNoBindingForADependencyExists()
            {
                Configuration.Bind<IWarrior>().To<Samurai>();

                Kernel = Configuration.BuildReadOnlyKernel();

                var warrior = Kernel.TryGet(typeof(IWarrior), (metadata) => true, Array.Empty<IParameter>());
                warrior.Should().BeNull();

                var bindings = Kernel.GetBindings(typeof(IWarrior));
                bindings.Should().HaveCount(1);
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullIfMultipleBindingsExistForADependency()
            {
                Configuration.Bind<IWarrior>().To<Samurai>();
                Configuration.Bind<IWeapon>().To<Sword>();
                Configuration.Bind<IWeapon>().To<Shuriken>();

                Kernel = Configuration.BuildReadOnlyKernel();

                var warrior = Kernel.TryGet(typeof(IWarrior), (metadata) => true, Array.Empty<IParameter>());
                warrior.Should().BeNull();

                var bindings = Kernel.GetBindings(typeof(IWarrior));
                bindings.Should().HaveCount(1);
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullIfOnlyUnmetConditionalBindingsExistForADependency()
            {
                Configuration.Bind<IWarrior>().To<Samurai>();
                Configuration.Bind<IWeapon>().To<Sword>().When(ctx => false);

                Kernel = Configuration.BuildReadOnlyKernel();

                var warrior = Kernel.TryGet(typeof(IWarrior), (metadata) => true, Array.Empty<IParameter>());
                warrior.Should().BeNull();

                var bindings = Kernel.GetBindings(typeof(IWarrior));
                bindings.Should().HaveCount(1);
            }
        }

        public class WhenTryGetIsCalledForServiceWithMultipleBindingsOfSameWeight : ReadOnlyKernelContext
        {
            [Fact]
            public void TryGetOfT_Parameters()
            {
                Configuration.Bind<IWeapon>().To<Sword>();
                Configuration.Bind<IWeapon>().To<Shuriken>();

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet<IWeapon>(Array.Empty<IParameter>());
                weapon.Should().BeNull();

                var bindings = Kernel.GetBindings(typeof(IWeapon));
                bindings.Should().HaveCount(2);
            }

            [Fact]
            public void TryGetOfT_NameAndParameters()
            {
                Configuration.Bind<IWeapon>().To<Sword>().Named("a");
                Configuration.Bind<IWeapon>().To<Shuriken>().Named("a");

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet<IWeapon>("a", Array.Empty<IParameter>());
                weapon.Should().BeNull();

                var bindings = Kernel.GetBindings(typeof(IWeapon));
                bindings.Should().HaveCount(2);
            }

            [Fact]
            public void TryGetOfT_ConstraintAndParameters()
            {
                Configuration.Bind<IWeapon>().To<Sword>().Named("a");
                Configuration.Bind<IWeapon>().To<Shuriken>().Named("a");

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet<IWeapon>((metadata) => metadata.Name == "a", Array.Empty<IParameter>());
                weapon.Should().BeNull();

                var bindings = Kernel.GetBindings(typeof(IWeapon));
                bindings.Should().HaveCount(2);
            }

            [Fact]
            public void TryGet_ServiceAndParameters()
            {
                var service = typeof(IWeapon);

                Configuration.Bind<IWeapon>().To<Sword>();
                Configuration.Bind<IWeapon>().To<Shuriken>();

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet(service, Array.Empty<IParameter>());

                weapon.Should().BeNull();

                var bindings = Kernel.GetBindings(service);
                bindings.Should().HaveCount(2);
                bindings.Should().OnlyContain(b => !b.IsImplicit);
            }

            [Fact(Skip = "Unique?")]
            public void TryGet_ServiceAndNameAndParameters_ResolvesUsingFirstMatchingBindingWhenTypeIsSelfBinding()
            {
                var service = typeof(Sword);

                Configuration.Bind<Sword>().To<Sword>().Named("a");
                Configuration.Bind<Sword>().To<ShortSword>().Named("a");

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet(service, "a", Array.Empty<IParameter>());
                weapon.Should().NotBeNull();
                weapon.Should().BeOfType<Sword>();

                var bindings = Kernel.GetBindings(service);
                bindings.Should().HaveCount(2);
                bindings.Should().OnlyContain(b => !b.IsImplicit);
            }


            [Fact(Skip = "Unique?")]
            public void TryGet_ServiceAndNameAndParameters_ResolvesUsingFirstMatchingBindingWhenTypeIsNotSelfBinding()
            {
                var service = typeof(IWeapon);

                Configuration.Bind<IWeapon>().To<Sword>().Named("a");
                Configuration.Bind<IWeapon>().To<Shuriken>().Named("a");

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet(service, "a", Array.Empty<IParameter>());
                weapon.Should().NotBeNull();
                weapon.Should().BeOfType<Sword>();

                var bindings = Kernel.GetBindings(service);
                bindings.Should().HaveCount(2);
                bindings.Should().OnlyContain(b => !b.IsImplicit);
            }

            [Fact(Skip = "Unique?")]
            public void TryGet_ServiceAndConstraintAndParameters_ResolvesUsingFirstMatchingBindingWhenTypeIsSelfBinding()
            {
                var service = typeof(Sword);

                Configuration.Bind<Sword>().To<ShortSword>().Named("b");
                Configuration.Bind<Sword>().To<Sword>().Named("a");
                Configuration.Bind<Sword>().To<ShortSword>().Named("a");

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet(service, (metadata) => metadata.Name == "a", Array.Empty<IParameter>());

                weapon.Should().NotBeNull();
                weapon.Should().BeOfType<Sword>();

                var bindings = Kernel.GetBindings(service);
                bindings.Should().HaveCount(3);
                bindings.Should().OnlyContain(b => !b.IsImplicit);
            }

            [Fact(Skip ="Unique?")]
            public void TryGet_ServiceAndConstraintAndParameters_ResolvesUsingFirstMatchingBindingWhenTypeIsNotSelfBindingAndNotGeneric()
            {
                var service = typeof(IWeapon);

                Configuration.Bind<IWeapon>().To<Sword>().Named("a");
                Configuration.Bind<IWeapon>().To<Shuriken>().Named("a");

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet(service, (metadata) => metadata.Name == "a", Array.Empty<IParameter>());

                weapon.Should().NotBeNull();
                weapon.Should().BeOfType<Sword>();

                var bindings = Kernel.GetBindings(service);
                bindings.Should().HaveCount(2);
                bindings.Should().OnlyContain(b => !b.IsImplicit);
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullWhenBindingsDoNotMatchAndTypeIsNotSelfBindingAndNotOpenGeneric()
            {
                var service = typeof(IWeapon);

                Configuration.Bind<IWeapon>().To<Sword>().Named("b");
                Configuration.Bind<IWeapon>().To<ShortSword>().Named("b");

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet(service, (metadata) => metadata.Name == "a", Array.Empty<IParameter>());

                weapon.Should().BeNull();

                var bindings = Kernel.GetBindings(service);
                bindings.Should().HaveCount(2);
                bindings.Should().OnlyContain(b => !b.IsImplicit);
            }
        }

        public class WhenTryGetIsCalledForBoundListOfServices : ReadOnlyKernelContext
        {
            [Fact]
            public void TryGetOfT_Parameters()
            {
                Configuration.Bind<IWeapon>().To<Sword>();
                Configuration.Bind<IWeapon>().To<Shuriken>();

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapons = Kernel.TryGet<List<IWeapon>>(Array.Empty<IParameter>());
                weapons.Should().NotBeNull();
                weapons.Should().HaveCount(2);
            }

            [Fact]
            public void TryGetOfT_Parameters_ShouldPreferBindingForList()
            {
                Configuration.Bind<IWeapon>().To<Sword>();
                Configuration.Bind<IWeapon>().To<Shuriken>();
                Configuration.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() });

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapons = Kernel.TryGet<List<IWeapon>>(Array.Empty<IParameter>());
                weapons.Should().NotBeNull();
                weapons.Should().HaveCount(1);
                weapons.Should().AllBeOfType<Dagger>();
            }

            [Fact]
            public void TryGetOfT_NameAndParameters()
            {
                Configuration.Bind<IWeapon>().To<Dagger>().Named("b");
                Configuration.Bind<IWeapon>().To<Sword>().Named("a");
                Configuration.Bind<IWeapon>().To<Shuriken>().Named("a");

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapons = Kernel.TryGet<List<IWeapon>>("a", Array.Empty<IParameter>());
                weapons.Should().NotBeNull();
                weapons.Should().HaveCount(3);

                var bindings = Kernel.GetBindings(typeof(List<IWeapon>));
                bindings.Should().HaveCount(0);
            }

            [Fact]
            public void TryGetOfT_NameAndParameters_ShouldPreferBindingForList()
            {
                Configuration.Bind<IWeapon>().To<Sword>().Named("a");
                Configuration.Bind<IWeapon>().To<Shuriken>().Named("a");
                Configuration.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Sword() }).Named("b");
                Configuration.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() }).Named("a");

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapons = Kernel.TryGet<List<IWeapon>>("a", Array.Empty<IParameter>());
                weapons.Should().NotBeNull();
                weapons.Should().HaveCount(1);
                weapons.Should().AllBeOfType<Dagger>();

                var bindings = Kernel.GetBindings(typeof(List<IWeapon>));
                bindings.Should().HaveCount(2);
            }

            [Fact]
            public void TryGetOfT_ConstraintAndParameters()
            {
                Configuration.Bind<IWeapon>().To<Dagger>().Named("b");
                Configuration.Bind<IWeapon>().To<Sword>().Named("a");
                Configuration.Bind<IWeapon>().To<Shuriken>().Named("a");

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapons = Kernel.TryGet<List<IWeapon>>((metadata) => metadata.Name == "a", Array.Empty<IParameter>());
                weapons.Should().NotBeNull();
                weapons.Should().HaveCount(3);

                var bindings = Kernel.GetBindings(typeof(List<IWeapon>));
                bindings.Should().HaveCount(0);
            }

            [Fact]
            public void TryGetOfT_ConstraintAndParameters_ShouldPreferBindingForList()
            {
                Configuration.Bind<IWeapon>().To<Sword>().Named("a");
                Configuration.Bind<IWeapon>().To<Shuriken>().Named("a");
                Configuration.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Sword() }).Named("b");
                Configuration.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() }).Named("a");

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapons = Kernel.TryGet<List<IWeapon>>((metadata) => metadata.Name == "a", Array.Empty<IParameter>());
                weapons.Should().NotBeNull();
                weapons.Should().HaveCount(1);
                weapons.Should().AllBeOfType<Dagger>();

                var bindings = Kernel.GetBindings(typeof(List<IWeapon>));
                bindings.Should().HaveCount(2);
            }


            [Fact]
            public void TryGet_ServiceAndParameters()
            {
                Configuration.Bind<IWeapon>().To<Sword>();
                Configuration.Bind<IWeapon>().To<Shuriken>();

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapons = Kernel.TryGet(typeof(List<IWeapon>), Array.Empty<IParameter>());
                weapons.Should().NotBeNull();
                weapons.Should().BeOfType<List<IWeapon>>();

                var weaponsList = (List<IWeapon>)weapons;
                weaponsList.Should().HaveCount(2);

                var bindings = Kernel.GetBindings(typeof(List<IWeapon>));
                bindings.Should().HaveCount(0);
            }

            [Fact]
            public void TryGet_ServiceAndParameters_ShouldPreferBindingForList()
            {
                Configuration.Bind<IWeapon>().To<Sword>();
                Configuration.Bind<IWeapon>().To<Shuriken>();
                Configuration.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() });

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapons = Kernel.TryGet(typeof(List<IWeapon>), Array.Empty<IParameter>());
                weapons.Should().NotBeNull();
                weapons.Should().BeOfType<List<IWeapon>>();

                var weaponsList = (List<IWeapon>)weapons;
                weaponsList.Should().HaveCount(1);
                weaponsList.Should().AllBeOfType<Dagger>();

                var bindings = Kernel.GetBindings(typeof(List<IWeapon>));
                bindings.Should().HaveCount(1);
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters()
            {
                Configuration.Bind<IWeapon>().To<Dagger>().Named("b");
                Configuration.Bind<IWeapon>().To<Sword>().Named("a");
                Configuration.Bind<IWeapon>().To<Shuriken>().Named("a");

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapons = Kernel.TryGet(typeof(List<IWeapon>), "a", Array.Empty<IParameter>());

                weapons.Should().NotBeNull();
                weapons.Should().BeOfType<List<IWeapon>>();

                var weaponsList = (List<IWeapon>)weapons;
                weaponsList.Should().HaveCount(3);

                var bindings = Kernel.GetBindings(typeof(List<IWeapon>));
                bindings.Should().HaveCount(0);
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters_ShouldPreferBindingForList()
            {
                Configuration.Bind<IWeapon>().To<Sword>().Named("a");
                Configuration.Bind<IWeapon>().To<Shuriken>().Named("a");
                Configuration.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Sword() }).Named("b");
                Configuration.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() }).Named("a");

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapons = Kernel.TryGet(typeof(List<IWeapon>), "a", Array.Empty<IParameter>());
                weapons.Should().NotBeNull();
                weapons.Should().BeOfType<List<IWeapon>>();

                var weaponsList = (List<IWeapon>)weapons;
                weaponsList.Should().HaveCount(1);
                weaponsList.Should().AllBeOfType<Dagger>();

                var bindings = Kernel.GetBindings(typeof(List<IWeapon>));
                bindings.Should().HaveCount(2);
            }


            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters()
            {
                Configuration.Bind<IWeapon>().To<Dagger>().Named("b");
                Configuration.Bind<IWeapon>().To<Sword>().Named("a");
                Configuration.Bind<IWeapon>().To<Shuriken>().Named("a");

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapons = Kernel.TryGet(typeof(List<IWeapon>), (metadata) => metadata.Name == "a", Array.Empty<IParameter>());
                weapons.Should().NotBeNull();
                weapons.Should().BeOfType<List<IWeapon>>();

                var weaponsList = (List<IWeapon>)weapons;
                weaponsList.Should().HaveCount(3);

                var bindings = Kernel.GetBindings(typeof(List<IWeapon>));
                bindings.Should().HaveCount(0);
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ShouldPreferBindingForList()
            {
                Configuration.Bind<IWeapon>().To<Sword>().Named("a");
                Configuration.Bind<IWeapon>().To<Shuriken>().Named("a");
                Configuration.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Sword() }).Named("b");
                Configuration.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() }).Named("a");

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapons = Kernel.TryGet(typeof(List<IWeapon>), (metadata) => metadata.Name == "a", Array.Empty<IParameter>());
                weapons.Should().NotBeNull();
                weapons.Should().BeOfType<List<IWeapon>>();

                var weaponsList = (List<IWeapon>)weapons;
                weaponsList.Should().HaveCount(1);
                weaponsList.Should().AllBeOfType<Dagger>();

                var bindings = Kernel.GetBindings(typeof(List<IWeapon>));
                bindings.Should().HaveCount(2);
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullWhenTypeIsUnboundGenericTypeDefinition()
            {
                var service = typeof(List<>);

                Configuration.Bind<List<int>>().ToConstant(new List<int> { 1 }).Named("a");

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet(service, (metadata) => metadata.Name == "a", Array.Empty<IParameter>());

                weapon.Should().BeNull();

                var bindings = Kernel.GetBindings(service);
                bindings.Should().BeEmpty();
            }
        }

        public class WhenTryGetIsCalledForUnboundListOfServices : ReadOnlyKernelContext
        {
            [Fact]
            public void TryGetOfT_Parameters()
            {
                Kernel = Configuration.BuildReadOnlyKernel();

                var weapons = Kernel.TryGet<List<IWeapon>>(Array.Empty<IParameter>());

                weapons.Should().NotBeNull();
                weapons.Should().BeEmpty();

                var bindings = Kernel.GetBindings(typeof(List<IWeapon>));
                bindings.Should().BeEmpty();
            }

            [Fact]
            public void TryGetOfT_NameAndParameters()
            {
                Kernel = Configuration.BuildReadOnlyKernel();

                var weapons = Kernel.TryGet<List<IWeapon>>("b", Array.Empty<IParameter>());

                weapons.Should().NotBeNull();
                weapons.Should().BeEmpty();

                var bindings = Kernel.GetBindings(typeof(List<IWeapon>));
                bindings.Should().BeEmpty();
            }

            [Fact]
            public void TryGetOfT_ConstraintAndParameters()
            {
                Kernel = Configuration.BuildReadOnlyKernel();

                var weapons = Kernel.TryGet<List<IWeapon>>((metadata) => metadata.Name == "b", Array.Empty<IParameter>());
                weapons.Should().NotBeNull();
                weapons.Should().BeEmpty();

                var bindings = Kernel.GetBindings(typeof(List<IWeapon>));
                bindings.Should().BeEmpty();
            }

            [Fact]
            public void TryGet_ServiceAndParameters()
            {
                Kernel = Configuration.BuildReadOnlyKernel();

                var weapons = Kernel.TryGet(typeof(List<IWeapon>), Array.Empty<IParameter>());
                weapons.Should().NotBeNull();
                weapons.Should().BeOfType<List<IWeapon>>();

                var weaponsList = (List<IWeapon>)weapons;
                weaponsList.Should().BeEmpty();

                var bindings = Kernel.GetBindings(typeof(List<IWeapon>));
                bindings.Should().BeEmpty();
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters()
            {
                Kernel = Configuration.BuildReadOnlyKernel();

                var weapons = Kernel.TryGet(typeof(List<IWeapon>), "a", Array.Empty<IParameter>());
                weapons.Should().NotBeNull();
                weapons.Should().BeOfType<List<IWeapon>>();

                var weaponsList = (List<IWeapon>)weapons;
                weaponsList.Should().BeEmpty();

                var bindings = Kernel.GetBindings(typeof(List<IWeapon>));
                bindings.Should().BeEmpty();
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters()
            {
                Kernel = Configuration.BuildReadOnlyKernel();

                var weapons = Kernel.TryGet(typeof(List<IWeapon>), (metadata) => metadata.Name == "a", Array.Empty<IParameter>());
                weapons.Should().NotBeNull();
                weapons.Should().BeOfType<List<IWeapon>>();

                var weaponsList = (List<IWeapon>)weapons;
                weaponsList.Should().BeEmpty();

                var bindings = Kernel.GetBindings(typeof(List<IWeapon>));
                bindings.Should().BeEmpty();
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullWhenTypeIsUnboundGenericTypeDefinition()
            {
                var service = typeof(List<>);

                Kernel = Configuration.BuildReadOnlyKernel();

                var weapon = Kernel.TryGet(service, (metadata) => metadata.Name == "a", Array.Empty<IParameter>());

                weapon.Should().BeNull();

                var bindings = Kernel.GetBindings(service);
                bindings.Should().BeEmpty();
            }
        }

        public abstract class ReadOnlyKernelContext : IDisposable
        {
            protected IReadOnlyKernel Kernel { get; set; }

            protected KernelConfiguration Configuration { get; }

            protected ReadOnlyKernelContext()
            {
                var settings = new NinjectSettings
                    {
                        // Disable to reduce memory pressure
                        ActivationCacheDisabled = true,
                        LoadExtensions = false,
                    };

                Configuration = new KernelConfiguration(settings);
            }

            public void Dispose()
            {
                Configuration?.Dispose();
            }
        }

        public class GenericService<T>
        {
        }
    }
}
