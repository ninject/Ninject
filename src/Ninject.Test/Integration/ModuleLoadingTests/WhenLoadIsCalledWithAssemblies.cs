#if !NO_ASSEMBLY_SCANNING
namespace Ninject.Tests.Integration.ModuleLoadingTests
{
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using Ninject.Tests.Integration.ModuleLoadingTests.Fakes;
    using Xunit;

#if MSTEST
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
#endif
    public class WhenLoadIsCalledWithAssemblies : ModuleLoadingContext
    {
#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void ModulesContainedInAssembliesAreLoaded()
        {
            var expectedModules = new[] { typeof(TestModule), typeof(TestModule2), typeof(OtherFakes.TestModule) };
#if !WINRT
            var assembly = Assembly.GetExecutingAssembly();
#else
            var assembly = typeof (WhenLoadIsCalledWithAssemblies).GetTypeInfo().Assembly;
#endif
            this.Kernel.Load(assembly);
            var modules = this.Kernel.GetModules().ToArray();

            modules.Select(m => m.GetType()).Should().BeEquivalentTo(expectedModules);
            modules.All(m => m.Kernel == this.Kernel).Should().BeTrue();
        }
    }
}
#endif