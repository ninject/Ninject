namespace Ninject.Tests.Integration.ModuleLoadingTests
{
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using Ninject.Tests.Integration.ModuleLoadingTests.Fakes;
    using Ninject.Tests.Unit.Modules;
    using Xunit;

    public class WhenLoadIsCalledWithAssemblies : ModuleLoadingContext
    {
        [Fact]
        public void ModulesContainedInAssembliesAreLoaded()
        {
            var expectedModules = new[] { typeof(TestModule), typeof(TestModule2), typeof(OtherFakes.TestModule), typeof(NinjectModuleTests.MyNinjectModule) };
            var assembly = Assembly.GetExecutingAssembly();
            
            this.KernelConfiguration.Load(assembly);
            var modules = this.KernelConfiguration.GetModules().ToArray();

            modules.Select(m => m.GetType()).Should().BeEquivalentTo(expectedModules);
        }
    }
}