#if !NO_MOQ
namespace Ninject.Tests.Integration.ModuleLoadingTests
{
    using Moq;
    using Ninject.Modules;

    public class ModuleLoadingContext
    {
        public ModuleLoadingContext()
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