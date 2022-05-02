namespace Ninject.Tests.Integration.SingletonScopeTests
{
    using System;
    using FluentAssertions;
    using Ninject.Activation;
    using Ninject.Infrastructure.Disposal;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class SingletonScopeContext : IDisposable
    {
        protected StandardKernel kernel;

        public SingletonScopeContext()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }
    }

    public class WhenServiceIsBoundToInterfaceInSingletonScope : SingletonScopeContext
    {
        [Fact]
        public void FirstActivatedInstanceIsReused()
        {
            this.kernel.Bind<IWeapon>().To<Sword>().InSingletonScope();

            var instance1 = this.kernel.Get<IWeapon>();
            var instance2 = this.kernel.Get<IWeapon>();

            instance1.Should().BeSameAs(instance2);
        }

        [Fact]
        public void InstancesAreNotGarbageCollectedAsLongAsKernelRemainsAlive()
        {
            this.kernel.Bind<IWeapon>().To<Sword>().InSingletonScope();

            var instance = this.kernel.Get<IWeapon>();
            var reference = new WeakReference(instance);

            instance = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            reference.IsAlive.Should().BeTrue();
        }

        [Fact]
        public void InstancesAreDeactivatedWhenKernelIsDisposed()
        {
            this.kernel.Bind<INotifyWhenDisposed>().To<NotifiesWhenDisposed>().InSingletonScope();

            var instance = this.kernel.Get<INotifyWhenDisposed>();
            this.kernel.Dispose();

            instance.IsDisposed.Should().BeTrue();
        }
    }

    public class WhenServiceIsBoundToSelfInSingletonScope : SingletonScopeContext
    {
        [Fact]
        public void FirstActivatedInstanceIsReused()
        {
            this.kernel.Bind<Sword>().ToSelf().InSingletonScope();

            var sword1 = this.kernel.Get<Sword>();
            var sword2 = this.kernel.Get<Sword>();

            sword1.Should().BeSameAs(sword2);
        }

        [Fact]
        public void InstancesAreNotGarbageCollectedAsLongAsKernelRemainsAlive()
        {
            this.kernel.Bind<NotifiesWhenDisposed>().ToSelf().InSingletonScope();

            var instance = this.kernel.Get<NotifiesWhenDisposed>();
            var reference = new WeakReference(instance);

            instance = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            reference.IsAlive.Should().BeTrue();
        }

        [Fact]
        public void InstancesAreDeactivatedWhenKernelIsDisposed()
        {
            this.kernel.Bind<NotifiesWhenDisposed>().ToSelf().InSingletonScope();

            var instance = this.kernel.Get<NotifiesWhenDisposed>();
            this.kernel.Dispose();

            instance.IsDisposed.Should().BeTrue();
        }
    }

    public class WhenServiceIsBoundToProviderInSingletonScope : SingletonScopeContext
    {
        [Fact]
        public void FirstActivatedInstanceIsReused()
        {
            this.kernel.Bind<INotifyWhenDisposed>().ToProvider<NotifiesWhenDisposedProvider>().InSingletonScope();

            var instance1 = this.kernel.Get<INotifyWhenDisposed>();
            var instance2 = this.kernel.Get<INotifyWhenDisposed>();

            instance1.Should().BeSameAs(instance2);
        }

        [Fact]
        public void InstancesAreNotGarbageCollectedAsLongAsKernelRemainsAlive()
        {
            this.kernel.Bind<INotifyWhenDisposed>().ToProvider<NotifiesWhenDisposedProvider>().InSingletonScope();

            var instance = this.kernel.Get<INotifyWhenDisposed>();
            var reference = new WeakReference(instance);

            instance = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            reference.IsAlive.Should().BeTrue();
        }

        [Fact]
        public void InstancesAreDeactivatedWhenKernelIsDisposed()
        {
            this.kernel.Bind<INotifyWhenDisposed>().ToProvider<NotifiesWhenDisposedProvider>().InSingletonScope();

            var instance = this.kernel.Get<INotifyWhenDisposed>();
            this.kernel.Dispose();

            instance.IsDisposed.Should().BeTrue();
        }
    }

    public class WhenServiceIsBoundToMethodInSingletonScope : SingletonScopeContext
    {
        [Fact]
        public void FirstActivatedInstanceIsReused()
        {
            this.kernel.Bind<INotifyWhenDisposed>().ToMethod(x => new NotifiesWhenDisposed()).InSingletonScope();

            var instance1 = this.kernel.Get<INotifyWhenDisposed>();
            var instance2 = this.kernel.Get<INotifyWhenDisposed>();

            instance1.Should().BeSameAs(instance2);
        }

        [Fact]
        public void InstancesAreNotGarbageCollectedAsLongAsKernelRemainsAlive()
        {
            this.kernel.Bind<INotifyWhenDisposed>().ToMethod(x => new NotifiesWhenDisposed()).InSingletonScope();

            var instance = this.kernel.Get<INotifyWhenDisposed>();
            var reference = new WeakReference(instance);

            instance = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            reference.IsAlive.Should().BeTrue();
        }

        [Fact]
        public void InstancesAreDeactivatedWhenKernelIsDisposed()
        {
            this.kernel.Bind<INotifyWhenDisposed>().ToMethod(x => new NotifiesWhenDisposed()).InSingletonScope();

            var instance = this.kernel.Get<INotifyWhenDisposed>();
            this.kernel.Dispose();

            instance.IsDisposed.Should().BeTrue();
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