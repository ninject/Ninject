#if !NO_ASSEMBLY_SCANNING
namespace Ninject.Tests.Integration.ModuleLoadingTests
{
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using Ninject.Modules;
    using Ninject.Tests.Integration.ModuleLoadingTests.Fakes;
    using Xunit;

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

            testModule.Should().NotBeNull();
            testModule2.Should().NotBeNull();
            testModule3.Should().NotBeNull();
            testModule.Kernel.Should().Be(this.Kernel);
            testModule2.Kernel.Should().Be(this.Kernel);
            testModule3.Kernel.Should().Be(this.Kernel);
        }
    }
}
#endif