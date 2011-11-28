#if !NO_MOQ
#if !NO_ASSEMBLY_SCANNING
namespace Ninject.Tests.Integration.ModuleLoadingTests
{
    using System;
    using System.Text;
    using FluentAssertions;
    using Moq;
    using Ninject.Tests.Integration.ModuleLoadingTests.Fakes;
    using Xunit;

    public class WhenLoadIsCalledWithModule : ModuleLoadingContext
    {
        [Fact]
        public void IdenticalNamedModulesFromDifferenNamespacesCanBeLoadedTogether()
        {
            this.Kernel.Load(new TestModule());
            this.Kernel.Load(new OtherFakes.TestModule());
        }

        [Fact]
        public void MockModulePassedToLoadIsLoadedAndCallsOnLoad()
        {
            var moduleMock = this.CreateModuleMock("SomeName");
            var module = moduleMock.Object;

            this.Kernel.Load(module);

            moduleMock.Verify(x => x.OnLoad(this.Kernel), Times.Once());
            this.Kernel.GetModules().Should().BeEquivalentTo(module);
        }

        [Fact]
        public void OnUnloadTheModuleIsUnloadedAndRemovedFormTheKernel()
        {
            var moduleMock = this.CreateModuleMock("SomeName");
            var module = moduleMock.Object;
            
            this.Kernel.Load(module);
            this.Kernel.Unload(module.Name);

            moduleMock.Verify(x => x.OnUnload(this.Kernel), Times.Once());
            this.Kernel.GetModules().Should().BeEmpty();
        }

        [Fact]
        public void ModuleInstanceWithNullNameIsNotSupported()
        {
            var module = this.CreateModule(null);

            Action moduleLoadingAction = () => this.Kernel.Load(module);

            moduleLoadingAction.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void ModuleInstanceWithEmptyNameIsNotSupported()
        {
            var module = this.CreateModule(string.Empty);

            Action moduleLoadingAction = () => this.Kernel.Load(module);

            moduleLoadingAction.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void TwoModulesWithSameNamesAreNotSupported()
        {
            const string ModuleName = "SomeModuleName";
            var module1 = this.CreateModule(ModuleName);
            var module2 = this.CreateModule(ModuleName);

            this.Kernel.Load(module1);
            Action moduleLoadingAction = () => this.Kernel.Load(module2);

            moduleLoadingAction.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void ModuleWithSameNameCanBeLoadedAfterTheFirstIsUnloaded()
        {
            const string ModuleName = "SomeModuleName";
            var module1 = this.CreateModule(ModuleName);
            var module2 = this.CreateModule(ModuleName);

            this.Kernel.Load(module1);
            this.Kernel.Unload(module1.Name);
            this.Kernel.Load(module2);

            this.Kernel.GetModules().Should().BeEquivalentTo(module2);
        }

        [Fact]
        public void UnloadNotLoadedModuleFails()
        {
            Action moduleUnloadingAction = () => this.Kernel.Unload("NotLoadedModule");

            moduleUnloadingAction.ShouldThrow<NotSupportedException>();
        }
    
        [Fact]
        public void ModulesAreVerifiedAfterAllModulesAreLoaded()
        {
            var moduleMock1 = this.CreateModuleMock("SomeName1");
            var moduleMock2 = this.CreateModuleMock("SomeName2");
            var orderStringBuilder = new StringBuilder();

            moduleMock1.Setup(m => m.OnLoad(this.Kernel)).Callback(() => orderStringBuilder.Append("LoadModule1 "));
            moduleMock2.Setup(m => m.OnLoad(this.Kernel)).Callback(() => orderStringBuilder.Append("LoadModule2 "));
            moduleMock1.Setup(m => m.OnVerifyRequiredModules()).Callback(() => orderStringBuilder.Append("VerifyModule "));
            moduleMock2.Setup(m => m.OnVerifyRequiredModules()).Callback(() => orderStringBuilder.Append("VerifyModule "));

            this.Kernel.Load(moduleMock1.Object, moduleMock2.Object);

            orderStringBuilder.ToString().Should().Be("LoadModule1 LoadModule2 VerifyModule VerifyModule ");
        }
    }
}
#endif
#endif