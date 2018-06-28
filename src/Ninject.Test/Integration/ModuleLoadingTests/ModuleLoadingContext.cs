namespace Ninject.Tests.Integration.ModuleLoadingTests
{
    using System;

    using Moq;
    using Ninject.Modules;

    public class ModuleLoadingContext : IDisposable
    {
        public ModuleLoadingContext()
        {
            this.KernelConfiguration = new KernelConfiguration();
        }

        public void Dispose()
        {
            this.KernelConfiguration.Dispose();
        }

        protected IKernelConfiguration KernelConfiguration { get; private set; }

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