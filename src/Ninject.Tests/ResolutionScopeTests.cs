using System;
using Ninject.Infrastructure;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.ResolutionScopeTests
{
	public class ResolutionScopeContext
	{
		protected readonly StandardKernel kernel;
		protected readonly ResolutionScope scope;

		public ResolutionScopeContext()
		{
			kernel = new StandardKernel();
			scope = new ResolutionScope(kernel);
		}
	}

	public class WhenScopeIsCreated : ResolutionScopeContext
	{
		[Fact]
		public void FirstActivatedInstanceIsReusedWithinScope()
		{
			kernel.Bind<IWeapon>().To<Sword>();

			var weapon1 = scope.Get<IWeapon>();
			var weapon2 = scope.Get<IWeapon>();

			Assert.Same(weapon1, weapon2);
		}

		[Fact]
		public void ScopeDoesNotInterfereWithExternalResolution()
		{
			kernel.Bind<IWeapon>().To<Sword>();

			var weapon1 = scope.Get<IWeapon>();
			var weapon2 = kernel.Get<IWeapon>();

			Assert.NotSame(weapon1, weapon2);
		}

		[Fact]
		public void InstancesAreNotGarbageCollectedAsLongAsScopeRemainsAlive()
		{
			kernel.Bind<NotifiesWhenDisposed>().ToSelf();

			bool instanceWasDisposed = false;

			var instance = scope.Get<NotifiesWhenDisposed>();
			instance.Disposed += (o, e) => instanceWasDisposed = true;

			GC.Collect();

			Assert.False(instanceWasDisposed);
		}
	}

	public class WhenScopeIsDisposed : ResolutionScopeContext
	{
		[Fact]
		public void InstancesActivatedWithinScopeAreDeactivated()
		{
			kernel.Bind<NotifiesWhenDisposed>().ToSelf();

			bool instanceWasDisposed = false;

			var instance = scope.Get<NotifiesWhenDisposed>();
			instance.Disposed += (o, e) => instanceWasDisposed = true;

			scope.Dispose();

			Assert.True(instanceWasDisposed);
		}
	}

	public class NotifiesWhenDisposed : DisposableObject { }
}