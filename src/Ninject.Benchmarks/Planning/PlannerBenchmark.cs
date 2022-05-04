using BenchmarkDotNet.Attributes;
using Ninject.Components;
using Ninject.Planning;
using Ninject.Planning.Strategies;
using Ninject.Tests.Fakes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ninject.Benchmarks.Planning
{
    [MemoryDiagnoser]
    public class PlannerBenchmark
    {
        private const int MaxTypeCount = 1_800;
        private static readonly TimeSpan StrategyExecuteDelay = TimeSpan.FromMilliseconds(0.2);

        private List<IPlanningStrategy> _strategiesWithDelay;
        private List<IPlanningStrategy> _strategiesWithoutDelay;
        private Planner _plannerWithDelay;
        private Planner _plannerWithoutDelay;
        private Type[] _types;

        public PlannerBenchmark()
        {
            _strategiesWithDelay = new List<IPlanningStrategy>
                {
                    new DelayPlanningStrategy(),
                    new DelayPlanningStrategy(),
                    new DelayPlanningStrategy(),
                    new DelayPlanningStrategy()
                };
            _strategiesWithoutDelay = new List<IPlanningStrategy>
                {
                    new NoOpPlanningStrategy(),
                    new NoOpPlanningStrategy(),
                    new NoOpPlanningStrategy(),
                    new NoOpPlanningStrategy()
                };
            _types = typeof(string).Assembly.GetTypes();

            if (_types.Length < 1_800)
                throw new Exception($"Expected min. {MaxTypeCount} types in {typeof(string).Assembly.FullName}, but was {_types.Length}.");
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _plannerWithDelay = new Planner(_strategiesWithDelay);
            _plannerWithDelay.GetPlan(typeof(Monk));

            _plannerWithoutDelay = new Planner(_strategiesWithoutDelay);
        }

        [Benchmark]
        public void GetPlan_Existing_Serial()
        {
            _plannerWithDelay.GetPlan(typeof(Monk));
        }

        [Benchmark(OperationsPerInvoke = 25_000)]
        public void GetPlan_Existing_Parallel()
        {
            const int threads = 25;
            const int iterationsPerThread = 1000;
            var type = GetType();

            var tasks = Enumerable.Range(1, threads).Select((_) =>
            {
                return Task.Factory.StartNew(() =>
                {
                    for (var i = 0; i < iterationsPerThread; i++)
                    {
                        _plannerWithDelay.GetPlan(type);
                    }
                });
            });

            Task.WaitAll(tasks.ToArray());
        }

        [Benchmark(OperationsPerInvoke = MaxTypeCount)]
        public void GetPlan_New_WithDelay_Serial()
        {
            for (var i = 0; i < MaxTypeCount; i++)
            {
                _plannerWithDelay.GetPlan(_types[i]);
            }

            _plannerWithDelay = new Planner(_strategiesWithDelay);
        }

        [Benchmark(OperationsPerInvoke = MaxTypeCount)]
        public void GetPlan_New_WithoutDelay_Serial()
        {
            for (var i = 0; i < MaxTypeCount; i++)
            {
                _plannerWithDelay.GetPlan(_types[i]);
            }

            _plannerWithoutDelay = new Planner(_strategiesWithoutDelay);
        }

        [Benchmark(OperationsPerInvoke = MaxTypeCount)]
        public void GetPlan_New_WithDelay_Parallel()
        {
            const int threadCount = 36;
            const int iterationsPerThread = 50;

            var tasks = Enumerable.Range(1, threadCount).Select((i) =>
            {
                return Task.Factory.StartNew(() =>
                {
                    // Each thread gets its unique range of types.
                    var typeIndex = i * iterationsPerThread;
                    var typeEndIndex = typeIndex + iterationsPerThread;

                    while (typeIndex < typeEndIndex)
                    {
                        _plannerWithDelay.GetPlan(_types[typeIndex++]);
                    }
                });
            });

            Task.WaitAll(tasks.ToArray());

            // create new planner to ensure next exection of method again creates new plans
            _plannerWithDelay = new Planner(_strategiesWithDelay);
        }

        [Benchmark(OperationsPerInvoke = MaxTypeCount)]
        public void GetPlan_New_WithoutDelay_Parallel()
        {
            const int threadCount = 36;
            const int iterationsPerThread = 50;

            var tasks = Enumerable.Range(1, threadCount).Select((i) =>
            {
                return Task.Factory.StartNew(() =>
                {
                    // Each thread gets its unique range of types.
                    var typeIndex = i * iterationsPerThread;
                    var typeEndIndex = typeIndex + iterationsPerThread;

                    while (typeIndex < typeEndIndex)
                    {
                        _plannerWithoutDelay.GetPlan(_types[typeIndex++]);
                    }
                });
            });

            Task.WaitAll(tasks.ToArray());

            // create new planner to ensure next exection of method again creates new plans
            _plannerWithoutDelay = new Planner(_strategiesWithoutDelay);
        }

        public class DelayPlanningStrategy : NinjectComponent, IPlanningStrategy
        {
            public void Execute(IPlan plan)
            {
                Task.Delay(StrategyExecuteDelay);
            }
        }

        public class NoOpPlanningStrategy : NinjectComponent, IPlanningStrategy
        {
            public void Execute(IPlan plan)
            {
            }
        }
    }
}
