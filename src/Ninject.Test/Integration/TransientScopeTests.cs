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
            kernel.Bind<IWeapon>().To<Sword>().InTransientScope();

            var instance1 = kernel.Get<IWeapon>();
            var instance2 = kernel.Get<IWeapon>();

            instance1.Should().NotBeSameAs(instance2);
        }

#if !MONO
        [Fact]
        public void InstancesAreGarbageCollectedIfAllExternalReferencesAreDropped()
        {
            kernel.Bind<IWeapon>().To<Sword>().InTransientScope();

            var instance = kernel.Get<IWeapon>();
            var reference = new WeakReference(instance);

            instance = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            reference.IsAlive.Should().BeFalse();
        }
#endif
    }

    public class WhenServiceIsBoundToSelfInTransientScope : TransientScopeContext
    {
        [Fact]
        public void NewInstanceIsReturnedForEachRequest()
        {
            kernel.Bind<Sword>().ToSelf().InTransientScope();

            var sword1 = kernel.Get<Sword>();
            var sword2 = kernel.Get<Sword>();

            sword1.Should().NotBeSameAs(sword2);
        }

#if !MONO
        [Fact]
        public void InstancesAreGarbageCollectedIfAllExternalReferencesAreDropped()
        {
            kernel.Bind<Sword>().ToSelf().InTransientScope();

            var instance = kernel.Get<Sword>();
            var reference = new WeakReference(instance);

            instance = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            reference.IsAlive.Should().BeFalse();

            var cache = kernel.Components.Get<ICache>();
            cache.Prune();

            cache.Count.Should().Be(0);
        }
#endif
    }

    public class WhenServiceIsBoundToProviderInTransientScope : TransientScopeContext
    {
        [Fact]
        public void NewInstanceIsReturnedForEachRequest()
        {
            kernel.Bind<IWeapon>().ToProvider<SwordProvider>().InTransientScope();

            var instance1 = kernel.Get<IWeapon>();
            var instance2 = kernel.Get<IWeapon>();

            instance1.Should().NotBeSameAs(instance2);
        }

#if !MONO
        [Fact]
        public void InstancesAreGarbageCollectedIfAllExternalReferencesAreDropped()
        {
            kernel.Bind<IWeapon>().ToProvider<SwordProvider>().InTransientScope();

            var instance = kernel.Get<IWeapon>();
            var reference = new WeakReference(instance);

            instance = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            reference.IsAlive.Should().BeFalse();
        }
#endif
    }

    public class WhenServiceIsBoundToMethodInTransientScope : TransientScopeContext
    {
        [Fact]
        public void NewInstanceIsReturnedForEachRequest()
        {
            kernel.Bind<IWeapon>().ToMethod(x => new Sword()).InTransientScope();

            var instance1 = kernel.Get<IWeapon>();
            var instance2 = kernel.Get<IWeapon>();

            instance1.Should().NotBeSameAs(instance2);
        }

#if !MONO
        [Fact]
        public void InstancesAreGarbageCollectedIfAllExternalReferencesAreDropped()
        {
            kernel.Bind<IWeapon>().ToMethod(x => new Sword()).InTransientScope();

            var instance = kernel.Get<IWeapon>();
            var reference = new WeakReference(instance);

            instance = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            reference.IsAlive.Should().BeFalse();
        }
#endif
    }

    public class SwordProvider : Provider<Sword>
    {
        protected override Sword CreateInstance(IContext context)
        {
            return new Sword();
        }
    }
}