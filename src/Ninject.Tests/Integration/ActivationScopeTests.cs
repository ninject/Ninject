using System;
using Ninject.Activation.Scope;
using Ninject.Infrastructure.Disposal;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Integration.ActivationScopeTests
{
	public class ActivationScopeContext
	{
		protected readonly StandardKernel kernel;
		protected readonly ActivationScope scope;

		public ActivationScopeContext()
		{
			kernel = new StandardKernel();
			scope = new ActivationScope(kernel);
		}
	}

	public class WhenScopeIsCreated : ActivationScopeContext
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

	public class WhenScopeIsDisposed : ActivationScopeContext
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