using System.Reflection;
using FluentAssertions;
using Ninject.Injection;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Unit.ExpressionInjectorFactoryTests
{
    public class ExpressionInjectorFactoryContext
    {
        protected ExpressionInjectorFactory injectorFactory;

        public ExpressionInjectorFactoryContext()
        {
            this.injectorFactory = new ExpressionInjectorFactory();
        }
    }

    public class WhenConstructorInjectorIsInvoked : ExpressionInjectorFactoryContext
    {
        protected ConstructorInfo constructor;
        protected ConstructorInjector injector;

        public WhenConstructorInjectorIsInvoked()
        {
            this.constructor = typeof(Samurai).GetConstructor(new[] { typeof(IWeapon) });
            this.injector = this.injectorFactory.Create(this.constructor);
        }

        [Fact]
        public void CallsConstructor()
        {
            var sword = new Sword();

            var samurai = this.injector.Invoke(new[] { sword }) as Samurai;

            samurai.Should().NotBeNull();
            samurai.Weapon.Should().BeSameAs(sword);
        }

        [Fact]
        public void CallsConstructorWithNullArgumentIfOneIsSpecified()
        {
            var samurai = this.injector.Invoke(new[] { (IWeapon)null }) as Samurai;

            samurai.Should().NotBeNull();
            samurai.Weapon.Should().BeNull();
        }
    }

    public class WhenPropertyInjectorIsInvoked : ExpressionInjectorFactoryContext
    {
        protected PropertyInfo property;
        protected PropertyInjector injector;

        public WhenPropertyInjectorIsInvoked()
        {
            this.property = typeof(Samurai).GetProperty("Weapon");
            this.injector = this.injectorFactory.Create(this.property);
        }

        [Fact]
        public void SetsPropertyValue()
        {
            var samurai = new Samurai(null);
            var sword = new Sword();

            this.injector.Invoke(samurai, sword);

            samurai.Weapon.Should().BeSameAs(sword);
        }

        [Fact]
        public void SetsPropertyValueToNullIfInvokedWithNullArgument()
        {
            var samurai = new Samurai(new Sword());
            this.injector.Invoke(samurai, null);
            samurai.Weapon.Should().BeNull();
        }
    }

    public class WhenMethodInjectorIsInvokedOnVoidMethod : ExpressionInjectorFactoryContext
    {
        protected MethodInfo method;
        protected MethodInjector injector;

        public WhenMethodInjectorIsInvokedOnVoidMethod()
        {
            this.method = typeof(Samurai).GetMethod("SetName");
            this.injector = this.injectorFactory.Create(this.method);
        }

        [Fact]
        public void CallsMethod()
        {
            var samurai = new Samurai(new Sword());
            this.injector.Invoke(samurai, new[] { "Bob" });
            samurai.Name.Should().Be("Bob");
        }
    }

    public class WhenMethodInjectorIsInvokedOnNonVoidMethod : ExpressionInjectorFactoryContext
    {
        protected MethodInfo method;
        protected MethodInjector injector;

        public WhenMethodInjectorIsInvokedOnNonVoidMethod()
        {
            this.method = typeof(Samurai).GetMethod("Attack");
            this.injector = this.injectorFactory.Create(this.method);
        }

        [Fact]
        public void CallsMethod()
        {
            var samurai = new Samurai(new Sword());
            this.injector.Invoke(samurai, new[] { "evildoer" });
            samurai.IsBattleHardened.Should().BeTrue();
        }
    }
}