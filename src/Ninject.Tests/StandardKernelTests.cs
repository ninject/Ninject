using System;
using System.Linq;
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

		[Fact]
		public void DependenciesAreInjectedViaConstructor()
		{
			kernel.Bind<IWeapon>().To<Sword>();
			kernel.Bind<IWarrior>().To<Samurai>();

			var warrior = kernel.Get<IWarrior>();

			Assert.NotNull(warrior);
			Assert.IsType<Samurai>(warrior);
			Assert.NotNull(warrior.Weapon);
			Assert.IsType<Sword>(warrior.Weapon);
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

		[Fact]
		public void DependenciesAreInjectedViaConstructor()
		{
			kernel.Bind<IWeapon>().To<Sword>();
			kernel.Bind<Samurai>().ToSelf();

			var samurai = kernel.Get<Samurai>();

			Assert.NotNull(samurai);
			Assert.NotNull(samurai.Weapon);
			Assert.IsType<Sword>(samurai.Weapon);
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

	public class WhenGetAllIsCalledForInterfaceBoundService : StandardKernelContext
	{
		[Fact]
		public void ReturnsSeriesOfItemsInOrderTheyWereBound()
		{
			kernel.Bind<IWeapon>().To<Sword>();
			kernel.Bind<IWeapon>().To<Shuriken>();

			var weapons = kernel.GetAll<IWeapon>().ToArray();

			Assert.NotNull(weapons);
			Assert.Equal(2, weapons.Length);
			Assert.IsType<Sword>(weapons[0]);
			Assert.IsType<Shuriken>(weapons[1]);
		}
	}

	public class WhenGetAllIsCalledForGenericServiceRegisteredViaOpenGenericType : StandardKernelContext
	{
		[Fact]
		public void GenericParametersAreInferred()
		{
			kernel.Bind(typeof(IGeneric<>)).To(typeof(GenericService<>));
			kernel.Bind(typeof(IGeneric<>)).To(typeof(GenericService2<>));

			var services = kernel.GetAll<IGeneric<int>>().ToArray();

			Assert.NotNull(services);
			Assert.Equal(2, services.Length);
			Assert.IsType<GenericService<int>>(services[0]);
			Assert.IsType<GenericService2<int>>(services[1]);
		}
	}

	public class WhenGetIsCalledWithConstraints : StandardKernelContext
	{
		[Fact]
		public void ReturnsServiceRegisteredViaBindingWithSpecifiedName()
		{
			kernel.Bind<IWeapon>().To<Shuriken>();
			kernel.Bind<IWeapon>().To<Sword>().WithName("sword");

			var weapon = kernel.Get<IWeapon>("sword");

			Assert.NotNull(weapon);
			Assert.IsType<Sword>(weapon);
		}

		[Fact]
		public void ReturnsServiceRegisteredViaBindingThatMatchesPredicate()
		{
			kernel.Bind<IWeapon>().To<Shuriken>().WithMetadata("type", "range");
			kernel.Bind<IWeapon>().To<Sword>().WithMetadata("type", "melee");

			var weapon = kernel.Get<IWeapon>(x => x.Get<string>("type") == "melee");

			Assert.NotNull(weapon);
			Assert.IsType<Sword>(weapon);
		}
	}

	public class WhenGetAllIsCalledWithName : StandardKernelContext
	{
	}

	public interface IGeneric<T> { }
	public class GenericService<T> : IGeneric<T> { }
	public class GenericService2<T> : IGeneric<T> { }
	public interface IGenericWithConstraints<T> where T : class { }
	public class GenericServiceWithConstraints<T> : IGenericWithConstraints<T> where T : class { }
}