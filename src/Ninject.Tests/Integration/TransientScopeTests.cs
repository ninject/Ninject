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

            // Use separate method to allow instance to be finalized
            var reference = GetWeakReference<IWeapon>();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            reference.IsAlive.Should().BeFalse();
        }

        private WeakReference GetWeakReference<T>()
        {
            var instance = this.kernel.Get<T>();
            return new WeakReference(instance);
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

            // Use separate method to allow instance to be finalized
            var weakReference = GetWeakReference<Sword>();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            weakReference.IsAlive.Should().BeFalse();

            var cache = this.kernel.Components.Get<ICache>();
            cache.Prune();

            cache.Count.Should().Be(0);
        }

        private WeakReference GetWeakReference<T>()
        {
            var instance = this.kernel.Get<T>();
            return new WeakReference(instance);
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

            // Use separate method to allow instance to be finalized
            var weakReference = GetWeakReference<IWeapon>();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            weakReference.IsAlive.Should().BeFalse();
        }

        private WeakReference GetWeakReference<T>()
        {
            var instance = this.kernel.Get<IWeapon>();
            return new WeakReference(instance);
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

            // Use separate method to allow instance to be finalized
            var weakReference = GetWeakReference<IWeapon>();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            weakReference.IsAlive.Should().BeFalse();
        }

        private WeakReference GetWeakReference<T>()
        {
            var instance = this.kernel.Get<IWeapon>();
            return new WeakReference(instance);
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