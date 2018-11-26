using BenchmarkDotNet.Attributes;
using Ninject.Planning;
using Ninject.Planning.Directives;
using System;

namespace Ninject.Benchmarks.Planning
{
    [MemoryDiagnoser]
    public class PlanBenchmark
    {
        private Plan _plan;

        public PlanBenchmark()
        {
            _plan = new Plan(GetType());
            _plan.Add(new MyDirectiveOne());
            _plan.Add(CreateConstructorInjectionDirective());
            _plan.Add(CreateConstructorInjectionDirective());
            _plan.Add(CreateConstructorInjectionDirective());
            _plan.Add(CreateConstructorInjectionDirective());
            _plan.Add(new MyDirectiveTwo());
            _plan.Add(CreatePropertyInjectionDirective());
            _plan.Add(CreatePropertyInjectionDirective());
            _plan.Add(CreatePropertyInjectionDirective());
            _plan.Add(CreatePropertyInjectionDirective());
            _plan.Add(CreateMethodInjectionDirective());
        }

        [Benchmark]
        public void HasDirective_Match_First()
        {
            _plan.Has<MyDirectiveOne>();
        }

        [Benchmark]
        public void HasDirective_Match_Middle()
        {
            _plan.Has<MyDirectiveTwo>();
        }

        [Benchmark]
        public void HasDirective_Match_Last()
        {
            _plan.Has<MethodInjectionDirective>();
        }

        [Benchmark]
        public void HasDirective_NoMatch()
        {
            _plan.Has<MyDirectiveThree>();
        }

        [Benchmark]
        public void GetOne_Match_First()
        {
            _plan.GetOne<MyDirectiveOne>();
        }

        [Benchmark]
        public void GetOne_Match_Middle()
        {
            _plan.GetOne<MyDirectiveTwo>();
        }

        [Benchmark]
        public void GetOne_Match_Last()
        {
            _plan.GetOne<MethodInjectionDirective>();
        }

        [Benchmark]
        public void GetOne_NoMatch()
        {
            _plan.GetOne<MyDirectiveThree>();
        }

        [Benchmark]
        public void GetAll_Match()
        {
            _plan.GetAll<ConstructorInjectionDirective>();
        }

        [Benchmark]
        public void GetAll_NoMatch()
        {
            _plan.GetAll<MyDirectiveThree>();
        }

        private static ConstructorInjectionDirective CreateConstructorInjectionDirective()
        {
            return new ConstructorInjectionDirective(typeof(MyService).GetConstructor(new Type[0]), (_) => null);
        }

        private static PropertyInjectionDirective CreatePropertyInjectionDirective()
        {
            return new PropertyInjectionDirective(typeof(MyService).GetProperty("Name"), (target, value) => { });
        }

        private static MethodInjectionDirective CreateMethodInjectionDirective()
        {
            return new MethodInjectionDirective(typeof(MyService).GetMethod("Run"), (target, arguments) => { });
        }

        public class MyDirectiveOne : IDirective
        {
        }

        public class MyDirectiveTwo : IDirective
        {
        }

        public class MyDirectiveThree : IDirective
        {
        }

        public class MyService
        {
            public MyService()
            {
            }

            public string Name { get; }

            public void Run()
            {
            }
        }
    }
}
