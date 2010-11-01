namespace Ninject.Tests.Integration.CircularDependenciesTests
{
    using System.Linq;
    using Ninject.Activation;
    using Ninject.Parameters;
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

    public class CircularDependenciesContext
    {
        protected StandardKernel kernel;

        public CircularDependenciesContext()
        {
            this.SetUp();
        }

        [TestInitialize]
        public virtual void SetUp()
        {
            this.kernel = new StandardKernel();
        }
    }

    [TestClass]
    public class WhenDependenciesHaveTwoWayCircularReferenceBetweenConstructors : CircularDependenciesContext
    {
        public override void SetUp()
        {
            base.SetUp();
            kernel.Bind<TwoWayConstructorFoo>().ToSelf().InSingletonScope();
            kernel.Bind<TwoWayConstructorBar>().ToSelf().InSingletonScope();
        }

        [Fact]
        public void DoesNotThrowExceptionIfHookIsCreated()
        {
            var request = new Request(typeof(TwoWayConstructorFoo), null, Enumerable.Empty<IParameter>(), null, false, false);
            Assert.DoesNotThrow(() => kernel.Resolve(request));
        }

        [Fact]
        public void ThrowsActivationExceptionWhenHookIsResolved()
        {
            Assert.Throws<ActivationException>(() => kernel.Get<TwoWayConstructorFoo>());
        }
    }

    [TestClass]
    public class WhenDependenciesHaveTwoWayCircularReferenceBetweenProperties : CircularDependenciesContext
    {
        public override void SetUp()
        {
            base.SetUp();
            kernel.Bind<TwoWayPropertyFoo>().ToSelf().InSingletonScope();
            kernel.Bind<TwoWayPropertyBar>().ToSelf().InSingletonScope();
        }


        [Fact]
        public void DoesNotThrowException()
        {
            Assert.DoesNotThrow(() => kernel.Get<TwoWayPropertyFoo>());
        }

        [Fact]
        public void ScopeIsRespected()
        {
            var foo = kernel.Get<TwoWayPropertyFoo>();
            var bar = kernel.Get<TwoWayPropertyBar>();

            foo.Bar.ShouldBeSameAs(bar);
            bar.Foo.ShouldBeSameAs(foo);
        }
    }

    [TestClass]
    public class WhenDependenciesHaveThreeWayCircularReferenceBetweenConstructors : CircularDependenciesContext
    {
        public override void SetUp()
        {
            base.SetUp();
            kernel.Bind<ThreeWayConstructorFoo>().ToSelf().InSingletonScope();
            kernel.Bind<ThreeWayConstructorBar>().ToSelf().InSingletonScope();
            kernel.Bind<ThreeWayConstructorBaz>().ToSelf().InSingletonScope();
        }

        [Fact]
        public void DoesNotThrowExceptionIfHookIsCreated()
        {
            var request = new Request(typeof(ThreeWayConstructorFoo), null, Enumerable.Empty<IParameter>(), null, false, false);
            Assert.DoesNotThrow(() => kernel.Resolve(request));
        }

        [Fact]
        public void ThrowsActivationExceptionWhenHookIsResolved()
        {
            Assert.Throws<ActivationException>(() => kernel.Get<ThreeWayConstructorFoo>());
        }
    }

    [TestClass]
    public class WhenDependenciesHaveThreeWayCircularReferenceBetweenProperties : CircularDependenciesContext
    {
        public override void SetUp()
        {
            base.SetUp();
            kernel.Bind<ThreeWayPropertyFoo>().ToSelf().InSingletonScope();
            kernel.Bind<ThreeWayPropertyBar>().ToSelf().InSingletonScope();
            kernel.Bind<ThreeWayPropertyBaz>().ToSelf().InSingletonScope();
        }

        [Fact]
        public void DoesNotThrowException()
        {
            Assert.DoesNotThrow(() => kernel.Get<ThreeWayPropertyFoo>());
        }

        [Fact]
        public void ScopeIsRespected()
        {
            var foo = kernel.Get<ThreeWayPropertyFoo>();
            var bar = kernel.Get<ThreeWayPropertyBar>();
            var baz = kernel.Get<ThreeWayPropertyBaz>();

            foo.Bar.ShouldBeSameAs(bar);
            bar.Baz.ShouldBeSameAs(baz);
            baz.Foo.ShouldBeSameAs(foo);
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
        [Inject] public TwoWayPropertyBar Bar { get; set; }
    }

    public class TwoWayPropertyBar
    {
        [Inject] public TwoWayPropertyFoo Foo { get; set; }
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
        [Inject] public ThreeWayPropertyBar Bar { get; set; }
    }

    public class ThreeWayPropertyBar
    {
        [Inject] public ThreeWayPropertyBaz Baz { get; set; }
    }

    public class ThreeWayPropertyBaz
    {
        [Inject] public ThreeWayPropertyFoo Foo { get; set; }
    }

}