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
        public void ImplicitSelfBindingIsRegisteredAndActivatedIfTypeIsSelfBindable()
        {
            var weapon = this.kernel.TryGet<Sword>();

            weapon.Should().NotBeNull();
            weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void ReturnsNullIfTypeIsNotSelfBindable()
        {
            var weapon = this.kernel.TryGet<IWeapon>();
            weapon.Should().BeNull();
        }

        [Fact]
        public void ReturnsNullIfTypeHasOnlyUnmetConditionalBindings()
        {
            this.kernel.Bind<IWeapon>().To<Sword>().When(ctx => false);

            var weapon = this.kernel.TryGet<IWeapon>();
            weapon.Should().BeNull();
        }

        [Fact]
        public void ReturnsNullIfNoBindingForADependencyExists()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();

            var warrior = this.kernel.TryGet<IWarrior>();
            warrior.Should().BeNull();
        }

        [Fact]
        public void ReturnsNullIfMultipleBindingsExistForADependency()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>();

            var warrior = this.kernel.TryGet<IWarrior>();
            warrior.Should().BeNull();
        }

        [Fact]
        public void ReturnsNullIfOnlyUnmetConditionalBindingsExistForADependency()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<IWeapon>().To<Sword>().When(ctx => false);

            var warrior = this.kernel.TryGet<IWarrior>();
            warrior.Should().BeNull();
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
            var request = this.kernel.CreateRequest(typeof(Sword), null, Enumerable.Empty<IParameter>(), false, true);

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
    public interface IGenericCoContraVarianceService<in T, out TK> {}
    public class ClosedGenericCoContraVarianceService : IGenericCoContraVarianceService<string, int> { }
    public class OpenGenericCoContraVarianceService<T, TK> : IGenericCoContraVarianceService<T, TK> { }


    public class NullProvider : Ninject.Activation.Provider<Sword>
    {
        protected override Sword CreateInstance (Activation.IContext context)
        {
            return null;
        }
    }
}