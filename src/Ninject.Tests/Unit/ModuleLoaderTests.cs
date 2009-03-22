using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using Ninject.Modules;
using Xunit;

namespace Ninject.Tests.Unit.ModuleLoaderTests
{
	public class ModuleLoaderContext
	{
		protected readonly ModuleLoader moduleLoader;
		protected readonly Mock<IKernel> kernelMock;
		protected readonly Mock<IModuleLoaderPlugin> fooPluginMock;
		protected readonly Mock<IModuleLoaderPlugin> barPluginMock;

		public ModuleLoaderContext()
		{
			kernelMock = new Mock<IKernel>();
			fooPluginMock = new Mock<IModuleLoaderPlugin>();
			barPluginMock = new Mock<IModuleLoaderPlugin>();
			moduleLoader = new ModuleLoader(kernelMock.Object, new[] { fooPluginMock.Object, barPluginMock.Object });

			fooPluginMock.SetupGet(x => x.SupportedExtensions).Returns(new[] { ".foo" });
			barPluginMock.SetupGet(x => x.SupportedExtensions).Returns(new[] { ".bar" });
		}
	}

	public class WhenLoadModulesIsCalled : ModuleLoaderContext
	{
		[Fact]
		public void PassesMatchingFilesToAppropriatePlugin()
		{
			moduleLoader.LoadModules(new[] { "TestModules/*" });

			var fooFiles = new[] { Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestModules\test.foo") };
			var barFiles = new[] { Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestModules\test.bar") };

			fooPluginMock.Verify(x => x.LoadModules(It.Is<IEnumerable<string>>(e => e.SequenceEqual(fooFiles))));
			barPluginMock.Verify(x => x.LoadModules(It.Is<IEnumerable<string>>(e => e.SequenceEqual(barFiles))));
		}
	}
}