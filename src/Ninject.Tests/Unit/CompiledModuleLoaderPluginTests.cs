#if !NO_ASSEMBLY_SCANNING
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Moq;
using Ninject.Modules;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Unit.CompiledModuleLoaderPluginTests
{
	public class CompiledModuleLoaderPluginContext
	{
		protected readonly CompiledModuleLoaderPlugin loaderPlugin;
		protected readonly Mock<IKernel> kernelMock;
		protected readonly string assemblyFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestModules\Ninject.Tests.TestModule.dll");

		public CompiledModuleLoaderPluginContext()
		{
			kernelMock = new Mock<IKernel>();
			loaderPlugin = new CompiledModuleLoaderPlugin(kernelMock.Object);
		}
	}

	public class WhenLoadModulesIsCalled : CompiledModuleLoaderPluginContext
	{
		[Fact(Skip = "Need to bring TestModule assembly into git")]
		public void CallsLoadMethodOnKernelWithAssemblies()
		{
			Assembly expected = Assembly.Load("Ninject.Tests.TestModule");
			expected.ShouldNotBeNull();

			loaderPlugin.LoadModules(new[] { assemblyFilename });

			kernelMock.Verify(x => x.Load(It.Is<IEnumerable<Assembly>>(p => p.Contains(expected))));
		}
	}
}
#endif //!NO_ASSEMBLY_SCANNING