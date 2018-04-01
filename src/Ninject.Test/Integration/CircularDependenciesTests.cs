namespace Ninject.Tests.Integration.CircularDependenciesTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Ninject.Activation;
    using Ninject.Parameters;
    using Ninject.Tests.Integration.StandardKernelTests;
    using Xunit;

    public class CircularDependenciesContext : IDisposable
    {
        protected StandardKernel kernel;

        public CircularDependenciesContext()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }
    }

    public class WhenDependenciesHaveTwoWayCircularReferenceBetweenConstructors : CircularDependenciesContext
    {
        public WhenDependenciesHaveTwoWayCircularReferenceBetweenConstructors()
        {
            this.kernel.Bind<TwoWayConstructorFoo>().ToSelf().InSingletonScope();
            this.kernel.Bind<TwoWayConstructorBar>().ToSelf().InSingletonScope();

            this.kernel.Bind<IDecoratorPattern>().To<Decorator1>().WhenInjectedInto<Decorator2>();
            this.kernel.Bind<IDecoratorPattern>().To<Decorator2>();

            this.kernel.Bind<IDecoratorPattern>().To<Decorator3>().Named("Decorator3");
            this.kernel.Bind<IDecoratorPattern>().To<Decorator4>().Named("Decorator4");
        }

        [Fact]
        public void DoesNotThrowExceptionIfHookIsCreated()
        {
            var request = new Request(typeof(TwoWayConstructorFoo), null, Enumerable.Empty<IParameter>(), null, false, false);
            this.kernel.Resolve(request);
        }

        [Fact]
        public void DoesNotThrowExceptionIfConditionDoesNotMatch()
        {
            this.kernel.Get<IDecoratorPattern>((string)null);
        }

        [Fact]
        public void DoesNotThrowExceptionWithNamedBinding()
        {
            this.kernel.Get<IDecoratorPattern>("Decorator4");
        }

        [Fact]
        public void ThrowsActivationExceptionWhenHookIsResolved()
        {
            Assert.Throws<ActivationException>(() => this.kernel.Get<TwoWayConstructorFoo>());
        }
    }

    public class WhenDependenciesHaveTwoWayCircularReferenceBetweenProperties : CircularDependenciesContext
    {
        public WhenDependenciesHaveTwoWayCircularReferenceBetweenProperties()
        {
            this.kernel.Bind<TwoWayPropertyFoo>().ToSelf().InSingletonScope();
            this.kernel.Bind<TwoWayPropertyBar>().ToSelf().InSingletonScope();
        }


        [Fact]
        public void DoesNotThrowException()
        {
            this.kernel.Get<TwoWayPropertyFoo>();
        }

        [Fact]
        public void ScopeIsRespected()
        {
            var foo = this.kernel.Get<TwoWayPropertyFoo>();
            var bar = this.kernel.Get<TwoWayPropertyBar>();

            foo.Bar.Should().BeSameAs(bar);
            bar.Foo.Should().BeSameAs(foo);
        }
    }

    public class WhenDependenciesHaveThreeWayCircularReferenceBetweenConstructors : CircularDependenciesContext
    {
        public WhenDependenciesHaveThreeWayCircularReferenceBetweenConstructors()
        {
            this.kernel.Bind<ThreeWayConstructorFoo>().ToSelf().InSingletonScope();
            this.kernel.Bind<ThreeWayConstructorBar>().ToSelf().InSingletonScope();
            this.kernel.Bind<ThreeWayConstructorBaz>().ToSelf().InSingletonScope();
        }

        [Fact]
        public void DoesNotThrowExceptionIfHookIsCreated()
        {
            var request = new Request(typeof(ThreeWayConstructorFoo), null, Enumerable.Empty<IParameter>(), null, false, false);

            this.kernel.Resolve(request);
        }

        [Fact]
        public void ThrowsActivationExceptionWhenHookIsResolved()
        {
            Assert.Throws<ActivationException>(() => this.kernel.Get<ThreeWayConstructorFoo>());
        }
    }

    public class WhenDependenciesHaveThreeWayCircularReferenceBetweenProperties : CircularDependenciesContext
    {
        public WhenDependenciesHaveThreeWayCircularReferenceBetweenProperties()
        {
            this.kernel.Bind<ThreeWayPropertyFoo>().ToSelf().InSingletonScope();
            this.kernel.Bind<ThreeWayPropertyBar>().ToSelf().InSingletonScope();
            this.kernel.Bind<ThreeWayPropertyBaz>().ToSelf().InSingletonScope();
        }

        [Fact]
        public void DoesNotThrowException()
        {
            this.kernel.Get<ThreeWayPropertyFoo>();
        }

        [Fact]
        public void ScopeIsRespected()
        {
            var foo = this.kernel.Get<ThreeWayPropertyFoo>();
            var bar = this.kernel.Get<ThreeWayPropertyBar>();
            var baz = this.kernel.Get<ThreeWayPropertyBaz>();

            foo.Bar.Should().BeSameAs(bar);
            bar.Baz.Should().BeSameAs(baz);
            baz.Foo.Should().BeSameAs(foo);
        }
    }

    public class WhenDependenciesHaveOpenGenericCircularReferenceBetweenConstructors : CircularDependenciesContext
    {
        public WhenDependenciesHaveOpenGenericCircularReferenceBetweenConstructors()
        {
            this.kernel.Bind(typeof(IOptions<>)).To(typeof(OptionsManager<>));

            this.kernel.Bind<IConfigureOptions<ClassA>>().To<ConfigureA1>();
            this.kernel.Bind<IConfigureOptions<ClassB>>().To<ConfigureB1>();
            this.kernel.Bind<IConfigureOptions<ClassC>>().To<HasCircularDependency1>();
            this.kernel.Bind<IConfigureOptions<ClassD>>().To<HasCircularDependency2>();

        }

        [Fact]
        public void DoesNotThrowException()
        {
            this.kernel.Get<IOptions<ClassA>>();

        }

        [Fact]
        public void DoesNotThrowException2()
        {
            var o = this.kernel.Get<HasOptionsPropertyInjected>();

        }

        [Fact]
        public void DetectsCyclicDependenciesInPropertySetter()
        {
            Action act = () => this.kernel.Get<IOptions<ClassC>>();

            act.Should().Throw<ActivationException>();
        }

        [Fact]
        public void DetectsCyclicDependenciesForGenericServiceRegisteredViaOpenGenericType2()
        {
            this.kernel.Bind(typeof(IGeneric<>)).To(typeof(GenericServiceWithGenericConstructor<>));

            Action act = () => this.kernel.Get<IGeneric<int>>();

            act.Should().Throw<ActivationException>();
        }

    }

    public class WhenDependenciesHaveTwoWayCircularReferenceBetweenConstructorAndProperty : CircularDependenciesContext
    {
        public WhenDependenciesHaveTwoWayCircularReferenceBetweenConstructorAndProperty()
        {
            this.kernel.Bind<TwoWayConstructorPropertyFoo>().ToSelf().InSingletonScope();
            this.kernel.Bind<TwoWayConstructorPropertyBar>().ToSelf().InSingletonScope();
        }

        [Fact]
        public void DoesNotThrowExceptionWhenGetFoo()
        {
            this.kernel.Get<TwoWayConstructorPropertyFoo>();
        }

        [Fact]
        public void DoesNotThrowExceptionWhenGetBar()
        {
            this.kernel.Get<TwoWayConstructorPropertyBar>();
        }

        [Fact]
        public void ScopeIsRespectedWhenGetFooFirstly()
        {
            var foo = this.kernel.Get<TwoWayConstructorPropertyFoo>();
            var bar = this.kernel.Get<TwoWayConstructorPropertyBar>();
            foo.Bar.Should().BeSameAs(bar);
        }

        [Fact]
        public void ScopeIsRespectedWhenGetBarFirstly()
        {
            var bar = this.kernel.Get<TwoWayConstructorPropertyBar>();
            var foo = this.kernel.Get<TwoWayConstructorPropertyFoo>();
            bar.Foo.Should().BeSameAs(foo);
        }
    }

    public class TwoWayConstructorFoo
    {
        public TwoWayConstructorFoo(TwoWayConstructorBar bar) { }
    }

    public class TwoWayConstructorBar
    {
        public TwoWayConstructorBar(TwoWayConstructorFoo foo) { }
    }

    public class TwoWayPropertyFoo
    {
        [Inject]
        public TwoWayPropertyBar Bar { get; set; }
    }

    public class TwoWayPropertyBar
    {
        [Inject]
        public TwoWayPropertyFoo Foo { get; set; }
    }

    public class TwoWayConstructorPropertyFoo
    {
        public TwoWayConstructorPropertyFoo(TwoWayConstructorPropertyBar bar)
        {
            this.Bar = bar;
        }

        public TwoWayConstructorPropertyBar Bar { get; private set; }
    }

    public class TwoWayConstructorPropertyBar
    {
        [Inject]
        public TwoWayConstructorPropertyFoo Foo { get; set; }
    }

    public class ThreeWayConstructorFoo
    {
        public ThreeWayConstructorFoo(ThreeWayConstructorBar bar) { }
    }

    public class ThreeWayConstructorBar
    {
        public ThreeWayConstructorBar(ThreeWayConstructorBaz baz) { }
    }

    public class ThreeWayConstructorBaz
    {
        public ThreeWayConstructorBaz(TwoWayConstructorFoo foo) { }
    }

    public class ThreeWayPropertyFoo
    {
        [Inject]
        public ThreeWayPropertyBar Bar { get; set; }
    }

    public class ThreeWayPropertyBar
    {
        [Inject]
        public ThreeWayPropertyBaz Baz { get; set; }
    }

    public class ThreeWayPropertyBaz
    {
        [Inject]
        public ThreeWayPropertyFoo Foo { get; set; }
    }

    public class GenericServiceWithGenericConstructor<T> : IGeneric<T>
    {
        public GenericServiceWithGenericConstructor(IGeneric<T> arg)
        {
        }
    }

    public interface IOptions<T>
    {
    }

    public class OptionsManager<T> : IOptions<T>
    {
        public OptionsManager(IConfigureOptions<T> items)
        {
        }
    }

    public interface IConfigureOptions<T>
    {
    }

    public class ConfigureA1 : IConfigureOptions<ClassA>
    {
        public ConfigureA1(IOptions<ClassB> bOptions)
        {
        }
    }

    public class ConfigureB1 : IConfigureOptions<ClassB>
    {
    }

    public class HasOptionsPropertyInjected
    {
        [Inject]
        public IOptions<ClassA> ClassAOptions { get; set; }
    }

    public class HasCircularDependency1 : IConfigureOptions<ClassC>
    {
        [Inject]
        public IOptions<ClassD> ClassDOptions { get; set; }
    }

    public class HasCircularDependency2 : IConfigureOptions<ClassD>
    {
        public HasCircularDependency2(IOptions<ClassC> classCOptions) { }
    }


    public class ClassA { }
    public class ClassB { }
    public class ClassC { }
    public class ClassD { }

    public interface IDecoratorPattern { }

    public class Decorator1 : IDecoratorPattern { }

    public class Decorator2 : IDecoratorPattern
    {
        public Decorator2(IDecoratorPattern decorator) { }
    }

    public class Decorator3 : IDecoratorPattern { }

    public class Decorator4 : IDecoratorPattern
    {
        public Decorator4([Named("Decorator3")]IDecoratorPattern decorator) { }
    }
}