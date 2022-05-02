using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Moq;
using Ninject.Components;
using Ninject.Modules;
using Xunit;

namespace Ninject.Tests.Unit.ModuleLoaderTests
{
    public class ModuleLoaderContext
    {
        protected readonly string executingAssemblyDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
        protected readonly ModuleLoader moduleLoader;
        protected readonly Mock<IKernel> kernelMock;
        protected readonly Mock<IComponentContainer> componentsMock;
        protected readonly Mock<IModuleLoaderPlugin> fooPluginMock;
        protected readonly Mock<IModuleLoaderPlugin> barPluginMock;

        public ModuleLoaderContext()
        {
            this.kernelMock = new Mock<IKernel>();
            this.componentsMock = new Mock<IComponentContainer>();
            this.fooPluginMock = new Mock<IModuleLoaderPlugin>();
            this.barPluginMock = new Mock<IModuleLoaderPlugin>();
            this.moduleLoader = new ModuleLoader(this.kernelMock.Object);

            var plugins = new[] { this.fooPluginMock.Object, this.barPluginMock.Object };

            this.kernelMock.SetupGet(x => x.Components).Returns(this.componentsMock.Object);
            this.componentsMock.Setup(x => x.GetAll<IModuleLoaderPlugin>()).Returns(plugins);
            this.fooPluginMock.SetupGet(x => x.SupportedExtensions).Returns(new[] { ".foo" });
            this.barPluginMock.SetupGet(x => x.SupportedExtensions).Returns(new[] { ".bar" });
        }
    }

    public class WhenLoadModulesIsCalled : ModuleLoaderContext
    {
        [Fact]
        public void PassesMatchingFilesToAppropriatePlugin()
        {
            this.moduleLoader.LoadModules(new[] { "TestModules/*" });

            var fooFiles = new[] { Path.Combine(this.executingAssemblyDirectory, @"TestModules\test.foo") };
            var barFiles = new[] { Path.Combine(this.executingAssemblyDirectory, @"TestModules\test.bar") };

            this.fooPluginMock.Verify(x => x.LoadModules(It.Is<IEnumerable<string>>(e => e.SequenceEqual(fooFiles))));
            this.barPluginMock.Verify(x => x.LoadModules(It.Is<IEnumerable<string>>(e => e.SequenceEqual(barFiles))));
        }
    }
}