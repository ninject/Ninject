#if !NO_ASSEMBLY_SCANNING
namespace Ninject.Tests.Integration.ModuleLoadingTests
{
    using System;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using Xunit;


    public class WhenLoadIsCalledWithFileName : ModuleLoadingContext
    {
#if !WINRT
        protected readonly string ModuleFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Ninject.Tests.TestModule.dll");
#else
        protected readonly string ModuleFilename = @"Ninject.Tests.TestModule.dll";
#endif

        [Fact]
        public 
#if !WINRT
        void
#else
            async System.Threading.Tasks.Task
#endif
            
            ModulesContainedInAssembliesAreLoaded()
        {
#if WINRT
            await 
#endif
                this.Kernel.Load(this.ModuleFilename);

            var modules = this.Kernel.GetModules().ToArray();

            modules.Select(m => m.GetType().FullName).Should().BeEquivalentTo(new[] { "Ninject.Tests.TestModules.TestModule" });
            modules.All(m => m.Kernel == this.Kernel).Should().BeTrue();
        }
    }
}
#endif
