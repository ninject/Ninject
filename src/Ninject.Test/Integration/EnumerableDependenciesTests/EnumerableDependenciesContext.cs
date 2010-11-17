namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
    using Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes;
#if SILVERLIGHT
#if SILVERLIGHT_MSTEST
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
    using UnitDriven;
#endif
#else
    using Ninject.Tests.MSTestAttributes;
#endif

    public abstract class EnumerableDependenciesContext
    {
        protected EnumerableDependenciesContext()
        {
            this.SetUp();
        }

        protected StandardKernel Kernel { get; private set; }

        [TestInitialize]
        public void SetUp()
        {
            this.Kernel = new StandardKernel();
        }

        protected abstract void VerifyInjection(IParent parent);
    }
}