using System;
using Ninject.Activation;
using Ninject.Infrastructure.Disposal;
using Ninject.Tests.Fakes;
using Xunit;

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
			kernel.Bind<INotifyWhenDisposed>().To<NotifiesWhenDisposed>().InSingletonScope();

			var instance1 = kernel.Get<INotifyWhenDisposed>();
			var instance2 = kernel.Get<INotifyWhenDisposed>();

			Assert.Same(instance1, instance2);
		}

		[Fact]
		public void InstancesAreNotGarbageCollectedAsLongAsKernelRemainsAlive()
		{
			kernel.Bind<INotifyWhenDisposed>().To<NotifiesWhenDisposed>().InSingletonScope();

			bool instanceWasDisposed = false;

			var instance = kernel.Get<INotifyWhenDisposed>();
			instance.Disposed += (o, e) => instanceWasDisposed = true;

			instance = null;

			GC.Collect();

			Assert.False(instanceWasDisposed);
		}

		[Fact]
		public void InstancesAreDeactivatedWhenKernelIsDisposed()
		{
			kernel.Bind<INotifyWhenDisposed>().To<NotifiesWhenDisposed>().InSingletonScope();

			bool instanceWasDisposed = false;

			var instance = kernel.Get<INotifyWhenDisposed>();
			instance.Disposed += (o, e) => instanceWasDisposed = true;

			kernel.Dispose();

			Assert.True(instanceWasDisposed);
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

			Assert.Same(sword1, sword2);
		}

		[Fact]
		public void InstancesAreNotGarbageCollectedAsLongAsKernelRemainsAlive()
		{
			kernel.Bind<NotifiesWhenDisposed>().ToSelf().InSingletonScope();

			bool instanceWasDisposed = false;

			var instance = kernel.Get<NotifiesWhenDisposed>();
			instance.Disposed += (o, e) => instanceWasDisposed = true;

			instance = null;

			GC.Collect();

			Assert.False(instanceWasDisposed);
		}

		[Fact]
		public void InstancesAreDeactivatedWhenKernelIsDisposed()
		{
			kernel.Bind<NotifiesWhenDisposed>().ToSelf().InSingletonScope();

			bool instanceWasDisposed = false;

			var instance = kernel.Get<NotifiesWhenDisposed>();
			instance.Disposed += (o, e) => instanceWasDisposed = true;

			kernel.Dispose();

			Assert.True(instanceWasDisposed);
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

			Assert.Same(instance1, instance2);
		}

		[Fact]
		public void InstancesAreNotGarbageCollectedAsLongAsKernelRemainsAlive()
		{
			kernel.Bind<INotifyWhenDisposed>().ToProvider<NotifiesWhenDisposedProvider>().InSingletonScope();

			bool instanceWasDisposed = false;

			var instance = kernel.Get<INotifyWhenDisposed>();
			instance.Disposed += (o, e) => instanceWasDisposed = true;

			instance = null;

			GC.Collect();

			Assert.False(instanceWasDisposed);
		}

		[Fact]
		public void InstancesAreDeactivatedWhenKernelIsDisposed()
		{
			kernel.Bind<INotifyWhenDisposed>().ToProvider<NotifiesWhenDisposedProvider>().InSingletonScope();

			bool instanceWasDisposed = false;

			var instance = kernel.Get<INotifyWhenDisposed>();
			instance.Disposed += (o, e) => instanceWasDisposed = true;

			kernel.Dispose();

			Assert.True(instanceWasDisposed);
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

			Assert.Same(instance1, instance2);
		}

		[Fact]
		public void InstancesAreNotGarbageCollectedAsLongAsKernelRemainsAlive()
		{
			kernel.Bind<INotifyWhenDisposed>().ToMethod(x => new NotifiesWhenDisposed()).InSingletonScope();

			bool instanceWasDisposed = false;

			var instance = kernel.Get<INotifyWhenDisposed>();
			instance.Disposed += (o, e) => instanceWasDisposed = true;

			instance = null;

			GC.Collect();

			Assert.False(instanceWasDisposed);
		}

		[Fact]
		public void InstancesAreDeactivatedWhenKernelIsDisposed()
		{
			kernel.Bind<INotifyWhenDisposed>().ToMethod(x => new NotifiesWhenDisposed()).InSingletonScope();

			bool instanceWasDisposed = false;

			var instance = kernel.Get<INotifyWhenDisposed>();
			instance.Disposed += (o, e) => instanceWasDisposed = true;

			kernel.Dispose();

			Assert.True(instanceWasDisposed);
		}
	}

	public class NotifiesWhenDisposed : DisposableObject { }

	public class NotifiesWhenDisposedProvider : Provider<INotifyWhenDisposed>
	{
		protected override INotifyWhenDisposed CreateInstance(IContext context)
		{
			return new NotifiesWhenDisposed();
		}
	}
}