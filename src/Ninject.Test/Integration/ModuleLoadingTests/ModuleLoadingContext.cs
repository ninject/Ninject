#if !WINDOWS_PHONE
namespace Ninject.Tests.Integration.ModuleLoadingTests
{
    using Moq;
    using Ninject.Modules;
#if SILVERLIGHT
#if SILVERLIGHT_MSTEST
        using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
        using UnitDriven;
#endif
#else
    using Ninject.Tests.MSTestAttributes;
#endif

    public class ModuleLoadingContext
    {
        public ModuleLoadingContext()
        {
            this.SetUp();
        }

        [TestInitialize]
        private void SetUp()
        {
            this.Kernel = new StandardKernel();
        }

        protected StandardKernel Kernel { get; private set; }

        protected string GetRegularMockModuleName()
        {
            return "TestModuleName";
        }

        protected Mock<INinjectModule> CreateModuleMock(string name)
        {
            var moduleMock = new Mock<INinjectModule>();
            moduleMock.SetupGet(x => x.Name).Returns(name);

            return moduleMock;
        }

        protected INinjectModule CreateModule(string name)
        {
            return this.CreateModuleMock(name).Object;
        }
    }
}
#endif