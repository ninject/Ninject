#if !NO_ASSEMBLY_SCANNING
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Ninject.Activation.Blocks;
using Ninject.Tests.Fakes;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Integration.ModuleLoadingTests
{
    using Ninject.Modules;

    public class ModuleLoadingContext
    {
        protected readonly StandardKernel kernel;

        public ModuleLoadingContext()
        {
            kernel = new StandardKernel();
        }
    }

    public class WhenLoadIsCalledWithAssemblies : ModuleLoadingContext
    {
        [Fact]
        public void ModulesContainedInAssembliesAreLoaded()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            kernel.Load(assembly);

            var modules = kernel.GetModules().ToArray();

            INinjectModule testModule = modules.SingleOrDefault(module => module.GetType() == typeof(TestModule));
            INinjectModule testModule2 = modules.SingleOrDefault(module => module.GetType() == typeof(TestModule2));
            INinjectModule testModule3 = modules.SingleOrDefault(module => module.GetType() == typeof(TestNamespace.TestModule));

            testModule.ShouldNotBeNull();
            testModule2.ShouldNotBeNull();
            testModule3.ShouldNotBeNull();
            testModule.Kernel.ShouldBe(kernel);
            testModule2.Kernel.ShouldBe(kernel);
            testModule3.Kernel.ShouldBe(kernel);
        }
    }

    public class WhenLoadIsCalledWithModule : ModuleLoadingContext
    {
        [Fact]
        public void ModulesPassedToLoadAreLoaded()
        {
            kernel.Load(new TestModule());
            kernel.Load(new TestModule2());

            var modules = kernel.GetModules().ToArray();

            modules.ShouldNotBeEmpty();
            modules[0].ShouldBeInstanceOf(typeof(TestModule));
            modules[0].Kernel.ShouldBe(kernel);
            modules[1].ShouldBeInstanceOf(typeof(TestModule2));
            modules[1].Kernel.ShouldBe(kernel);
        }

        [Fact]
        public void SameModuleCannotBeLoadedTwice()
        {
            kernel.Load(new TestModule());

            Assert.Throws<NotSupportedException>(() => kernel.Load(new TestModule()));
        }

        [Fact]
        public void IdenticalNamedModulesFromDifferenNamespacesCanBeLoadedTogether()
        {
            kernel.Load(new TestModule());
            kernel.Load(new TestNamespace.TestModule());
        }
    }

    public class TestModule : NinjectModule
    {
        public override void Load()
        {
        }
    }

    public class TestModule2 : NinjectModule
    {
        public override void Load()
        {
        }
    }
}

namespace TestNamespace
{
    using Ninject.Modules;

    public class TestModule : NinjectModule
    {
        public override void Load()
        {
        }
    }
}

#endif //!NO_ASSEMBLY_SCANNING