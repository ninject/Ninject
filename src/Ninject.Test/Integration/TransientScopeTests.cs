namespace Ninject.Tests.Integration.TransientScopeTests
{
    using System;
    using FluentAssertions;
    using Ninject.Activation;
    using Ninject.Activation.Caching;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class TransientScopeContext : IDisposable
    {
        protected StandardKernel kernel;

        public TransientScopeContext()
        {
            this.kernel = new StandardKernel();            
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }
    }

    public class WhenServiceIsBoundToInterfaceInTransientScope : TransientScopeContext
    {
        [Fact]
        public void NewInstanceIsReturnedForEachRequest()
        {
            this.kernel.Bind<IWeapon>().To<Sword>().InTransientScope();

            var instance1 = this.kernel.Get<IWeapon>();
            var instance2 = this.kernel.Get<IWeapon>();

            instance1.Should().NotBeSameAs(instance2);
        }

        [Fact]
        public void InstancesAreGarbageCollectedIfAllExternalReferencesAreDropped()
        {
            this.kernel.Bind<IWeapon>().To<Sword>().InTransientScope();

            var instance = this.kernel.Get<IWeapon>();
            var reference = new WeakReference(instance);

            instance = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            reference.IsAlive.Should().BeFalse();
        }
    }

    public class WhenServiceIsBoundToSelfInTransientScope : TransientScopeContext
    {
        [Fact]
        public void NewInstanceIsReturnedForEachRequest()
        {
            this.kernel.Bind<Sword>().ToSelf().InTransientScope();

            var sword1 = this.kernel.Get<Sword>();
            var sword2 = this.kernel.Get<Sword>();

            sword1.Should().NotBeSameAs(sword2);
        }

        [Fact]
        public void InstancesAreGarbageCollectedIfAllExternalReferencesAreDropped()
        {
            this.kernel.Bind<Sword>().ToSelf().InTransientScope();

            var instance = this.kernel.Get<Sword>();
            var reference = new WeakReference(instance);

            instance = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            reference.IsAlive.Should().BeFalse();

            var cache = this.kernel.Components.Get<ICache>();
            cache.Prune();

            cache.Count.Should().Be(0);
        }
    }

    public class WhenServiceIsBoundToProviderInTransientScope : TransientScopeContext
    {
        [Fact]
        public void NewInstanceIsReturnedForEachRequest()
        {
            this.kernel.Bind<IWeapon>().ToProvider<SwordProvider>().InTransientScope();

            var instance1 = this.kernel.Get<IWeapon>();
            var instance2 = this.kernel.Get<IWeapon>();

            instance1.Should().NotBeSameAs(instance2);
        }

        [Fact]
        public void InstancesAreGarbageCollectedIfAllExternalReferencesAreDropped()
        {
            this.kernel.Bind<IWeapon>().ToProvider<SwordProvider>().InTransientScope();

            var instance = this.kernel.Get<IWeapon>();
            var reference = new WeakReference(instance);

            instance = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            reference.IsAlive.Should().BeFalse();
        }
    }

    public class WhenServiceIsBoundToMethodInTransientScope : TransientScopeContext
    {
        [Fact]
        public void NewInstanceIsReturnedForEachRequest()
        {
            this.kernel.Bind<IWeapon>().ToMethod(x => new Sword()).InTransientScope();

            var instance1 = this.kernel.Get<IWeapon>();
            var instance2 = this.kernel.Get<IWeapon>();

            instance1.Should().NotBeSameAs(instance2);
        }

        [Fact]
        public void InstancesAreGarbageCollectedIfAllExternalReferencesAreDropped()
        {
            this.kernel.Bind<IWeapon>().ToMethod(x => new Sword()).InTransientScope();

            var instance = this.kernel.Get<IWeapon>();
            var reference = new WeakReference(instance);

            instance = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            reference.IsAlive.Should().BeFalse();
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