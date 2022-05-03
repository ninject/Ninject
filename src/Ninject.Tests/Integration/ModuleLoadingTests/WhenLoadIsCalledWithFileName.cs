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
            this.Kernel.Load(this.ModuleFilename);

            var modules = this.Kernel.GetModules().ToArray();

            modules.Select(m => m.GetType().FullName).Should().BeEquivalentTo(new[] { "Ninject.Tests.TestModule.TestModule" });
            modules.All(m => m.Kernel == this.Kernel).Should().BeTrue();
        }
    }
}