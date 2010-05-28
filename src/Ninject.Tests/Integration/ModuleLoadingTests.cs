#if !NO_ASSEMBLY_SCANNING
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Ninject.Activation.Blocks;
using Ninject.Tests.Fakes;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Integration.ModuleLoadingTests
{
	public class ModuleLoadingContext
	{
		protected readonly StandardKernel kernel;
		protected readonly string assemblyFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestModules\Ninject.Tests.TestModule.dll");

		public ModuleLoadingContext()
		{
			kernel = new StandardKernel();
		}
	}

	public class WhenLoadIsCalledWithAssemblies : ModuleLoadingContext
	{
		[Fact(Skip = "Issue with signing in NAnt exception, need to investigate")]
		public void ModulesContainedInAssembliesAreLoaded()
		{
			var assembly = Assembly.Load(new AssemblyName { CodeBase = assemblyFilename });

			kernel.Load(assembly);

			var modules = kernel.GetModules().ToArray();

			modules.ShouldNotBeEmpty();
			modules[0].ShouldBeInstanceOf(assembly.GetType("Ninject.Tests.TestModules.TestModule"));
			modules[0].Kernel.ShouldBe(kernel);
		}
	}
}
#endif //!NO_ASSEMBLY_SCANNING