#if !NO_MOQ
namespace Ninject.Tests.Integration.ModuleLoadingTests
{
    using System;
    using System.Linq;
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
            this.Kernel.GetModules().ShouldContainSingle().Should().Be(module);
            this.Kernel.HasModule(module.Name).Should().BeTrue();
        }

        [Fact]
        public void OnUnloadTheModuleIsUnloadedAndRemovedFormTheKernel()
        {
            var moduleMock = this.CreateModuleMock("SomeName");
            var module = moduleMock.Object;
            
            this.Kernel.Load(module);
            this.Kernel.Unload(module.Name);

            moduleMock.Verify(x => x.OnUnload(this.Kernel), Times.Once());
            this.Kernel.HasModule(module.Name).Should().BeFalse();
            this.Kernel.GetModules().Count().Should().Be(0);
        }

        [Fact]
        public void ModuleInstanceWithNullNameIsNotSupported()
        {
            var module = this.CreateModule(null);

            Assert.Throws<NotSupportedException>(() => this.Kernel.Load(module));
        }

        [Fact]
        public void ModuleInstanceWithEmptyNameIsNotSupported()
        {
            var module = this.CreateModule(string.Empty);

            Assert.Throws<NotSupportedException>(() => this.Kernel.Load(module));
        }

        [Fact]
        public void TwoModulesWithSameNamesAreNotSupported()
        {
            const string ModuleName = "SomeModuleName";
            var module1 = this.CreateModule(ModuleName);
            var module2 = this.CreateModule(ModuleName);

            this.Kernel.Load(module1);
            Assert.Throws<NotSupportedException>(() => this.Kernel.Load(module2));
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

            this.Kernel.GetModules().ShouldContainSingle().Should().Be(module2);
        }

        [Fact]
        public void UnloadNotLoadedModuleFails()
        {
            Assert.Throws<NotSupportedException>(() => this.Kernel.Unload("NotLoadedModule"));
        }
    }
}
#endif