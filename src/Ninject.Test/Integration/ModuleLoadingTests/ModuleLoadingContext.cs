
namespace Ninject.Tests.Integration.ModuleLoadingTests
{
    using System;
#if !NO_MOQ
    using Moq;
#endif
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
        #if !NO_MOQ
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
#endif

        protected INinjectModule CreateModule(string name)
        {
            return new FakeModule(name);
        }

        private class FakeModule : INinjectModule
        {
            public FakeModule(string name)
            {
                Name = name;
            }

            public string Name { get; private set; }

            public void OnLoad(IKernel kernel)
            {
                throw new NotImplementedException();
            }

            public void OnUnload(IKernel kernel)
            {
                throw new NotImplementedException();
            }

            public void OnVerifyRequiredModules()
            {
                throw new NotImplementedException();
            }

            public IKernel Kernel
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}