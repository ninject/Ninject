namespace Ninject.Tests.Integration.StandardKernelTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;

    using Ninject.Parameters;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class StandardKernelContext : IDisposable
    {
        protected StandardKernel kernel;

        public StandardKernelContext()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }
    }

    public class WhenGetIsCalledForInterfaceBoundService : StandardKernelContext
    {
        [Fact]
        public void SingleInstanceIsReturnedWhenOneBindingIsRegistered()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();

            var weapon = this.kernel.Get<IWeapon>();

            weapon.Should().NotBeNull();
            weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void ActivationExceptionThrownWhenMultipleBindingsAreRegistered()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>();

            var exception = Assert.Throws<ActivationException>(() => this.kernel.Get<IWeapon>());

            exception.Message.Should().Contain("More than one matching bindings are available.");
            exception.Message.Should().Contain("1) binding from IWeapon to Sword");
            exception.Message.Should().Contain("2) binding from IWeapon to Shuriken");
        }

        [Fact]
        public void DependenciesAreInjectedViaConstructor()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWarrior>().To<Samurai>();

            var warrior = this.kernel.Get<IWarrior>();

            warrior.Should().NotBeNull();
            warrior.Should().BeOfType<Samurai>();
            warrior.Weapon.Should().NotBeNull();
            warrior.Weapon.Should().BeOfType<Sword>();
        }
    }

    public class WhenGetIsCalledForSelfBoundService : StandardKernelContext
    {
        [Fact]
        public void SingleInstanceIsReturnedWhenOneBindingIsRegistered()
        {
            this.kernel.Bind<Sword>().ToSelf();

            var weapon = this.kernel.Get<Sword>();

            weapon.Should().NotBeNull();
            weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void DependenciesAreInjectedViaConstructor()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<Samurai>().ToSelf();

            var samurai = this.kernel.Get<Samurai>();

            samurai.Should().NotBeNull();
            samurai.Weapon.Should().NotBeNull();
            samurai.Weapon.Should().BeOfType<Sword>();
        }
    }

    public class WhenGetIsCalledForUnboundService : StandardKernelContext
    {
        [Fact]
        public void ImplicitSelfBindingIsRegisteredAndActivated()
        {
            var weapon = this.kernel.Get<Sword>();

            weapon.Should().NotBeNull();
            weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void ImplicitSelfBindingForGenericTypeIsRegisteredAndActivated()
        {
            var service = this.kernel.Get<GenericService<int>>();

            service.Should().NotBeNull();
            service.Should().BeOfType<GenericService<int>>();
        }

        [Fact]
        public void ThrowsExceptionIfAnUnboundInterfaceIsRequested()
        {
            Assert.Throws<ActivationException>(() => this.kernel.Get<IWeapon>());
        }

        [Fact]
        public void ThrowsExceptionIfAnUnboundAbstractClassIsRequested()
        {
            Assert.Throws<ActivationException>(() => this.kernel.Get<AbstractWeapon>());
        }

        [Fact]
        public void ThrowsExceptionIfAnUnboundValueTypeIsRequested()
        {
            Assert.Throws<ActivationException>(() => this.kernel.Get<int>());
        }

        [Fact]
        public void ThrowsExceptionIfAStringIsRequestedWithNoBinding()
        {
            Assert.Throws<ActivationException>(() => this.kernel.Get<string>());
        }

        [Fact]
        public void ThrowsExceptionIfAnOpenGenericTypeIsRequested()
        {
            Assert.Throws<ActivationException>(() => this.kernel.Get(typeof(IGeneric<>)));
        }
    }

    public class WhenGetIsCalledForGenericServiceRegisteredViaOpenGenericType : StandardKernelContext
    {
        [Fact]
        public void GenericParametersAreInferred()
        {
            this.kernel.Bind(typeof(IGeneric<>)).To(typeof(GenericService<>));

            var service = this.kernel.Get<IGeneric<int>>();

            service.Should().NotBeNull();
            service.Should().BeOfType<GenericService<int>>();
        }
    }

    public class WhenTryGetIsCalledForInterfaceBoundService : StandardKernelContext
    {
        [Fact]
        public void SingleInstanceIsReturnedWhenOneBindingIsRegistered()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();

            var weapon = this.kernel.TryGet<IWeapon>();

            weapon.Should().NotBeNull();
            weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void NullIsReturnedWhenMultipleBindingsAreRegistered()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>();

            var weapon = this.kernel.TryGet<IWeapon>();

            weapon.Should().BeNull();
        }
    }

    public class WhenTryGetIsCalledForUnboundService : StandardKernelContext
    {
        [Fact]
        public void TryGetOfT_Parameters_ImplicitSelfBindingIsRegisteredAndActivatedIfTypeIsSelfBindable()
        {
            var weapon = this.kernel.TryGet<Sword>();
            weapon.Should().NotBeNull();
            weapon.Should().BeOfType<Sword>();

            var bindings = this.kernel.GetBindings(typeof(Sword));
            bindings.Should().HaveCount(1);
            bindings.Should().ContainSingle(b => b.IsImplicit);
        }

        [Fact]
        public void TryGetOfT_Parameters_ReturnsNullIfTypeIsNotSelfBindable()
        {
            var weapon = this.kernel.TryGet<IWeapon>();
            weapon.Should().BeNull();

            var bindings = this.kernel.GetBindings(typeof(IWeapon));
            bindings.Should().BeEmpty();
        }

        [Fact]
        public void TryGetOfT_Parameters_ReturnsNullIfTypeHasOnlyUnmetConditionalBindings()
        {
            this.kernel.Bind<IWeapon>().To<Sword>().When(ctx => false);

            var weapon = this.kernel.TryGet<IWeapon>();
            weapon.Should().BeNull();

            var bindings = this.kernel.GetBindings(typeof(IWeapon));
            bindings.Should().HaveCount(1);
        }

        [Fact]
        public void TryGetOfT_Parameters_ReturnsNullIfNoBindingForADependencyExists()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();

            var warrior = this.kernel.TryGet<IWarrior>();
            warrior.Should().BeNull();

            var bindings = this.kernel.GetBindings(typeof(IWarrior));
            bindings.Should().HaveCount(1);
        }

        [Fact]
        public void TryGetOfT_Parameters_ReturnsNullIfMultipleBindingsExistForADependency()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>();

            var warrior = this.kernel.TryGet<IWarrior>();
            warrior.Should().BeNull();

            var bindings = this.kernel.GetBindings(typeof(IWarrior));
            bindings.Should().HaveCount(1);
        }

        [Fact]
        public void TryGetOfT_Parameters_ReturnsNullIfOnlyUnmetConditionalBindingsExistForADependency()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<IWeapon>().To<Sword>().When(ctx => false);

            var warrior = this.kernel.TryGet<IWarrior>();
            warrior.Should().BeNull();

            var bindings = this.kernel.GetBindings(typeof(IWarrior));
            bindings.Should().HaveCount(1);
        }

        [Fact(Skip = "https://github.com/ninject/Ninject/issues/341")]
        public void TryGetOfT_NameAndParameters_ReturnsNullWhenNoMatchingBindingExistsAndRegistersImplicitSelfBindingIfTypeIsSelfBindable()
        {
            var weapon = this.kernel.TryGet<Sword>("a", Array.Empty<IParameter>());

            weapon.Should().BeNull();

            var bindings = this.kernel.GetBindings(typeof(Sword));
            bindings.Should().HaveCount(1);
            bindings.Should().OnlyContain(b => b.IsImplicit);
        }

        [Fact(Skip = "https://github.com/ninject/Ninject/issues/341")]
        public void TryGet_ServiceAndNameAndParameters_ReturnsNullWhenNoMatchingBindingExistsAndRegistersImplicitSelfBindingIfTypeIsSelfBindable()
        {
            var weapon = this.kernel.TryGet(typeof(Sword), "a", Array.Empty<IParameter>());

            weapon.Should().BeNull();

            var bindings = this.kernel.GetBindings(typeof(Sword));
            bindings.Should().HaveCount(1);
            bindings.Should().OnlyContain(b => b.IsImplicit);
        }

        [Fact]
        public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfNoBindingExistsAndTypeIsNotSelfBindable()
        {
            var weapon = this.kernel.TryGet(typeof(IWeapon), "a", Array.Empty<IParameter>());
            weapon.Should().BeNull();

            var bindings = this.kernel.GetBindings(typeof(IWeapon));
            bindings.Should().BeEmpty();
        }

        [Fact]
        public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfNoMatchingBindingExistsAndTypeIsNotSelfBindable()
        {
            this.kernel.Bind<IWeapon>().To<Sword>().Named("b");

            var weapon = this.kernel.TryGet(typeof(IWeapon), "a", Array.Empty<IParameter>());
            weapon.Should().BeNull();

            var bindings = this.kernel.GetBindings(typeof(IWeapon));
            bindings.Should().HaveCount(1);
        }

        [Fact]
        public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfTypeHasOnlyUnmetConditionalBindings()
        {
            this.kernel.Bind<IWeapon>().To<Sword>().When(ctx => false).Named("a");

            var weapon = this.kernel.TryGet(typeof(IWeapon), "a", Array.Empty<IParameter>());
            weapon.Should().BeNull();

            var bindings = this.kernel.GetBindings(typeof(IWeapon));
            bindings.Should().HaveCount(1);
        }

        [Fact]
        public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfNoBindingForADependencyExists()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>().Named("a");

            var warrior = this.kernel.TryGet(typeof(IWarrior), "a", Array.Empty<IParameter>());
            warrior.Should().BeNull();

            var bindings = this.kernel.GetBindings(typeof(IWarrior));
            bindings.Should().HaveCount(1);
        }

        [Fact]
        public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfMultipleBindingsExistForADependency()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>().Named("a");
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>();

            var warrior = this.kernel.TryGet(typeof(IWarrior), "a", Array.Empty<IParameter>());
            warrior.Should().BeNull();

            var bindings = this.kernel.GetBindings(typeof(IWarrior));
            bindings.Should().HaveCount(1);
        }

        [Fact]
        public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfOnlyUnmetConditionalBindingsExistForADependency()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<IWeapon>().To<Sword>().When(ctx => false);

            var warrior = this.kernel.TryGet(typeof(IWarrior), (metadata) => true, Array.Empty<IParameter>());
            warrior.Should().BeNull();

            var bindings = this.kernel.GetBindings(typeof(IWarrior));
            bindings.Should().HaveCount(1);
        }

        [Fact]
        public void TryGet_ServiceAndConstraintAndParameters_ImplicitSelfBindingIsRegisteredAndActivatedIfTypeIsSelfBindable()
        {
            var weapon = this.kernel.TryGet(typeof(Sword), (metadata) => true, Array.Empty<IParameter>());
            weapon.Should().NotBeNull();
            weapon.Should().BeOfType<Sword>();

            var bindings = this.kernel.GetBindings(typeof(Sword));
            bindings.Should().HaveCount(1);
        }

        [Fact]
        public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullIfTypeIsNotSelfBindable()
        {
            var weapon = this.kernel.TryGet(typeof(IWeapon), (metadata) => true, Array.Empty<IParameter>());
            weapon.Should().BeNull();

            var bindings = this.kernel.GetBindings(typeof(IWeapon));
            bindings.Should().HaveCount(0);
        }

        [Fact]
        public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullIfTypeHasOnlyUnmetConditionalBindings()
        {
            this.kernel.Bind<IWeapon>().To<Sword>().When(ctx => false);

            var weapon = this.kernel.TryGet(typeof(IWeapon), (metadata) => true, Array.Empty<IParameter>());
            weapon.Should().BeNull();

            var bindings = this.kernel.GetBindings(typeof(IWeapon));
            bindings.Should().HaveCount(1);
        }

        [Fact]
        public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullIfNoBindingForADependencyExists()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();

            var warrior = this.kernel.TryGet(typeof(IWarrior), (metadata) => true, Array.Empty<IParameter>());
            warrior.Should().BeNull();

            var bindings = this.kernel.GetBindings(typeof(IWarrior));
            bindings.Should().HaveCount(1);
        }

        [Fact]
        public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullIfMultipleBindingsExistForADependency()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>();

            var warrior = this.kernel.TryGet(typeof(IWarrior), (metadata) => true, Array.Empty<IParameter>());
            warrior.Should().BeNull();

            var bindings = this.kernel.GetBindings(typeof(IWarrior));
            bindings.Should().HaveCount(1);
        }

        [Fact]
        public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullIfOnlyUnmetConditionalBindingsExistForADependency()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<IWeapon>().To<Sword>().When(ctx => false);

            var warrior = this.kernel.TryGet(typeof(IWarrior), (metadata) => true, Array.Empty<IParameter>());
            warrior.Should().BeNull();

            var bindings = this.kernel.GetBindings(typeof(IWarrior));
            bindings.Should().HaveCount(1);
        }
    }

    public class WhenTryGetIsCalledForServiceWithMultipleBindingsOfSameWeight : StandardKernelContext
    {
        [Fact]
        public void TryGetOfT_Parameters()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>();

            var weapon = this.kernel.TryGet<IWeapon>(Array.Empty<IParameter>());
            weapon.Should().BeNull();

            var bindings = this.kernel.GetBindings(typeof(IWeapon));
            bindings.Should().HaveCount(2);
        }

        [Fact]
        public void TryGetOfT_NameAndParameters()
        {
            this.kernel.Bind<IWeapon>().To<Sword>().Named("a");
            this.kernel.Bind<IWeapon>().To<Shuriken>().Named("a");

            var weapon = this.kernel.TryGet<IWeapon>("a", Array.Empty<IParameter>());
            weapon.Should().BeNull();

            var bindings = this.kernel.GetBindings(typeof(IWeapon));
            bindings.Should().HaveCount(2);
        }

        [Fact]
        public void TryGetOfT_ConstraintAndParameters()
        {
            this.kernel.Bind<IWeapon>().To<Sword>().Named("a");
            this.kernel.Bind<IWeapon>().To<Shuriken>().Named("a");

            var weapon = this.kernel.TryGet<IWeapon>((metadata) => metadata.Name == "a", Array.Empty<IParameter>());
            weapon.Should().BeNull();

            var bindings = this.kernel.GetBindings(typeof(IWeapon));
            bindings.Should().HaveCount(2);
        }

        [Fact]
        public void TryGet_ServiceAndParameters()
        {
            var service = typeof(IWeapon);

            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>();

            var weapon = this.kernel.TryGet(service, Array.Empty<IParameter>());

            weapon.Should().BeNull();

            var bindings = this.kernel.GetBindings(service);
            bindings.Should().HaveCount(2);
            bindings.Should().OnlyContain(b => !b.IsImplicit);
        }

        [Fact(Skip = "Unique?")]
        public void TryGet_ServiceAndNameAndParameters_ResolvesUsingFirstMatchingBindingWhenTypeIsSelfBinding()
        {
            var service = typeof(Sword);

            this.kernel.Bind<Sword>().To<Sword>().Named("a");
            this.kernel.Bind<Sword>().To<ShortSword>().Named("a");

            var weapon = this.kernel.TryGet(service, "a", Array.Empty<IParameter>());
            weapon.Should().NotBeNull();
            weapon.Should().BeOfType<Sword>();

            var bindings = this.kernel.GetBindings(service);
            bindings.Should().HaveCount(2);
            bindings.Should().OnlyContain(b => !b.IsImplicit);
        }


        [Fact(Skip = "Unique?")]
        public void TryGet_ServiceAndNameAndParameters_ResolvesUsingFirstMatchingBindingWhenTypeIsNotSelfBinding()
        {
            var service = typeof(IWeapon);

            this.kernel.Bind<IWeapon>().To<Sword>().Named("a");
            this.kernel.Bind<IWeapon>().To<Shuriken>().Named("a");

            var weapon = this.kernel.TryGet(service, "a", Array.Empty<IParameter>());
            weapon.Should().NotBeNull();
            weapon.Should().BeOfType<Sword>();

            var bindings = this.kernel.GetBindings(service);
            bindings.Should().HaveCount(2);
            bindings.Should().OnlyContain(b => !b.IsImplicit);
        }

        [Fact(Skip = "Unique?")]
        public void TryGet_ServiceAndConstraintAndParameters_ResolvesUsingFirstMatchingBindingWhenTypeIsSelfBinding()
        {
            var service = typeof(Sword);

            this.kernel.Bind<Sword>().To<ShortSword>().Named("b");
            this.kernel.Bind<Sword>().To<Sword>().Named("a");
            this.kernel.Bind<Sword>().To<ShortSword>().Named("a");

            var weapon = this.kernel.TryGet(service, (metadata) => metadata.Name == "a", Array.Empty<IParameter>());

            weapon.Should().NotBeNull();
            weapon.Should().BeOfType<Sword>();

            var bindings = this.kernel.GetBindings(service);
            bindings.Should().HaveCount(3);
            bindings.Should().OnlyContain(b => !b.IsImplicit);
        }

        [Fact(Skip = "Unique?")]
        public void TryGet_ServiceAndConstraintAndParameters_ResolvesUsingFirstMatchingBindingWhenTypeIsNotSelfBindingAndNotGeneric()
        {
            var service = typeof(IWeapon);

            this.kernel.Bind<IWeapon>().To<Sword>().Named("a");
            this.kernel.Bind<IWeapon>().To<Shuriken>().Named("a");

            var weapon = this.kernel.TryGet(service, (metadata) => metadata.Name == "a", Array.Empty<IParameter>());

            weapon.Should().NotBeNull();
            weapon.Should().BeOfType<Sword>();

            var bindings = this.kernel.GetBindings(service);
            bindings.Should().HaveCount(2);
            bindings.Should().OnlyContain(b => !b.IsImplicit);
        }

        [Fact]
        public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullWhenBindingsDoNotMatchAndTypeIsNotSelfBindingAndNotOpenGeneric()
        {
            var service = typeof(IWeapon);

            this.kernel.Bind<IWeapon>().To<Sword>().Named("b");
            this.kernel.Bind<IWeapon>().To<ShortSword>().Named("b");

            var weapon = this.kernel.TryGet(service, (metadata) => metadata.Name == "a", Array.Empty<IParameter>());

            weapon.Should().BeNull();

            var bindings = this.kernel.GetBindings(service);
            bindings.Should().HaveCount(2);
            bindings.Should().OnlyContain(b => !b.IsImplicit);
        }
    }

    public class WhenTryGetIsCalledForBoundListOfServices : StandardKernelContext
    {
        [Fact]
        public void TryGetOfT_Parameters()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>();

            var weapons = this.kernel.TryGet<List<IWeapon>>(Array.Empty<IParameter>());
            weapons.Should().NotBeNull();
            weapons.Should().HaveCount(2);
        }

        [Fact]
        public void TryGetOfT_Parameters_ShouldPreferBindingForList()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>();
            this.kernel.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() });

            var weapons = this.kernel.TryGet<List<IWeapon>>(Array.Empty<IParameter>());
            weapons.Should().NotBeNull();
            weapons.Should().HaveCount(1);
            weapons.Should().AllBeOfType<Dagger>();
        }

        [Fact]
        public void TryGetOfT_NameAndParameters()
        {
            this.kernel.Bind<IWeapon>().To<Dagger>().Named("b");
            this.kernel.Bind<IWeapon>().To<Sword>().Named("a");
            this.kernel.Bind<IWeapon>().To<Shuriken>().Named("a");

            var weapons = this.kernel.TryGet<List<IWeapon>>("a", Array.Empty<IParameter>());
            weapons.Should().NotBeNull();
            weapons.Should().HaveCount(3);

            var bindings = this.kernel.GetBindings(typeof(List<IWeapon>));
            bindings.Should().HaveCount(0);
        }

        [Fact]
        public void TryGetOfT_NameAndParameters_ShouldPreferBindingForList()
        {
            this.kernel.Bind<IWeapon>().To<Sword>().Named("a");
            this.kernel.Bind<IWeapon>().To<Shuriken>().Named("a");
            this.kernel.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Sword() }).Named("b");
            this.kernel.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() }).Named("a");

            var weapons = this.kernel.TryGet<List<IWeapon>>("a", Array.Empty<IParameter>());
            weapons.Should().NotBeNull();
            weapons.Should().HaveCount(1);
            weapons.Should().AllBeOfType<Dagger>();

            var bindings = this.kernel.GetBindings(typeof(List<IWeapon>));
            bindings.Should().HaveCount(2);
        }

        [Fact]
        public void TryGetOfT_ConstraintAndParameters()
        {
            this.kernel.Bind<IWeapon>().To<Dagger>().Named("b");
            this.kernel.Bind<IWeapon>().To<Sword>().Named("a");
            this.kernel.Bind<IWeapon>().To<Shuriken>().Named("a");

            var weapons = this.kernel.TryGet<List<IWeapon>>((metadata) => metadata.Name == "a", Array.Empty<IParameter>());
            weapons.Should().NotBeNull();
            weapons.Should().HaveCount(3);

            var bindings = this.kernel.GetBindings(typeof(List<IWeapon>));
            bindings.Should().HaveCount(0);
        }

        [Fact]
        public void TryGetOfT_ConstraintAndParameters_ShouldPreferBindingForList()
        {
            this.kernel.Bind<IWeapon>().To<Sword>().Named("a");
            this.kernel.Bind<IWeapon>().To<Shuriken>().Named("a");
            this.kernel.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Sword() }).Named("b");
            this.kernel.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() }).Named("a");

            var weapons = this.kernel.TryGet<List<IWeapon>>((metadata) => metadata.Name == "a", Array.Empty<IParameter>());
            weapons.Should().NotBeNull();
            weapons.Should().HaveCount(1);
            weapons.Should().AllBeOfType<Dagger>();

            var bindings = this.kernel.GetBindings(typeof(List<IWeapon>));
            bindings.Should().HaveCount(2);
        }


        [Fact]
        public void TryGet_ServiceAndParameters()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>();

            var weapons = this.kernel.TryGet(typeof(List<IWeapon>), Array.Empty<IParameter>());
            weapons.Should().NotBeNull();
            weapons.Should().BeOfType<List<IWeapon>>();

            var weaponsList = (List<IWeapon>)weapons;
            weaponsList.Should().HaveCount(2);

            var bindings = this.kernel.GetBindings(typeof(List<IWeapon>));
            bindings.Should().HaveCount(0);
        }

        [Fact]
        public void TryGet_ServiceAndParameters_ShouldPreferBindingForList()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>();
            this.kernel.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() });

            var weapons = this.kernel.TryGet(typeof(List<IWeapon>), Array.Empty<IParameter>());
            weapons.Should().NotBeNull();
            weapons.Should().BeOfType<List<IWeapon>>();

            var weaponsList = (List<IWeapon>)weapons;
            weaponsList.Should().HaveCount(1);
            weaponsList.Should().AllBeOfType<Dagger>();

            var bindings = this.kernel.GetBindings(typeof(List<IWeapon>));
            bindings.Should().HaveCount(1);
        }

        [Fact]
        public void TryGet_ServiceAndNameAndParameters()
        {
            this.kernel.Bind<IWeapon>().To<Dagger>().Named("b");
            this.kernel.Bind<IWeapon>().To<Sword>().Named("a");
            this.kernel.Bind<IWeapon>().To<Shuriken>().Named("a");

            var weapons = this.kernel.TryGet(typeof(List<IWeapon>), "a", Array.Empty<IParameter>());

            weapons.Should().NotBeNull();
            weapons.Should().BeOfType<List<IWeapon>>();

            var weaponsList = (List<IWeapon>)weapons;
            weaponsList.Should().HaveCount(3);

            var bindings = this.kernel.GetBindings(typeof(List<IWeapon>));
            bindings.Should().HaveCount(0);
        }

        [Fact]
        public void TryGet_ServiceAndNameAndParameters_ShouldPreferBindingForList()
        {
            this.kernel.Bind<IWeapon>().To<Sword>().Named("a");
            this.kernel.Bind<IWeapon>().To<Shuriken>().Named("a");
            this.kernel.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Sword() }).Named("b");
            this.kernel.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() }).Named("a");

            var weapons = this.kernel.TryGet(typeof(List<IWeapon>), "a", Array.Empty<IParameter>());
            weapons.Should().NotBeNull();
            weapons.Should().BeOfType<List<IWeapon>>();

            var weaponsList = (List<IWeapon>)weapons;
            weaponsList.Should().HaveCount(1);
            weaponsList.Should().AllBeOfType<Dagger>();

            var bindings = this.kernel.GetBindings(typeof(List<IWeapon>));
            bindings.Should().HaveCount(2);
        }


        [Fact]
        public void TryGet_ServiceAndConstraintAndParameters()
        {
            this.kernel.Bind<IWeapon>().To<Dagger>().Named("b");
            this.kernel.Bind<IWeapon>().To<Sword>().Named("a");
            this.kernel.Bind<IWeapon>().To<Shuriken>().Named("a");

            var weapons = this.kernel.TryGet(typeof(List<IWeapon>), (metadata) => metadata.Name == "a", Array.Empty<IParameter>());
            weapons.Should().NotBeNull();
            weapons.Should().BeOfType<List<IWeapon>>();

            var weaponsList = (List<IWeapon>)weapons;
            weaponsList.Should().HaveCount(3);

            var bindings = this.kernel.GetBindings(typeof(List<IWeapon>));
            bindings.Should().HaveCount(0);
        }

        [Fact]
        public void TryGet_ServiceAndConstraintAndParameters_ShouldPreferBindingForList()
        {
            this.kernel.Bind<IWeapon>().To<Sword>().Named("a");
            this.kernel.Bind<IWeapon>().To<Shuriken>().Named("a");
            this.kernel.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Sword() }).Named("b");
            this.kernel.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() }).Named("a");

            var weapons = this.kernel.TryGet(typeof(List<IWeapon>), (metadata) => metadata.Name == "a", Array.Empty<IParameter>());
            weapons.Should().NotBeNull();
            weapons.Should().BeOfType<List<IWeapon>>();

            var weaponsList = (List<IWeapon>)weapons;
            weaponsList.Should().HaveCount(1);
            weaponsList.Should().AllBeOfType<Dagger>();

            var bindings = this.kernel.GetBindings(typeof(List<IWeapon>));
            bindings.Should().HaveCount(2);
        }


        [Fact]
        public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullWhenTypeIsUnboundGenericTypeDefinition()
        {
            var service = typeof(List<>);

            this.kernel.Bind<List<int>>().ToConstant(new List<int> { 1 }).Named("a");

            var weapon = this.kernel.TryGet(service, (metadata) => metadata.Name == "a", Array.Empty<IParameter>());

            weapon.Should().BeNull();

            var bindings = this.kernel.GetBindings(service);
            bindings.Should().BeEmpty();
        }
    }

    public class WhenTryGetIsCalledForUnboundListOfServices : StandardKernelContext
    {
        [Fact]
        public void TryGetOfT_Parameters()
        {
            var weapons = this.kernel.TryGet<List<IWeapon>>(Array.Empty<IParameter>());

            weapons.Should().NotBeNull();
            weapons.Should().BeEmpty();

            var bindings = this.kernel.GetBindings(typeof(List<IWeapon>));
            bindings.Should().BeEmpty();
        }

        [Fact]
        public void TryGetOfT_NameAndParameters()
        {
            var weapons = this.kernel.TryGet<List<IWeapon>>("b", Array.Empty<IParameter>());

            weapons.Should().NotBeNull();
            weapons.Should().BeEmpty();

            var bindings = this.kernel.GetBindings(typeof(List<IWeapon>));
            bindings.Should().BeEmpty();
        }

        [Fact]
        public void TryGetOfT_ConstraintAndParameters()
        {
            var weapons = this.kernel.TryGet<List<IWeapon>>((metadata) => metadata.Name == "b", Array.Empty<IParameter>());
            weapons.Should().NotBeNull();
            weapons.Should().BeEmpty();

            var bindings = this.kernel.GetBindings(typeof(List<IWeapon>));
            bindings.Should().BeEmpty();
        }

        [Fact]
        public void TryGet_ServiceAndParameters()
        {
            var weapons = this.kernel.TryGet(typeof(List<IWeapon>), Array.Empty<IParameter>());
            weapons.Should().NotBeNull();
            weapons.Should().BeOfType<List<IWeapon>>();

            var weaponsList = (List<IWeapon>)weapons;
            weaponsList.Should().BeEmpty();

            var bindings = this.kernel.GetBindings(typeof(List<IWeapon>));
            bindings.Should().BeEmpty();
        }

        [Fact]
        public void TryGet_ServiceAndNameAndParameters()
        {
            var weapons = this.kernel.TryGet(typeof(List<IWeapon>), "a", Array.Empty<IParameter>());
            weapons.Should().NotBeNull();
            weapons.Should().BeOfType<List<IWeapon>>();

            var weaponsList = (List<IWeapon>)weapons;
            weaponsList.Should().BeEmpty();

            var bindings = this.kernel.GetBindings(typeof(List<IWeapon>));
            bindings.Should().BeEmpty();
        }

        [Fact]
        public void TryGet_ServiceAndConstraintAndParameters()
        {
            var weapons = this.kernel.TryGet(typeof(List<IWeapon>), (metadata) => metadata.Name == "a", Array.Empty<IParameter>());
            weapons.Should().NotBeNull();
            weapons.Should().BeOfType<List<IWeapon>>();

            var weaponsList = (List<IWeapon>)weapons;
            weaponsList.Should().BeEmpty();

            var bindings = this.kernel.GetBindings(typeof(List<IWeapon>));
            bindings.Should().BeEmpty();
        }

        [Fact]
        public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullWhenTypeIsUnboundGenericTypeDefinition()
        {
            var service = typeof(List<>);

            var weapon = this.kernel.TryGet(service, (metadata) => metadata.Name == "a", Array.Empty<IParameter>());

            weapon.Should().BeNull();

            var bindings = this.kernel.GetBindings(service);
            bindings.Should().BeEmpty();
        }
    }


    public class WhenTryGetAndThrowOnInvalidBindingIsCalledForInterfaceBoundService : StandardKernelContext
    {
        [Fact]
        public void SingleInstanceIsReturnedWhenOneBindingIsRegistered()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();

            var weapon = this.kernel.TryGetAndThrowOnInvalidBinding<IWeapon>();

            weapon.Should().NotBeNull();
            weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void ConditionalBindingInstanceIsReturnedWhenConditionalAndUnconditionalBindingAreRegisteredAndConditionIsMet()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>().When(ctx => true);

            var weapon = this.kernel.TryGetAndThrowOnInvalidBinding<IWeapon>();

            weapon.Should().NotBeNull();
            weapon.Should().BeOfType<Shuriken>();
        }

        [Fact]
        public void UnconditionalBindingInstanceIsReturnedWhenConditionalAndUnconditionalBindingAreRegisteredAndConditionIsUnmet()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>().When(ctx => false);

            var weapon = this.kernel.TryGetAndThrowOnInvalidBinding<IWeapon>();

            weapon.Should().NotBeNull();
            weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void ThrowsActivationExceptionWhenSingleInvalidBindingIsRegistered()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();

            this.kernel.Invoking(x => x.TryGetAndThrowOnInvalidBinding<IWarrior>())
                .Should().Throw<ActivationException>();
        }

        [Fact]
        public void ThrowsActivationExceptionWhenMultipleBindingsAreRegistered()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>();

            this.kernel.Invoking(x => x.TryGetAndThrowOnInvalidBinding<IWeapon>())
                .Should().Throw<ActivationException>();
        }

        [Fact]
        public void ThrowsActivationExceptionWhenMultipleBindingsExistForADependency()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>();

            this.kernel.Invoking(x => x.TryGetAndThrowOnInvalidBinding<IWarrior>())
                .Should().Throw<ActivationException>();
        }

        [Fact]
        public void ThrowsActivationExceptionWhenOnlyUnmetConditionalBindingsExistForADependency()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<IWeapon>().To<Sword>().When(ctx => false);

            this.kernel.Invoking(x => x.TryGetAndThrowOnInvalidBinding<IWarrior>())
                .Should().Throw<ActivationException>();
        }
    }

    public class WhenTryGetAndThrowOnInvalidBindingIsCalledForUnboundService : StandardKernelContext
    {
        [Fact]
        public void ImplicitSelfBindingIsRegisteredAndActivatedIfTypeIsSelfBindable()
        {
            var weapon = this.kernel.TryGetAndThrowOnInvalidBinding<Sword>();

            weapon.Should().NotBeNull();
            weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void ReturnsNullIfTypeIsNotSelfBindable()
        {
            var weapon = this.kernel.TryGetAndThrowOnInvalidBinding<IWeapon>();
            weapon.Should().BeNull();
        }

        [Fact]
        public void ReturnsNullIfTypeHasOnlyUnmetConditionalBindings()
        {
            this.kernel.Bind<IWeapon>().To<Sword>().When(ctx => false);

            var weapon = this.kernel.TryGetAndThrowOnInvalidBinding<IWeapon>();
            weapon.Should().BeNull();
        }
    }

    public class WhenGetAllIsCalledForInterfaceBoundService : StandardKernelContext
    {
        [Fact]
        public void ReturnsSeriesOfItemsInOrderTheyWereBound()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>();

            var weapons = this.kernel.GetAll<IWeapon>().ToArray();

            weapons.Should().NotBeNull();
            weapons.Length.Should().Be(2);
            weapons[0].Should().BeOfType<Sword>();
            weapons[1].Should().BeOfType<Shuriken>();
        }

        [Fact]
        public void DoesNotActivateItemsUntilTheEnumeratorRunsOverThem()
        {
            InitializableA.Count = 0;
            InitializableB.Count = 0;
            this.kernel.Bind<IInitializable>().To<InitializableA>();
            this.kernel.Bind<IInitializable>().To<InitializableB>();

            IEnumerable<IInitializable> instances = this.kernel.GetAll<IInitializable>();
            IEnumerator<IInitializable> enumerator = instances.GetEnumerator();

            InitializableA.Count.Should().Be(0);
            enumerator.MoveNext();
            InitializableA.Count.Should().Be(1);
            InitializableB.Count.Should().Be(0);
            enumerator.MoveNext();
            InitializableA.Count.Should().Be(1);
            InitializableB.Count.Should().Be(1);
        }
    }

    public class WhenGetAllIsCalledForGenericServiceRegisteredViaOpenGenericType : StandardKernelContext
    {
        [Fact]
        public void GenericParametersAreInferred()
        {
            this.kernel.Bind(typeof(IGeneric<>)).To(typeof(GenericService<>));
            this.kernel.Bind(typeof(IGeneric<>)).To(typeof(GenericService2<>));

            var services = this.kernel.GetAll<IGeneric<int>>().ToArray();

            services.Should().NotBeNull();
            services.Length.Should().Be(2);
            services[0].Should().BeOfType<GenericService<int>>();
            services[1].Should().BeOfType<GenericService2<int>>();
        }

        [Fact]
        public void OpenGenericBindingsCanBeOverriddenByClosedGenericBindings()
        {
            this.kernel.Bind(typeof(IGeneric<>)).To(typeof(GenericService<>));
            this.kernel.Bind<IGeneric<int>>().To<ClosedGenericService>();

            var service = this.kernel.Get<IGeneric<int>>();

            service.Should().BeOfType<ClosedGenericService>();
        }

        [Fact]
        public void OpenGenericsWithCoAndContraVarianceCanBeResolved()
        {
            this.kernel.Bind(typeof(IGenericCoContraVarianceService<,>)).To(typeof(OpenGenericCoContraVarianceService<,>));

            var service = this.kernel.Get<IGenericCoContraVarianceService<string, int>>();

            service.Should().BeOfType<OpenGenericCoContraVarianceService<string, int>>();
        }

        [Fact]
        public void ClosedGenericsWithCoAndContraVarianceCanBeResolved()
        {
            this.kernel.Bind(typeof(IGenericCoContraVarianceService<string, int>)).To(typeof(ClosedGenericCoContraVarianceService));

            var service = this.kernel.Get<IGenericCoContraVarianceService<string, int>>();

            service.Should().BeOfType<ClosedGenericCoContraVarianceService>();
        }
    }

    public class WhenGetAllIsCalledForUnboundService : StandardKernelContext
    {
        [Fact]
        public void ImplicitSelfBindingIsRegisteredAndActivatedIfTypeIsSelfBindable()
        {
            var weapons = this.kernel.GetAll<Sword>().ToArray();

            weapons.Should().NotBeNull();
            weapons.Length.Should().Be(1);
            weapons[0].Should().BeOfType<Sword>();
        }

        [Fact]
        public void ReturnsEmptyEnumerableIfTypeIsNotSelfBindable()
        {
            var weapons = this.kernel.GetAll<IWeapon>().ToArray();

            weapons.Should().NotBeNull();
            weapons.Length.Should().Be(0);
        }
    }

    public class WhenGetIsCalledForProviderBoundService : StandardKernelContext
    {
        [Fact]
        public void WhenProviderReturnsNullThenActivationExceptionIsThrown()
        {
            this.kernel.Bind<IWeapon>().ToProvider<NullProvider>();

            Assert.Throws<ActivationException>(() => this.kernel.Get<IWeapon>());
        }

        [Fact]
        public void WhenProviderReturnsNullButAllowedInSettingsThenNullIsResolved()
        {
            this.kernel.Settings.AllowNullInjection = true;
            this.kernel.Bind<IWeapon>().ToProvider<NullProvider>();

            var weapon = this.kernel.Get<IWeapon>();

            weapon.Should().BeNull();
        }
    }

    public class WhenGetIsCalledWithConstraints : StandardKernelContext
    {
        [Fact]
        public void ReturnsServiceRegisteredViaBindingWithSpecifiedName()
        {
            this.kernel.Bind<IWeapon>().To<Shuriken>();
            this.kernel.Bind<IWeapon>().To<Sword>().Named("sword");

            var weapon = this.kernel.Get<IWeapon>("sword");

            weapon.Should().NotBeNull();
            weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void ReturnsServiceRegisteredViaBindingThatMatchesPredicate()
        {
            this.kernel.Bind<IWeapon>().To<Shuriken>().WithMetadata("type", "range");
            this.kernel.Bind<IWeapon>().To<Sword>().WithMetadata("type", "melee");

            var weapon = this.kernel.Get<IWeapon>(x => x.Get<string>("type") == "melee");

            weapon.Should().NotBeNull();
            weapon.Should().BeOfType<Sword>();
        }
    }

    public class WhenUnbindIsCalled : StandardKernelContext
    {
        [Fact]
        public void RemovesAllBindingsForService()
        {
            this.kernel.Bind<IWeapon>().To<Shuriken>();
            this.kernel.Bind<IWeapon>().To<Sword>();

            var bindings = this.kernel.GetBindings(typeof(IWeapon)).ToArray();
            bindings.Length.Should().Be(2);

            this.kernel.Unbind<IWeapon>();
            bindings = this.kernel.GetBindings(typeof(IWeapon)).ToArray();
            bindings.Should().BeEmpty();
        }
    }

    public class WhenRebindIsCalled : StandardKernelContext
    {
        [Fact]
        public void RemovesAllBindingsForServiceAndReplacesWithSpecifiedBinding()
        {
            this.kernel.Bind<IWeapon>().To<Shuriken>();
            this.kernel.Bind<IWeapon>().To<Sword>();

            var bindings = this.kernel.GetBindings(typeof(IWeapon)).ToArray();
            bindings.Length.Should().Be(2);

            this.kernel.Rebind<IWeapon>().To<Sword>();
            bindings = this.kernel.GetBindings(typeof(IWeapon)).ToArray();
            bindings.Length.Should().Be(1);
        }
    }

    public class WhenCanResolveIsCalled : StandardKernelContext
    {
        [Fact]
        public void ForImplicitBindings()
        {
            this.kernel.Get<Sword>();
            var request = this.kernel.CreateRequest(typeof(Sword), null, Array.Empty<IParameter>(), false, true);

            this.kernel.CanResolve(request, true).Should().BeFalse();
            this.kernel.CanResolve(request, false).Should().BeTrue();
            this.kernel.CanResolve(request).Should().BeTrue();
        }
    }

    public class WhenDerivedClassWithPrivateGetterIsResolved
    {
        [Fact]
        public void ItCanBeResolved()
        {
            using (var kernel = new StandardKernel(
                new NinjectSettings
                {
                    UseReflectionBasedInjection = true,
                    InjectNonPublic = true,
                    InjectParentPrivateProperties = true
                }))
            {
                kernel.Get<DerivedClassWithPrivateGetter>();
            }
        }
    }

    public class WhenServiceIsResolvedThroughServiceProviderInterface : StandardKernelContext
    {
        [Fact]
        public void ItResolvesBoundService()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();

            var provider = this.kernel as IServiceProvider;
            provider.GetService(typeof(IWeapon)).Should().NotBeNull();
        }

        [Fact]
        public void ItReturnsNullWhenServiceIsNotConfigured()
        {
            var provider = this.kernel as IServiceProvider;
            provider.GetService(typeof(Samurai)).Should().BeNull();
        }

        [Fact]
        public void ItThrowsWhenServiceIsNotConfiguredAndSettingThrowOnGetServiceNotFound()
        {
            using (var kernel = new StandardKernel(new NinjectSettings
            {
                ThrowOnGetServiceNotFound = true
            }))
            {
                var provider = kernel as IServiceProvider;
                Action resolveAction = () => provider.GetService(typeof(Samurai));
                resolveAction.Should().Throw<ActivationException>();
            }
        }
    }

    public class InitializableA : IInitializable
    {
        public static int Count = 0;

        public void Initialize()
        {
            Count++;
        }
    }

    public class InitializableB : IInitializable
    {
        public static int Count = 0;

        public void Initialize()
        {
            Count++;
        }
    }

    public class ClassWithPrivateGetter
    {
        float Value
        {
            get { return 0f; }
        }
    }

    public class DerivedClassWithPrivateGetter : ClassWithPrivateGetter { }
    public interface IGeneric<T> { }
    public class GenericService<T> : IGeneric<T> { }
    public class GenericService2<T> : IGeneric<T> { }
    public class ClosedGenericService : IGeneric<int> { }
    public interface IGenericWithConstraints<T> where T : class { }
    public class GenericServiceWithConstraints<T> : IGenericWithConstraints<T> where T : class { }
    public interface IGenericCoContraVarianceService<in T, out TK> { }
    public class ClosedGenericCoContraVarianceService : IGenericCoContraVarianceService<string, int> { }
    public class OpenGenericCoContraVarianceService<T, TK> : IGenericCoContraVarianceService<T, TK> { }


    public class NullProvider : Ninject.Activation.Provider<Sword>
    {
        protected override Sword CreateInstance(Activation.IContext context)
        {
            return null;
        }
    }
}