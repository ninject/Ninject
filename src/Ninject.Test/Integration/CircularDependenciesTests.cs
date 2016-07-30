namespace Ninject.Tests.Integration.CircularDependenciesTests
{
    using System;
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
            kernel.Bind<TwoWayConstructorFoo>().ToSelf().InSingletonScope();
            kernel.Bind<TwoWayConstructorBar>().ToSelf().InSingletonScope();
        }

        [Fact]
        public void DoesNotThrowExceptionIfHookIsCreated()
        {
            var request = new Request(typeof(TwoWayConstructorFoo), null, Enumerable.Empty<IParameter>(), null, false, false);
            kernel.Resolve(request);
        }

        [Fact]
        public void ThrowsActivationExceptionWhenHookIsResolved()
        {
            Assert.Throws<ActivationException>(() => kernel.Get<TwoWayConstructorFoo>());
        }
    }

    public class WhenDependenciesHaveTwoWayCircularReferenceBetweenProperties : CircularDependenciesContext
    {
        public WhenDependenciesHaveTwoWayCircularReferenceBetweenProperties()
        {
            kernel.Bind<TwoWayPropertyFoo>().ToSelf().InSingletonScope();
            kernel.Bind<TwoWayPropertyBar>().ToSelf().InSingletonScope();
        }


        [Fact]
        public void DoesNotThrowException()
        {
            kernel.Get<TwoWayPropertyFoo>();
        }

        [Fact]
        public void ScopeIsRespected()
        {
            var foo = kernel.Get<TwoWayPropertyFoo>();
            var bar = kernel.Get<TwoWayPropertyBar>();

            foo.Bar.Should().BeSameAs(bar);
            bar.Foo.Should().BeSameAs(foo);
        }
    }

    public class WhenDependenciesHaveTwoWayCircularReferenceBetweenPropertiesBoundInTransientScope : CircularDependenciesContext
    {
        public WhenDependenciesHaveTwoWayCircularReferenceBetweenPropertiesBoundInTransientScope()
        {
            kernel.Bind<TwoWayPropertyFoo>().ToSelf();
            kernel.Bind<TwoWayPropertyBar>().ToSelf();
        }


        [Fact]
        public void ThrowsActivationException()
        {
            Assert.Throws<ActivationException>(() => kernel.Get<TwoWayPropertyFoo>());
        }
    }

    public class WhenDependenciesHaveThreeWayCircularReferenceBetweenConstructors : CircularDependenciesContext
    {
        public WhenDependenciesHaveThreeWayCircularReferenceBetweenConstructors()
        {
            kernel.Bind<ThreeWayConstructorFoo>().ToSelf().InSingletonScope();
            kernel.Bind<ThreeWayConstructorBar>().ToSelf().InSingletonScope();
            kernel.Bind<ThreeWayConstructorBaz>().ToSelf().InSingletonScope();
        }

        [Fact]
        public void DoesNotThrowExceptionIfHookIsCreated()
        {
            var request = new Request(typeof(ThreeWayConstructorFoo), null, Enumerable.Empty<IParameter>(), null, false, false);
            kernel.Resolve(request);
        }

        [Fact]
        public void ThrowsActivationExceptionWhenHookIsResolved()
        {
            Assert.Throws<ActivationException>(() => kernel.Get<ThreeWayConstructorFoo>());
        }
    }

    public class WhenDependenciesHaveThreeWayCircularReferenceBetweenProperties : CircularDependenciesContext
    {
        public WhenDependenciesHaveThreeWayCircularReferenceBetweenProperties()
        {
            kernel.Bind<ThreeWayPropertyFoo>().ToSelf().InSingletonScope();
            kernel.Bind<ThreeWayPropertyBar>().ToSelf().InSingletonScope();
            kernel.Bind<ThreeWayPropertyBaz>().ToSelf().InSingletonScope();
        }

        [Fact]
        public void DoesNotThrowException()
        {
            kernel.Get<ThreeWayPropertyFoo>();
        }

        [Fact]
        public void ScopeIsRespected()
        {
            var foo = kernel.Get<ThreeWayPropertyFoo>();
            var bar = kernel.Get<ThreeWayPropertyBar>();
            var baz = kernel.Get<ThreeWayPropertyBaz>();

            foo.Bar.Should().BeSameAs(bar);
            bar.Baz.Should().BeSameAs(baz);
            baz.Foo.Should().BeSameAs(foo);
        }
    }

    public class WhenDependenciesHaveOpenGenericCircularReferenceBetweenConstructors : CircularDependenciesContext
    {
        public WhenDependenciesHaveOpenGenericCircularReferenceBetweenConstructors()
        {
            kernel.Bind(typeof(IOptions<>)).To(typeof(OptionsManager<>));

            kernel.Bind<IConfigureOptions<ClassA>>().To<ConfigureA1>();
            kernel.Bind<IConfigureOptions<ClassB>>().To<ConfigureB1>();
            kernel.Bind<IConfigureOptions<ClassC>>().To<HasCircularDependency1>();
            kernel.Bind<IConfigureOptions<ClassD>>().To<HasCircularDependency2>();

        }

        [Fact]
        public void DoesNotThrowException()
        {
            kernel.Get<IOptions<ClassA>>();

        }

        [Fact]
        public void DoesNotThrowException2()
        {
            var o = kernel.Get<HasOptionsPropertyInjected>();

        }

        [Fact]
        public void DetectsCyclicDependenciesInPropertySetter()
        {
            Action act = () => kernel.Get<IOptions<ClassC>>();

            act.ShouldThrow<ActivationException>();
        }

        [Fact]
        public void DetectsCyclicDependenciesForGenericServiceRegisteredViaOpenGenericType2()
        {
            kernel.Bind(typeof(IGeneric<>)).To(typeof(GenericServiceWithGenericConstructor<>));

            Action act = () => kernel.Get<IGeneric<int>>();

            act.ShouldThrow<ActivationException>();
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
}