using BenchmarkDotNet.Attributes;
using System;

namespace Ninject.Benchmarks
{
    [MemoryDiagnoser]
    public class NinjectSettingsBenchmark
    {
        private NinjectSettings _settings;

        public NinjectSettingsBenchmark()
        {
            _settings = new NinjectSettings();
        }

        [Benchmark]
        public void InjectAttribute()
        {
            var injectAttribute = _settings.InjectAttribute;
            if (injectAttribute == null)
                throw new System.Exception();
        }

        [Benchmark]
        public void CachePruningInterval()
        {
            var interval = _settings.CachePruningInterval;
            if (interval == TimeSpan.Zero)
                throw new System.Exception();
        }

        [Benchmark]
        public void LoadExtensions()
        {
            var loadExtensions = _settings.LoadExtensions;
            if (!loadExtensions)
                throw new System.Exception();
        }
    }
}
