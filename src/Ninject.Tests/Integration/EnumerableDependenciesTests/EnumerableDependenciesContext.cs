namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
    using System;

    using Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes;

    public abstract class EnumerableDependenciesContext : IDisposable
    {
        protected EnumerableDependenciesContext()
        {
            this.Kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.Kernel.Dispose();
        }

        protected StandardKernel Kernel { get; private set; }

        protected abstract void VerifyInjection(IParent parent);
    }
}