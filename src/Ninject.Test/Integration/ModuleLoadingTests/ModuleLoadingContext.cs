#if !NO_MOQ
namespace Ninject.Tests.Integration.ModuleLoadingTests
{
    using System;

    using Moq;
    using Ninject.Modules;

    public class ModuleLoadingContext : IDisposable
    {
        public ModuleLoadingContext()
        {
            this.Kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.Kernel.Dispose();
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