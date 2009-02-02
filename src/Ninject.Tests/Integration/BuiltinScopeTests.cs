using System;
using System.Threading;
using Ninject.Infrastructure.Disposal;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Integration.BuiltInScopeTests
{
	public class BuiltInScopeContext
	{
		protected readonly StandardKernel kernel;

		public BuiltInScopeContext()
		{
			kernel = new StandardKernel();
		}
	}

	public class WhenServiceIsBoundWithSingletonScope : BuiltInScopeContext
	{
		[Fact]
		public void FirstActivatedInstanceIsReused()
		{
			kernel.Bind<IWeapon>().To<Sword>().InSingletonScope();

			var weapon1 = kernel.Get<IWeapon>();
			var weapon2 = kernel.Get<IWeapon>();

			Assert.Same(weapon1, weapon2);
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

	public class WhenServiceIsBoundWithThreadScope : BuiltInScopeContext
	{
		[Fact]
		public void FirstActivatedInstanceIsReusedWithinThread()
		{
			kernel.Bind<IWeapon>().To<Sword>().InThreadScope();

			IWeapon weapon1 = null;
			IWeapon weapon2 = null;

			ThreadStart callback = () =>
			{
				weapon1 = kernel.Get<IWeapon>();
				weapon2 = kernel.Get<IWeapon>();
			};

			var thread = new Thread(callback);

			thread.Start();
			thread.Join();

			Assert.NotNull(weapon1);
			Assert.NotNull(weapon2);
			Assert.Same(weapon1, weapon2);
		}

		[Fact]
		public void ScopeDoesNotInterfereWithExternalRequests()
		{
			kernel.Bind<IWeapon>().To<Sword>().InThreadScope();

			IWeapon weapon1 = kernel.Get<IWeapon>();
			IWeapon weapon2 = null;

			ThreadStart callback = () => weapon2 = kernel.Get<IWeapon>();

			var thread = new Thread(callback);

			thread.Start();
			thread.Join();

			Assert.NotNull(weapon1);
			Assert.NotNull(weapon2);
			Assert.NotSame(weapon1, weapon2);
		}

		[Fact(Skip = "Need to rethink time-based tests")]
		public void InstancesActivatedWithinScopeAreDeactivatedWithinASecondOfThreadEnding()
		{
			kernel.Bind<NotifiesWhenDisposed>().ToSelf().InThreadScope();

			NotifiesWhenDisposed instance = null;

			ThreadStart callback = () => instance = kernel.Get<NotifiesWhenDisposed>();

			var thread = new Thread(callback);

			thread.Start();
			thread.Join();

			thread = null;

			GC.Collect();
			Thread.Sleep(1500);

			Assert.NotNull(instance);
			Assert.True(instance.IsDisposed);
		}
	}

	public class NotifiesWhenDisposed : DisposableObject { }
}