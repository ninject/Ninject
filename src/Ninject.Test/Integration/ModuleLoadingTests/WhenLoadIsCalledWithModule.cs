#if !WINDOWS_PHONE
namespace Ninject.Tests.Integration.ModuleLoadingTests
{
    using System;
    using System.Linq;
    using Moq;
    using Ninject.Tests.Integration.ModuleLoadingTests.Fakes;
#if SILVERLIGHT
#if SILVERLIGHT_MSTEST
        using MsTest.Should;
        using Microsoft.VisualStudio.TestTools.UnitTesting;
        using Assert = AssertWithThrows;
        using Fact = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
#else
        using UnitDriven;
        using UnitDriven.Should;
        using Assert = AssertWithThrows;
        using Fact = UnitDriven.TestMethodAttribute;
#endif
#else
    using Ninject.Tests.MSTestAttributes;
    using Xunit;
    using Xunit.Should;
#endif

    [TestClass]
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
            this.Kernel.GetModules().ShouldContainSingle().ShouldBe(module);
            this.Kernel.HasModule(module.Name).ShouldBeTrue();
        }

        [Fact]
        public void OnUnloadTheModuleIsUnloadedAndRemovedFormTheKernel()
        {
            var moduleMock = this.CreateModuleMock("SomeName");
            var module = moduleMock.Object;
            
            this.Kernel.Load(module);
            this.Kernel.Unload(module.Name);

            moduleMock.Verify(x => x.OnUnload(this.Kernel), Times.Once());
            this.Kernel.HasModule(module.Name).ShouldBeFalse();
            this.Kernel.GetModules().Count().ShouldBe(0);
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

            this.Kernel.GetModules().ShouldContainSingle().ShouldBe(module2);
        }

        [Fact]
        public void UnloadNotLoadedModuleFails()
        {
            Assert.Throws<NotSupportedException>(() => this.Kernel.Unload("NotLoadedModule"));
        }
    }
}
#endif