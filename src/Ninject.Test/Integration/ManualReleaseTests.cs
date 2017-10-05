namespace Ninject.Tests.Integration.ManualReleaseTests
{
    using System;

    using FluentAssertions;
    using Ninject.Tests.Fakes;
    using Xunit;
    
    public class ManualReleaseContext : IDisposable
    {
        protected StandardKernel kernel;

        public ManualReleaseContext()
        {
            this.kernel = new StandardKernel();            
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }
    }

    public class WhenReleaseIsCalled : ManualReleaseContext
    {
        [Fact]
        public void InstanceIsDeactivated()
        {
            this.kernel.Bind<NotifiesWhenDisposed>().ToSelf().InSingletonScope();

            var instance = this.kernel.Get<NotifiesWhenDisposed>();
            this.kernel.Release(instance);

            instance.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void InstanceIsRemovedFromCache()
        {
            this.kernel.Bind<NotifiesWhenDisposed>().ToSelf().InSingletonScope();

            var instance1 = this.kernel.Get<NotifiesWhenDisposed>();
            var instance2 = this.kernel.Get<NotifiesWhenDisposed>();
            instance1.Should().BeSameAs(instance2);

            this.kernel.Release(instance1);

            var instance3 = this.kernel.Get<NotifiesWhenDisposed>();
            instance3.Should().NotBeSameAs(instance1);
            instance3.Should().NotBeSameAs(instance2);
        }
    }
}