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

			weapon1.ShouldBeSameAs(weapon2);
		}

		[Fact]
		public void ScopeDoesNotInterfereWithExternalResolution()
		{
			kernel.Bind<IWeapon>().To<Sword>();

			var weapon1 = scope.Get<IWeapon>();
			var weapon2 = kernel.Get<IWeapon>();

			weapon1.ShouldNotBeSameAs(weapon2);
		}

		[Fact]
		public void InstancesAreNotGarbageCollectedAsLongAsScopeRemainsAlive()
		{
			kernel.Bind<NotifiesWhenDisposed>().ToSelf();

			var instance = scope.Get<NotifiesWhenDisposed>();
			GC.Collect();

			instance.IsDisposed.ShouldBeFalse();
		}
	}

	public class WhenScopeIsDisposed : ActivationScopeContext
	{
		[Fact]
		public void InstancesActivatedWithinScopeAreDeactivated()
		{
			kernel.Bind<NotifiesWhenDisposed>().ToSelf();

			var instance = scope.Get<NotifiesWhenDisposed>();
			scope.Dispose();

			instance.IsDisposed.ShouldBeTrue();
		}
	}
}