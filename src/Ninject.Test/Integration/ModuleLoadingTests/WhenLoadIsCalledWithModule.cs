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
            this.KernelConfiguration.Load(new TestModule());
            this.KernelConfiguration.Load(new OtherFakes.TestModule());
        }

        [Fact]
        public void MockModulePassedToLoadIsLoadedAndCallsOnLoad()
        {
            var moduleMock = this.CreateModuleMock("SomeName");
            var module = moduleMock.Object;

            this.KernelConfiguration.Load(module);

            moduleMock.Verify(x => x.OnLoad(this.KernelConfiguration), Times.Once());
            this.KernelConfiguration.GetModules().Should().BeEquivalentTo(module);
        }

        [Fact]
        public void OnUnloadTheModuleIsUnloadedAndRemovedFormTheKernel()
        {
            var moduleMock = this.CreateModuleMock("SomeName");
            var module = moduleMock.Object;
            
            this.KernelConfiguration.Load(module);
            this.KernelConfiguration.Unload(module.Name);

            moduleMock.Verify(x => x.OnUnload(), Times.Once());
            this.KernelConfiguration.GetModules().Should().BeEmpty();
        }

        [Fact]
        public void ModuleInstanceWithNullNameIsNotSupported()
        {
            var module = this.CreateModule(null);

            Action moduleLoadingAction = () => this.KernelConfiguration.Load(module);

            moduleLoadingAction.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void ModuleInstanceWithEmptyNameIsNotSupported()
        {
            var module = this.CreateModule(string.Empty);

            Action moduleLoadingAction = () => this.KernelConfiguration.Load(module);

            moduleLoadingAction.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void TwoModulesWithSameNamesAreNotSupported()
        {
            const string ModuleName = "SomeModuleName";
            var module1 = this.CreateModule(ModuleName);
            var module2 = this.CreateModule(ModuleName);

            this.KernelConfiguration.Load(module1);
            Action moduleLoadingAction = () => this.KernelConfiguration.Load(module2);

            moduleLoadingAction.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void ModuleWithSameNameCanBeLoadedAfterTheFirstIsUnloaded()
        {
            const string ModuleName = "SomeModuleName";
            var module1 = this.CreateModule(ModuleName);
            var module2 = this.CreateModule(ModuleName);

            this.KernelConfiguration.Load(module1);
            this.KernelConfiguration.Unload(module1.Name);
            this.KernelConfiguration.Load(module2);

            this.KernelConfiguration.GetModules().Should().BeEquivalentTo(module2);
        }

        [Fact]
        public void UnloadNotLoadedModuleFails()
        {
            Action moduleUnloadingAction = () => this.KernelConfiguration.Unload("NotLoadedModule");

            moduleUnloadingAction.Should().Throw<NotSupportedException>();
        }
    
        [Fact]
        public void ModulesAreVerifiedAfterAllModulesAreLoaded()
        {
            var moduleMock1 = this.CreateModuleMock("SomeName1");
            var moduleMock2 = this.CreateModuleMock("SomeName2");
            var orderStringBuilder = new StringBuilder();

            moduleMock1.Setup(m => m.OnLoad(this.KernelConfiguration)).Callback(() => orderStringBuilder.Append("LoadModule1 "));
            moduleMock2.Setup(m => m.OnLoad(this.KernelConfiguration)).Callback(() => orderStringBuilder.Append("LoadModule2 "));
            moduleMock1.Setup(m => m.OnVerifyRequiredModules()).Callback(() => orderStringBuilder.Append("VerifyModule "));
            moduleMock2.Setup(m => m.OnVerifyRequiredModules()).Callback(() => orderStringBuilder.Append("VerifyModule "));

            this.KernelConfiguration.Load(moduleMock1.Object, moduleMock2.Object);

            orderStringBuilder.ToString().Should().Be("LoadModule1 LoadModule2 VerifyModule VerifyModule ");
        }
    }
}