#if !NO_LCG
using System.Reflection;
using Ninject.Injection;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Unit.DynamicMethodInjectorFactoryTests
{
    using FluentAssertions;

    public class DynamicMethodInjectorFactoryContext
    {
        protected DynamicMethodInjectorFactory injectorFactory;

        public DynamicMethodInjectorFactoryContext()
        {
            injectorFactory = new DynamicMethodInjectorFactory();
            injectorFactory.Settings = new NinjectSettings();
        }
    }

    public class WhenConstructorInjectorIsInvoked : DynamicMethodInjectorFactoryContext
    {
        protected ConstructorInfo constructor;
        protected ConstructorInjector injector;

        public WhenConstructorInjectorIsInvoked()
        {
            constructor = typeof(Samurai).GetConstructor(new[] { typeof(IWeapon) });
            injector = injectorFactory.Create(constructor);
        }

        [Fact]
        public void CallsConstructor()
        {
            var sword = new Sword();

            var samurai = injector.Invoke(new[] { sword }) as Samurai;

            samurai.Should().NotBeNull();
            samurai.Weapon.Should().BeSameAs(sword);
        }

        [Fact]
        public void CallsConstructorWithNullArgumentIfOneIsSpecified()
        {
            var samurai = injector.Invoke(new[] { (IWeapon)null }) as Samurai;

            samurai.Should().NotBeNull();
            samurai.Weapon.Should().BeNull();
        }
    }

    public class WhenPropertyInjectorIsInvoked : DynamicMethodInjectorFactoryContext
    {
        protected PropertyInfo property;
        protected PropertyInjector injector;

        public WhenPropertyInjectorIsInvoked()
        {
            property = typeof(Samurai).GetProperty("Weapon");
            injector = injectorFactory.Create(property);
        }

        [Fact]
        public void SetsPropertyValue()
        {
            var samurai = new Samurai(null);
            var sword = new Sword();

            injector.Invoke(samurai, sword);

            samurai.Weapon.Should().BeSameAs(sword);
        }

        [Fact]
        public void SetsPropertyValueToNullIfInvokedWithNullArgument()
        {
            var samurai = new Samurai(new Sword());
            injector.Invoke(samurai, null);
            samurai.Weapon.Should().BeNull();
        }
    }

    public class WhenMethodInjectorIsInvokedOnVoidMethod : DynamicMethodInjectorFactoryContext
    {
        protected MethodInfo method;
        protected MethodInjector injector;

        public WhenMethodInjectorIsInvokedOnVoidMethod()
        {
            method = typeof(Samurai).GetMethod("SetName");
            injector = injectorFactory.Create(method);
        }

        [Fact]
        public void CallsMethod()
        {
            var samurai = new Samurai(new Sword());
            injector.Invoke(samurai, new[] { "Bob" });
            samurai.Name.Should().Be("Bob");
        }
    }

    public class WhenMethodInjectorIsInvokedOnNonVoidMethod : DynamicMethodInjectorFactoryContext
    {
        protected MethodInfo method;
        protected MethodInjector injector;

        public WhenMethodInjectorIsInvokedOnNonVoidMethod()
        {
            method = typeof(Samurai).GetMethod("Attack");
            injector = injectorFactory.Create(method);
        }

        [Fact]
        public void CallsMethod()
        {
            var samurai = new Samurai(new Sword());
            injector.Invoke(samurai, new[] { "evildoer" });
            samurai.IsBattleHardened.Should().BeTrue();
        }
    }
}
#endif //!NO_LCG