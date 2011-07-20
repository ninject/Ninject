#if !NO_MOQ
#if !NO_ASSEMBLY_SCANNING
namespace Ninject.Tests.Unit.CompiledModuleLoaderPluginTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using Moq;
    using Ninject.Modules;
    using Xunit;

    public class CompiledModuleLoaderPluginContext
    {
        protected readonly CompiledModuleLoaderPlugin loaderPlugin;
        protected readonly Mock<IKernel> kernelMock;
        protected readonly string moduleFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Ninject.Tests.TestModule.dll");
        protected readonly string assemblyFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Ninject.Tests.TestAssembly.dll");

        public CompiledModuleLoaderPluginContext()
        {
            kernelMock = new Mock<IKernel>();
            loaderPlugin = new CompiledModuleLoaderPlugin(kernelMock.Object, new AssemblyNameRetriever());
        }
    }

    public class WhenLoadModulesIsCalled : CompiledModuleLoaderPluginContext
    {
        [Fact]
        public void CallsLoadMethodOnKernelWithAssemblies()
        {
            var expected = Assembly.LoadFrom(this.moduleFilename).GetName().Name;

            IEnumerable<Assembly> actual = null;
            kernelMock.Setup(x => x.Load(It.IsAny<IEnumerable<Assembly>>()))
                      .Callback<IEnumerable<Assembly>>(m => actual = m);

            loaderPlugin.LoadModules(new[] { this.moduleFilename });
            actual.Should().NotBeNull();
            actual.Count().Should().Be(1);
            actual.Where(a => a.GetName().Name == expected).Should().NotBeEmpty();
        }

        [Fact]
        public void DoesNotLoadAssembliesWithoutModules()
        {
            loaderPlugin.LoadModules(new[] { this.assemblyFilename });
            kernelMock.Verify(k => k.Load(It.Is<IEnumerable<Assembly>>(p => p.Any())), Times.Never());
        }
    }
}
#endif //!NO_ASSEMBLY_SCANNING
#endif