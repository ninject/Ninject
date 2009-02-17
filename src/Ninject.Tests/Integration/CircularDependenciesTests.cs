using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ninject.Activation;
using Ninject.Activation.Hooks;
using Ninject.Parameters;
using Xunit;

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

	public class WhenDependenciesHaveCircularReferenceBetweenConstructors : CircularDependenciesContext
	{
		public WhenDependenciesHaveCircularReferenceBetweenConstructors()
		{
			kernel.Bind<ConstructorFoo>().ToSelf().InSingletonScope();
			kernel.Bind<ConstructorBar>().ToSelf().InSingletonScope();
		}

		[Fact]
		public void DoesNotThrowExceptionIfHookIsCreated()
		{
			var request = new Request(typeof(ConstructorFoo), null, Enumerable.Empty<IParameter>(), null);
			Assert.DoesNotThrow(() => kernel.Resolve(request));
		}

		[Fact]
		public void ThrowsActivationExceptionWhenHookIsResolved()
		{
			Assert.Throws<ActivationException>(() => kernel.Get<ConstructorFoo>());
		}
	}

	public class WhenDependenciesHaveCircularReferenceBetweenProperties : CircularDependenciesContext
	{
		public WhenDependenciesHaveCircularReferenceBetweenProperties()
		{
			kernel.Bind<PropertyFoo>().ToSelf().InSingletonScope();
			kernel.Bind<PropertyBar>().ToSelf().InSingletonScope();
		}

		[Fact]
		public void DoesNotThrowException()
		{
			Assert.DoesNotThrow(() => kernel.Get<PropertyFoo>());
		}

		[Fact]
		public void ScopeIsRespected()
		{
			var foo = kernel.Get<PropertyFoo>();
			var bar = kernel.Get<PropertyBar>();

			Assert.Same(bar, foo.Bar);
			Assert.Same(foo, bar.Foo);
		}
	}

	public class ConstructorFoo
	{
		public ConstructorFoo(ConstructorBar bar) { }
	}

	public class ConstructorBar
	{
		public ConstructorBar(ConstructorFoo foo) { }
	}

	public class PropertyFoo
	{
		[Inject] public PropertyBar Bar { get; set; }
	}

	public class PropertyBar
	{
		[Inject] public PropertyFoo Foo { get; set; }
	}
}