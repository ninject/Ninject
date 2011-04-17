namespace Ninject.Tests.Integration.ActivationBlockTests
{
    using System;

    using Ninject.Activation.Blocks;
    using Ninject.Tests.Fakes;
    using Xunit;
    using Xunit.Should;

    public class ActivationBlockContext
    {
        protected StandardKernel kernel;
        protected ActivationBlock block;

        public ActivationBlockContext()
        {
            this.kernel = new StandardKernel();
            this.block = new ActivationBlock(kernel);
        }
    }

    public class WhenBlockIsCreated : ActivationBlockContext
    {
        [Fact]
        public void FirstActivatedInstanceIsReusedWithinBlock()
        {
            kernel.Bind<IWeapon>().To<Sword>();

            var weapon1 = block.Get<IWeapon>();
            var weapon2 = block.Get<IWeapon>();

            weapon1.ShouldBeSameAs(weapon2);
        }

        [Fact]
        public void BlockDoesNotInterfereWithExternalResolution()
        {
            kernel.Bind<IWeapon>().To<Sword>();

            var weapon1 = block.Get<IWeapon>();
            var weapon2 = kernel.Get<IWeapon>();

            weapon1.ShouldNotBeSameAs(weapon2);
        }

        [Fact]
        public void InstancesAreNotGarbageCollectedAsLongAsBlockRemainsAlive()
        {
            kernel.Bind<NotifiesWhenDisposed>().ToSelf();

            var instance = block.Get<NotifiesWhenDisposed>();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            instance.IsDisposed.ShouldBeFalse();
        }
    }

    public class WhenBlockIsDisposed : ActivationBlockContext
    {
        [Fact]
        public void InstancesActivatedWithinBlockAreDeactivated()
        {
            kernel.Bind<NotifiesWhenDisposed>().ToSelf();

            var instance = block.Get<NotifiesWhenDisposed>();
            block.Dispose();

            instance.IsDisposed.ShouldBeTrue();
        }
    }
}