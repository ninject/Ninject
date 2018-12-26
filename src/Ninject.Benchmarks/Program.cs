using BenchmarkDotNet.Running;

namespace Ninject.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            var b = new ReadOnlyKernelBenchmark();
            b.GlobalSetup();

            for (var i = 0; i < 5000; i++)
                b.Resolve_Unique_MatchingBinding();
            */

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
