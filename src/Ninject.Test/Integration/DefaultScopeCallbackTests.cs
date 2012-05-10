namespace Ninject.Tests.Integration.DefaultScopeCallbackTests
{
    using System;
    using FluentAssertions;
    using Ninject.Infrastructure;
    using Xunit;

    public class DefaultScopeContext : IDisposable
    {
        protected StandardKernel kernel;        

        public DefaultScopeContext()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }
    }

    public class WhenKernelIsCreatedWithDefaults : DefaultScopeContext
    {
        [Fact]
        public void ScopeShouldBeTransient()
        {
            kernel.Settings.DefaultScopeCallback.Should().BeSameAs(StandardScopeCallbacks.Transient);
        }
    }
}
