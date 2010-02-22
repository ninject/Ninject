using System;
using System.Linq;
using Ninject.Activation;
using Ninject.Parameters;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Integration.CircularDependenciesTests
{
	public class CircularDependenciesContext
	{
		protected readonly StandardKernel kernel;

		public CircularDependenciesContext()
		{
			kernel = new StandardKernel();
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
			Assert.DoesNotThrow(() => kernel.Resolve(request));
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
			Assert.DoesNotThrow(() => kernel.Resolve(request));
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