﻿#if !NO_ASSEMBLY_SCANNING && !NO_MOQ
namespace Ninject.Tests.Integration.ModuleLoadingTests
{
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using Ninject.Tests.Integration.ModuleLoadingTests.Fakes;
    using Xunit;

    public class WhenLoadIsCalledWithAssemblies : ModuleLoadingContext
    {
        [Fact]
        public void ModulesContainedInAssembliesAreLoaded()
        {
            var expectedModules = new[] { typeof(TestModule), typeof(TestModule2), typeof(OtherFakes.TestModule) };
            var assembly = Assembly.GetExecutingAssembly();
            
            this.Kernel.Load(assembly);
            var modules = this.Kernel.GetModules().ToArray();

            modules.Select(m => m.GetType()).Should().BeEquivalentTo((object[])expectedModules);
            modules.All(m => m.KernelConfiguration == this.Kernel).Should().BeTrue();
        }
    }
}
#endif