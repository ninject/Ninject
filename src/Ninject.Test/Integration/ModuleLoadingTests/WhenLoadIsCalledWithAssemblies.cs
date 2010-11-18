#if !NO_ASSEMBLY_SCANNING
namespace Ninject.Tests.Integration.ModuleLoadingTests
{
    using System.Linq;
    using System.Reflection;
    using Ninject.Modules;
    using Ninject.Tests.Integration.ModuleLoadingTests.Fakes;
    using Xunit;
    using Xunit.Should;

    public class WhenLoadIsCalledWithAssemblies : ModuleLoadingContext
    {
        [Fact]
        public void ModulesContainedInAssembliesAreLoaded()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            this.Kernel.Load(assembly);

            var modules = this.Kernel.GetModules().ToArray();

            INinjectModule testModule = modules.SingleOrDefault(module => module.GetType() == typeof(TestModule));
            INinjectModule testModule2 = modules.SingleOrDefault(module => module.GetType() == typeof(TestModule2));
            INinjectModule testModule3 = modules.SingleOrDefault(module => module.GetType() == typeof(OtherFakes.TestModule));

            testModule.ShouldNotBeNull();
            testModule2.ShouldNotBeNull();
            testModule3.ShouldNotBeNull();
            testModule.Kernel.ShouldBe(this.Kernel);
            testModule2.Kernel.ShouldBe(this.Kernel);
            testModule3.Kernel.ShouldBe(this.Kernel);
        }
    }
}
#endif