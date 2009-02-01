using System;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.StandardKernelTests
{
	public class StandardKernelContext
	{
		protected readonly StandardKernel kernel;

		public StandardKernelContext()
		{
			kernel = new StandardKernel();
		}
	}

	public class WhenGetIsCalledForInterfaceBoundService : StandardKernelContext
	{
		[Fact]
		public void SingleInstanceIsReturnedWhenOneBindingIsRegistered()
		{
			kernel.Bind<IWeapon>().To<Sword>();
			var weapon = kernel.Get<IWeapon>();
			Assert.NotNull(weapon);
			Assert.IsType<Sword>(weapon);
		}

		[Fact]
		public void FirstInstanceIsReturnedWhenMultipleBindingsAreRegistered()
		{
			kernel.Bind<IWeapon>().To<Sword>();
			kernel.Bind<IWeapon>().To<Shuriken>();
			var weapon = kernel.Get<IWeapon>();
			Assert.NotNull(weapon);
			Assert.IsType<Sword>(weapon);
		}
	}

	public class WhenGetIsCalledForSelfBoundService : StandardKernelContext
	{
		[Fact]
		public void SingleInstanceIsReturnedWhenOneBindingIsRegistered()
		{
			kernel.Bind<Sword>().ToSelf();
			var weapon = kernel.Get<Sword>();
			Assert.NotNull(weapon);
			Assert.IsType<Sword>(weapon);
		}
	}

	public class WhenGetIsCalledForUnboundService : StandardKernelContext
	{
		[Fact]
		public void ImplicitSelfBindingIsRegisteredAndActivated()
		{
			var weapon = kernel.Get<Sword>();
			Assert.NotNull(weapon);
			Assert.IsType<Sword>(weapon);
		}

		[Fact]
		public void ImplicitSelfBindingForGenericTypeIsRegisteredAndActivated()
		{
			var service = kernel.Get<GenericService<int>>();
			Assert.NotNull(service);
			Assert.IsType<GenericService<int>>(service);
		}

		[Fact]
		public void ThrowsExceptionIfAnUnboundInterfaceIsRequested()
		{
			Assert.Throws<NotSupportedException>(() => kernel.Get<IWeapon>());
		}

		[Fact]
		public void ThrowsExceptionIfAnUnboundAbstractClassIsRequested()
		{
			Assert.Throws<NotSupportedException>(() => kernel.Get<AbstractWeapon>());
		}

		[Fact]
		public void ThrowsExceptionIfAnOpenGenericTypeIsRequested()
		{
			Assert.Throws<NotSupportedException>(() => kernel.Get(typeof(IGeneric<>)));
		}
	}

	public class WhenGetIsCalledForGenericServiceRegisteredViaOpenGenericType : StandardKernelContext
	{
		[Fact]
		public void GenericParametersAreInferred()
		{
			kernel.Bind(typeof(IGeneric<>)).To(typeof(GenericService<>));
			var service = kernel.Get<IGeneric<int>>();
			Assert.NotNull(service);
			Assert.IsType<GenericService<int>>(service);
		}
	}

	public interface IGeneric<T> { }
	public class GenericService<T> : IGeneric<T> { }
	public interface IGenericWithConstraints<T> where T : class { }
	public class GenericServiceWithConstraints<T> : IGenericWithConstraints<T> where T : class { }
}