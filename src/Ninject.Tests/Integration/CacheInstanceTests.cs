namespace Ninject.Tests.Integration.ExternalInjectionTests
{
    using System;

    using Ninject.Tests.Fakes;
    using Xunit;

    public class CacheInstanceContext : IDisposable
    {
        protected StandardKernel kernel;

        public CacheInstanceContext()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }
    }

    public class WhenResolveWithPropertyInjection : CacheInstanceContext
    {
        [Fact]
        public void ShouldThrowActivationExceptionTheSecondTime()
        {
            this.kernel.Bind<SomeWarrior>().ToSelf().InSingletonScope();
            try
            {
                this.kernel.Get<SomeWarrior>();
            }
            catch (ActivationException)
            {
            }

            Assert.Throws<ActivationException>(() => this.kernel.Get<SomeWarrior>());
        }
    }

    public class SomeWarrior
    {
        [Inject] public IWeapon Weapon { get; set; }
    }
}