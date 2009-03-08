using System;
using Ninject.Activation;
using Ninject.Infrastructure.Disposal;
using Ninject.Tests.Fakes;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Integration.SingletonScopeTests
{
	public class SingletonScopeContext
	{
		protected readonly StandardKernel kernel;

		public SingletonScopeContext()
		{
			kernel = new StandardKernel();
		}
	}

	public class WhenServiceIsBoundToInterfaceInSingletonScope : SingletonScopeContext
	{
		[Fact]
		public void FirstActivatedInstanceIsReused()
		{
			kernel.Bind<IWeapon>().To<Sword>().InSingletonScope();

			var instance1 = kernel.Get<IWeapon>();
			var instance2 = kernel.Get<IWeapon>();

			instance1.ShouldBeSameAs(instance2);
		}

		[Fact]
		public void InstancesAreNotGarbageCollectedAsLongAsKernelRemainsAlive()
		{
			kernel.Bind<IWeapon>().To<Sword>().InSingletonScope();

			var instance = kernel.Get<IWeapon>();
			var reference = new WeakReference(instance);

			instance = null;

			GC.Collect();
			GC.WaitForPendingFinalizers();

			reference.IsAlive.ShouldBeTrue();
		}

		[Fact]
		public void InstancesAreDeactivatedWhenKernelIsDisposed()
		{
			kernel.Bind<INotifyWhenDisposed>().To<NotifiesWhenDisposed>().InSingletonScope();

			var instance = kernel.Get<INotifyWhenDisposed>();
			kernel.Dispose();

			instance.IsDisposed.ShouldBeTrue();
		}
	}

	public class WhenServiceIsBoundToSelfInSingletonScope : SingletonScopeContext
	{
		[Fact]
		public void FirstActivatedInstanceIsReused()
		{
			kernel.Bind<Sword>().ToSelf().InSingletonScope();

			var sword1 = kernel.Get<Sword>();
			var sword2 = kernel.Get<Sword>();

			sword1.ShouldBeSameAs(sword2);
		}

		[Fact]
		public void InstancesAreNotGarbageCollectedAsLongAsKernelRemainsAlive()
		{
			kernel.Bind<NotifiesWhenDisposed>().ToSelf().InSingletonScope();

			var instance = kernel.Get<NotifiesWhenDisposed>();
			var reference = new WeakReference(instance);

			instance = null;

			GC.Collect();
			GC.WaitForPendingFinalizers();

			reference.IsAlive.ShouldBeTrue();
		}

		[Fact]
		public void InstancesAreDeactivatedWhenKernelIsDisposed()
		{
			kernel.Bind<NotifiesWhenDisposed>().ToSelf().InSingletonScope();

			var instance = kernel.Get<NotifiesWhenDisposed>();
			kernel.Dispose();

			instance.IsDisposed.ShouldBeTrue();
		}
	}

	public class WhenServiceIsBoundToProviderInSingletonScope : SingletonScopeContext
	{
		[Fact]
		public void FirstActivatedInstanceIsReused()
		{
			kernel.Bind<INotifyWhenDisposed>().ToProvider<NotifiesWhenDisposedProvider>().InSingletonScope();

			var instance1 = kernel.Get<INotifyWhenDisposed>();
			var instance2 = kernel.Get<INotifyWhenDisposed>();

			instance1.ShouldBeSameAs(instance2);
		}

		[Fact]
		public void InstancesAreNotGarbageCollectedAsLongAsKernelRemainsAlive()
		{
			kernel.Bind<INotifyWhenDisposed>().ToProvider<NotifiesWhenDisposedProvider>().InSingletonScope();

			var instance = kernel.Get<INotifyWhenDisposed>();
			var reference = new WeakReference(instance);

			instance = null;

			GC.Collect();
			GC.WaitForPendingFinalizers();

			reference.IsAlive.ShouldBeTrue();
		}

		[Fact]
		public void InstancesAreDeactivatedWhenKernelIsDisposed()
		{
			kernel.Bind<INotifyWhenDisposed>().ToProvider<NotifiesWhenDisposedProvider>().InSingletonScope();

			var instance = kernel.Get<INotifyWhenDisposed>();
			kernel.Dispose();

			instance.IsDisposed.ShouldBeTrue();
		}
	}

	public class WhenServiceIsBoundToMethodInSingletonScope : SingletonScopeContext
	{
		[Fact]
		public void FirstActivatedInstanceIsReused()
		{
			kernel.Bind<INotifyWhenDisposed>().ToMethod(x => new NotifiesWhenDisposed()).InSingletonScope();

			var instance1 = kernel.Get<INotifyWhenDisposed>();
			var instance2 = kernel.Get<INotifyWhenDisposed>();

			instance1.ShouldBeSameAs(instance2);
		}

		[Fact]
		public void InstancesAreNotGarbageCollectedAsLongAsKernelRemainsAlive()
		{
			kernel.Bind<INotifyWhenDisposed>().ToMethod(x => new NotifiesWhenDisposed()).InSingletonScope();

			var instance = kernel.Get<INotifyWhenDisposed>();
			var reference = new WeakReference(instance);

			instance = null;

			GC.Collect();
			GC.WaitForPendingFinalizers();

			reference.IsAlive.ShouldBeTrue();
		}

		[Fact]
		public void InstancesAreDeactivatedWhenKernelIsDisposed()
		{
			kernel.Bind<INotifyWhenDisposed>().ToMethod(x => new NotifiesWhenDisposed()).InSingletonScope();

			var instance = kernel.Get<INotifyWhenDisposed>();
			kernel.Dispose();

			instance.IsDisposed.ShouldBeTrue();
		}
	}

	public class NotifiesWhenDisposedProvider : Provider<NotifiesWhenDisposed>
	{
		protected override NotifiesWhenDisposed CreateInstance(IContext context)
		{
			return new NotifiesWhenDisposed();
		}
	}
}