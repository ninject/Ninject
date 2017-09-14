namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
    using System;

    using Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes;

    public abstract class EnumerableDependenciesContext : IDisposable
    {
        protected EnumerableDependenciesContext()
        {
            this.Configuration = new KernelConfiguration();
        }

        public void Dispose()
        {
            this.Configuration.Dispose();
        }

        protected IKernelConfiguration Configuration { get; set; }

        protected abstract void VerifyInjection(IParent parent);
    }
}