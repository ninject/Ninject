namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
    using Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes;

    public abstract class EnumerableDependenciesContext
    {
        protected EnumerableDependenciesContext()
        {
            this.SetUp();
        }

        protected StandardKernel Kernel { get; private set; }

        public void SetUp()
        {
            this.Kernel = new StandardKernel();
        }

        protected abstract void VerifyInjection(IParent parent);
    }
}