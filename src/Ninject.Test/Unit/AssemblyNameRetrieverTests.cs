#if !NO_MOQ
#if !NO_ASSEMBLY_SCANNING
namespace Ninject.Tests.Unit
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using Ninject.Modules;
    using Xunit;

    public class AssemblyNameRetrieverTests
    {
        public class AssemblyNameRetrieverContext
        {
            protected readonly AssemblyNameRetriever AssemblyNameRetriever;
            protected readonly string ModuleFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Ninject.Tests.TestModule.dll");
            protected readonly string AssemblyFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Ninject.Tests.TestAssembly.dll");

            public AssemblyNameRetrieverContext()
            {
                this.AssemblyNameRetriever = new AssemblyNameRetriever();
            }
        }

        public class WhenGetAssemblyNamesIsCalledWithFileName : AssemblyNameRetrieverContext
        {
            [Fact]
            public void AssemblyNamesOfMatchingAssembliesAreReturned()
            {
                var expected = Assembly.LoadFrom(this.ModuleFilename).GetName();

                var actualNames = this.AssemblyNameRetriever.GetAssemblyNames(
                    new[] { this.ModuleFilename, this.AssemblyFilename },
                    asm => asm.FullName.StartsWith("Ninject.Tests.TestModule"));

                var assemblyFullNames = actualNames.Select(a => a.FullName).ToList();
                assemblyFullNames.Should().BeEquivalentTo(new[] { expected.FullName });
            }
        }
        
        public class WhenGetAssemblyNamesIsCalledWithAssemblyName : AssemblyNameRetrieverContext
        {
            [Fact]
            public void AssemblyNamesOfMatchingAssembliesAreReturned()
            {
                var expected = Assembly.LoadFrom(this.ModuleFilename).GetName();

                var actualNames = this.AssemblyNameRetriever.GetAssemblyNames(
                    new[] { expected.FullName },
                    asm => true);

                var assemblyFullNames = actualNames.Select(a => a.FullName).ToList();
                assemblyFullNames.Should().BeEquivalentTo(new[] { expected.FullName });
            }
        }

        public class WhenGetAssemblyNamesIsCalledWithUnknownAssemblyName : AssemblyNameRetrieverContext
        {
            [Fact]
            public void WillBeIgnored()
            {
                var actualNames = this.AssemblyNameRetriever.GetAssemblyNames(new[] { "Blah" }, asm => true);

                actualNames.Should().BeEmpty();
            }
        }
    }
}
#endif //!NO_ASSEMBLY_SCANNING
#endif