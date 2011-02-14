namespace Ninject.Tests.Integration.StandardKernelTests
{
    using System.Collections.Generic;
    using System.Linq;

    using Ninject.Tests.Fakes;
#if SILVERLIGHT
#if SILVERLIGHT_MSTEST
    using MsTest.Should;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Assert = AssertWithThrows;
    using Fact = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
#else
    using UnitDriven;
    using UnitDriven.Should;
    using Assert = AssertWithThrows;
    using Fact = UnitDriven.TestMethodAttribute;
#endif
#else
    using Ninject.Tests.MSTestAttributes;
    using Xunit;
    using Xunit.Should;
#endif

    public class StandardKernelContext
    {
        protected StandardKernel kernel;

        public StandardKernelContext()
        {
            this.SetUp();
        }

        [TestInitialize]
        public void SetUp()
        {
            this.kernel = new StandardKernel();
        }
    }

    [TestClass]
    public class WhenGetIsCalledForInterfaceBoundService : StandardKernelContext
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
        public void ActivationExceptionThrownWhenMultipleBindingsAreRegistered()
        {
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWeapon>().To<Shuriken>();

            var exception = Assert.Throws<ActivationException>(() => kernel.Get<IWeapon>());
            
            exception.Message.ShouldContain("More than one matching bindings are available.");
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

    [TestClass]
    public class WhenGetIsCalledForSelfBoundService : StandardKernelContext
    {
        [Fact]
        public void SingleInstanceIsReturnedWhenOneBindingIsRegistered()
        {
            kernel.Bind<Sword>().ToSelf();

            var weapon = kernel.Get<Sword>();

            weapon.ShouldNotBeNull();
            weapon.ShouldBeInstanceOf<Sword>();
        }

        [Fact]
        public void DependenciesAreInjectedViaConstructor()
        {
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<Samurai>().ToSelf();

            var samurai = kernel.Get<Samurai>();

            samurai.ShouldNotBeNull();
            samurai.Weapon.ShouldNotBeNull();
            samurai.Weapon.ShouldBeInstanceOf<Sword>();
        }
    }

    [TestClass]
    public class WhenGetIsCalledForUnboundService : StandardKernelContext
    {
        [Fact]
        public void ImplicitSelfBindingIsRegisteredAndActivated()
        {
            var weapon = kernel.Get<Sword>();

            weapon.ShouldNotBeNull();
            weapon.ShouldBeInstanceOf<Sword>();
        }

        [Fact]
        public void ImplicitSelfBindingForGenericTypeIsRegisteredAndActivated()
        {
            var service = kernel.Get<GenericService<int>>();

            service.ShouldNotBeNull();
            service.ShouldBeInstanceOf<GenericService<int>>();
        }

        [Fact]
        public void ThrowsExceptionIfAnUnboundInterfaceIsRequested()
        {
            Assert.Throws<ActivationException>(() => kernel.Get<IWeapon>());
        }

        [Fact]
        public void ThrowsExceptionIfAnUnboundAbstractClassIsRequested()
        {
            Assert.Throws<ActivationException>(() => kernel.Get<AbstractWeapon>());
        }

        [Fact]
        public void ThrowsExceptionIfAnUnboundValueTypeIsRequested()
        {
            Assert.Throws<ActivationException>(() => kernel.Get<int>());
        }

        [Fact]
        public void ThrowsExceptionIfAStringIsRequestedWithNoBinding()
        {
            Assert.Throws<ActivationException>(() => kernel.Get<string>());
        }

        [Fact]
        public void ThrowsExceptionIfAnOpenGenericTypeIsRequested()
        {
            Assert.Throws<ActivationException>(() => kernel.Get(typeof(IGeneric<>)));
        }
    }

    [TestClass]
    public class WhenGetIsCalledForGenericServiceRegisteredViaOpenGenericType : StandardKernelContext
    {
        [Fact]
        public void GenericParametersAreInferred()
        {
            kernel.Bind(typeof(IGeneric<>)).To(typeof(GenericService<>));

            var service = kernel.Get<IGeneric<int>>();

            service.ShouldNotBeNull();
            service.ShouldBeInstanceOf<GenericService<int>>();
        }
    }

    [TestClass]
    public class WhenTryGetIsCalledForInterfaceBoundService : StandardKernelContext
    {
        [Fact]
        public void SingleInstanceIsReturnedWhenOneBindingIsRegistered()
        {
            kernel.Bind<IWeapon>().To<Sword>();

            var weapon = kernel.TryGet<IWeapon>();

            weapon.ShouldNotBeNull();
            weapon.ShouldBeInstanceOf<Sword>();
        }

        [Fact]
        public void NullIsReturnedWhenMultipleBindingsAreRegistered()
        {
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWeapon>().To<Shuriken>();

            var weapon = kernel.TryGet<IWeapon>();

            weapon.ShouldBeNull();
        }
    }

    [TestClass]
    public class WhenTryGetIsCalledForUnboundService : StandardKernelContext
    {
        [Fact]
        public void ImplicitSelfBindingIsRegisteredAndActivatedIfTypeIsSelfBindable()
        {
            var weapon = kernel.TryGet<Sword>();

            weapon.ShouldNotBeNull();
            weapon.ShouldBeInstanceOf<Sword>();
        }

        [Fact]
        public void ReturnsNullIfTypeIsNotSelfBindable()
        {
            var weapon = kernel.TryGet<IWeapon>();
            weapon.ShouldBeNull();
        }

        [Fact]
        public void ReturnsNullIfTypeHasOnlyUnmetConditionalBindings()
        {
            this.kernel.Bind<IWeapon>().To<Sword>().When(ctx => false);

            var weapon = kernel.TryGet<IWeapon>();
            weapon.ShouldBeNull();
        }

        [Fact]
        public void ReturnsNullIfNoBindingForADependencyExists()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();

            var warrior = kernel.TryGet<IWarrior>();
            warrior.ShouldBeNull();
        }

        [Fact]
        public void ReturnsNullIfMultipleBindingsExistForADependency()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWeapon>().To<Shuriken>();

            var warrior = kernel.TryGet<IWarrior>();
            warrior.ShouldBeNull();
        }

        [Fact]
        public void ReturnsNullIfOnlyUnmetConditionalBindingsExistForADependency()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();
            kernel.Bind<IWeapon>().To<Sword>().When(ctx => false);

            var warrior = kernel.TryGet<IWarrior>();
            warrior.ShouldBeNull();
        }
    }

    [TestClass]
    public class WhenGetAllIsCalledForInterfaceBoundService : StandardKernelContext
    {
        [Fact]
        public void ReturnsSeriesOfItemsInOrderTheyWereBound()
        {
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWeapon>().To<Shuriken>();

            var weapons = kernel.GetAll<IWeapon>().ToArray();

            weapons.ShouldNotBeNull();
            weapons.Length.ShouldBe(2);
            weapons[0].ShouldBeInstanceOf<Sword>();
            weapons[1].ShouldBeInstanceOf<Shuriken>();
        }

        [Fact]
        public void DoesNotActivateItemsUntilTheEnumeratorRunsOverThem()
        {
            kernel.Bind<IInitializable>().To<InitializableA>();
            kernel.Bind<IInitializable>().To<InitializableB>();

            IEnumerable<IInitializable> instances = kernel.GetAll<IInitializable>();
            IEnumerator<IInitializable> enumerator = instances.GetEnumerator();

            InitializableA.Count.ShouldBe(0);
            enumerator.MoveNext();
            InitializableA.Count.ShouldBe(1);
            InitializableB.Count.ShouldBe(0);
            enumerator.MoveNext();
            InitializableA.Count.ShouldBe(1);
            InitializableB.Count.ShouldBe(1);
        }
    }

    [TestClass]
    public class WhenGetAllIsCalledForGenericServiceRegisteredViaOpenGenericType : StandardKernelContext
    {
        [Fact]
        public void GenericParametersAreInferred()
        {
            kernel.Bind(typeof(IGeneric<>)).To(typeof(GenericService<>));
            kernel.Bind(typeof(IGeneric<>)).To(typeof(GenericService2<>));

            var services = kernel.GetAll<IGeneric<int>>().ToArray();

            services.ShouldNotBeNull();
            services.Length.ShouldBe(2);
            services[0].ShouldBeInstanceOf<GenericService<int>>();
            services[1].ShouldBeInstanceOf<GenericService2<int>>();
        }
    }

    [TestClass]
    public class WhenGetAllIsCalledForUnboundService : StandardKernelContext
    {
        [Fact]
        public void ImplicitSelfBindingIsRegisteredAndActivatedIfTypeIsSelfBindable()
        {
            var weapons = kernel.GetAll<Sword>().ToArray();

            weapons.ShouldNotBeNull();
            weapons.Length.ShouldBe(1);
            weapons[0].ShouldBeInstanceOf<Sword>();
        }

        [Fact]
        public void ReturnsEmptyEnumerableIfTypeIsNotSelfBindable()
        {
            var weapons = kernel.GetAll<IWeapon>().ToArray();

            weapons.ShouldNotBeNull();
            weapons.Length.ShouldBe(0);
        }
    }

    [TestClass]
    public class WhenGetIsCalledForProviderBoundService : StandardKernelContext
    {
        [Fact]
        public void WhenProviderReturnsNullThenActivationExceptionIsThrown()
        {
            kernel.Bind<IWeapon>().ToProvider<NullProvider>();
            
            Assert.Throws<Ninject.ActivationException>(() => kernel.Get<IWeapon>());
        }
    
        [Fact]
        public void WhenProviderReturnsNullButAllowedInSettingsThenNullIsResolved()
        {
            kernel.Settings.AllowNullInjection = true;
            kernel.Bind<IWeapon>().ToProvider<NullProvider>();

            var weapon = kernel.Get<IWeapon>();

            weapon.ShouldBeNull();
        }
    }

    [TestClass]
    public class WhenGetIsCalledWithConstraints : StandardKernelContext
    {
        [Fact]
        public void ReturnsServiceRegisteredViaBindingWithSpecifiedName()
        {
            kernel.Bind<IWeapon>().To<Shuriken>();
            kernel.Bind<IWeapon>().To<Sword>().Named("sword");

            var weapon = kernel.Get<IWeapon>("sword");

            weapon.ShouldNotBeNull();
            weapon.ShouldBeInstanceOf<Sword>();
        }

        [Fact]
        public void ReturnsServiceRegisteredViaBindingThatMatchesPredicate()
        {
            kernel.Bind<IWeapon>().To<Shuriken>().WithMetadata("type", "range");
            kernel.Bind<IWeapon>().To<Sword>().WithMetadata("type", "melee");

            var weapon = kernel.Get<IWeapon>(x => x.Get<string>("type") == "melee");

            weapon.ShouldNotBeNull();
            weapon.ShouldBeInstanceOf<Sword>();
        }
    }

    [TestClass]
    public class WhenUnbindIsCalled : StandardKernelContext
    {
        [Fact]
        public void RemovesAllBindingsForService()
        {
            kernel.Bind<IWeapon>().To<Shuriken>();
            kernel.Bind<IWeapon>().To<Sword>();

            var bindings = kernel.GetBindings(typeof(IWeapon)).ToArray();
            bindings.Length.ShouldBe(2);

            kernel.Unbind<IWeapon>();
            bindings = kernel.GetBindings(typeof(IWeapon)).ToArray();
            bindings.ShouldBeEmpty();
        }
    }

    [TestClass]
    public class WhenRebindIsCalled : StandardKernelContext
    {
        [Fact]
        public void RemovesAllBindingsForServiceAndReplacesWithSpecifiedBinding()
        {
            kernel.Bind<IWeapon>().To<Shuriken>();
            kernel.Bind<IWeapon>().To<Sword>();

            var bindings = kernel.GetBindings(typeof(IWeapon)).ToArray();
            bindings.Length.ShouldBe(2);

            kernel.Rebind<IWeapon>().To<Sword>();
            bindings = kernel.GetBindings(typeof(IWeapon)).ToArray();
            bindings.Length.ShouldBe(1);
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

    public interface IGeneric<T> { }
    public class GenericService<T> : IGeneric<T> { }
    public class GenericService2<T> : IGeneric<T> { }
    public interface IGenericWithConstraints<T> where T : class { }
    public class GenericServiceWithConstraints<T> : IGenericWithConstraints<T> where T : class { }
    
    public class NullProvider : Ninject.Activation.Provider<Sword>
    {
        protected override Sword CreateInstance (Activation.IContext context)
        {
            return null;
        }
    }
}