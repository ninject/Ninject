namespace Ninject.Tests.Integration.ModuleLoadingTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using Xunit;

    public class WhenLoadIsCalledWithFileName : ModuleLoadingContext
    {
        protected readonly string ModuleFilename = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), @"Ninject.Tests.TestModule.dll");

        [Fact]
        public void ModulesContainedInAssembliesAreLoaded()
        {
            this.KernelConfiguration.Load(this.ModuleFilename);

            var modules = this.KernelConfiguration.GetModules().ToArray();

            modules.Select(m => m.GetType().FullName).Should().BeEquivalentTo(new[] { "Ninject.Tests.TestModules.TestModule" });
            modules.All(m => m.KernelConfiguration == this.KernelConfiguration).Should().BeTrue();
        }
    }
}