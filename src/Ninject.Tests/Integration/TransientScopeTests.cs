using System;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Tests.Fakes;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Integration.TransientScopeTests
{
	public class TransientScopeContext
	{
		protected readonly StandardKernel kernel;

		public TransientScopeContext()
		{
			kernel = new StandardKernel();
		}
	}

	public class WhenServiceIsBoundToInterfaceInTransientScope : TransientScopeContext
	{
		[Fact]
		public void NewInstanceIsReturnedForEachRequest()
		{
			kernel.Bind<IWeapon>().To<Sword>().InTransientScope();

			var instance1 = kernel.Get<IWeapon>();
			var instance2 = kernel.Get<IWeapon>();

			instance1.ShouldNotBeSameAs(instance2);
		}

		[Fact]
		public void InstancesAreGarbageCollectedIfAllExternalReferencesAreDropped()
		{
			kernel.Bind<IWeapon>().To<Sword>().InTransientScope();

			var instance = kernel.Get<IWeapon>();
			var reference = new WeakReference(instance);

			instance = null;

			GC.Collect();
			GC.WaitForPendingFinalizers();

			reference.IsAlive.ShouldBeFalse();
		}
	}

	public class WhenServiceIsBoundToSelfInTransientScope : TransientScopeContext
	{
		[Fact]
		public void NewInstanceIsReturnedForEachRequest()
		{
			kernel.Bind<Sword>().ToSelf().InTransientScope();

			var sword1 = kernel.Get<Sword>();
			var sword2 = kernel.Get<Sword>();

			sword1.ShouldNotBeSameAs(sword2);
		}

		[Fact]
		public void InstancesAreGarbageCollectedIfAllExternalReferencesAreDropped()
		{
			kernel.Bind<Sword>().ToSelf().InTransientScope();

			var instance = kernel.Get<Sword>();
			var reference = new WeakReference(instance);

			instance = null;

			GC.Collect();
			GC.WaitForPendingFinalizers();

			reference.IsAlive.ShouldBeFalse();

			var cache = kernel.Components.Get<ICache>();
			cache.Prune();

			cache.Count.ShouldBe(0);
		}
	}

	public class WhenServiceIsBoundToProviderInTransientScope : TransientScopeContext
	{
		[Fact]
		public void NewInstanceIsReturnedForEachRequest()
		{
			kernel.Bind<IWeapon>().ToProvider<SwordProvider>().InTransientScope();

			var instance1 = kernel.Get<IWeapon>();
			var instance2 = kernel.Get<IWeapon>();

			instance1.ShouldNotBeSameAs(instance2);
		}

		[Fact]
		public void InstancesAreGarbageCollectedIfAllExternalReferencesAreDropped()
		{
			kernel.Bind<IWeapon>().ToProvider<SwordProvider>().InTransientScope();

			var instance = kernel.Get<IWeapon>();
			var reference = new WeakReference(instance);

			instance = null;

			GC.Collect();
			GC.WaitForPendingFinalizers();

			reference.IsAlive.ShouldBeFalse();
		}
	}

	public class WhenServiceIsBoundToMethodInTransientScope : TransientScopeContext
	{
		[Fact]
		public void NewInstanceIsReturnedForEachRequest()
		{
			kernel.Bind<IWeapon>().ToMethod(x => new Sword()).InTransientScope();

			var instance1 = kernel.Get<IWeapon>();
			var instance2 = kernel.Get<IWeapon>();

			instance1.ShouldNotBeSameAs(instance2);
		}

		[Fact]
		public void InstancesAreGarbageCollectedIfAllExternalReferencesAreDropped()
		{
			kernel.Bind<IWeapon>().ToMethod(x => new Sword()).InTransientScope();

			var instance = kernel.Get<IWeapon>();
			var reference = new WeakReference(instance);

			instance = null;

			GC.Collect();
			GC.WaitForPendingFinalizers();

			reference.IsAlive.ShouldBeFalse();
		}
	}

	public class SwordProvider : Provider<Sword>
	{
		protected override Sword CreateInstance(IContext context)
		{
			return new Sword();
		}
	}
}