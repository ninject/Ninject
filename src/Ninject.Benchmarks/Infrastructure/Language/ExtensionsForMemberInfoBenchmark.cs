using System.Reflection;
using BenchmarkDotNet.Attributes;

using Ninject.Infrastructure.Language;

namespace Ninject.Benchmarks.Infrastructure.Language
{
    [MemoryDiagnoser]
    public class ExtensionsForMemberInfoBenchmark
    {
        private PropertyInfo _property_PrivateGetterAndPrivateSetter;
        private PropertyInfo _property_PrivateGetterAndPublicSetter;
        private PropertyInfo _property_PrivateGetterAndNoSetter;
        private PropertyInfo _property_PublicGetterAndPrivateSetter;
        private PropertyInfo _property_PublicGetterAndPublicSetter;
        private PropertyInfo _property_PublicGetterAndNoSetter;
        private PropertyInfo _property_Indexer;
        private PropertyInfo _property_NoIndexer_Base;

        public ExtensionsForMemberInfoBenchmark()
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            _property_PrivateGetterAndPrivateSetter = typeof(MyService).GetProperty("PrivateGetterAndPrivateSetter", bindingFlags);
            _property_PrivateGetterAndPublicSetter = typeof(MyService).GetProperty("PrivateGetterAndPublicSetter", bindingFlags);
            _property_PrivateGetterAndNoSetter = typeof(MyService).GetProperty("PrivateGetterAndNoSetter", bindingFlags);
            _property_PublicGetterAndPrivateSetter = typeof(MyService).GetProperty("PublicGetterAndPrivateSetter", bindingFlags);
            _property_PublicGetterAndPublicSetter = typeof(MyService).GetProperty("PublicGetterAndPublicSetter", bindingFlags);
            _property_PublicGetterAndNoSetter = typeof(MyService).GetProperty("PublicGetterAndNoSetter", bindingFlags);

            _property_Indexer = typeof(MyService).GetProperty("Item");
            _property_NoIndexer_Base = typeof(MyServiceBase).GetProperty("NoIndexer");
        }

        [Benchmark]
        public void IsPrivate_PrivateGetterAndPrivateSetter()
        {
            _property_PrivateGetterAndPrivateSetter.IsPrivate();
        }

        [Benchmark]
        public void IsPrivate_PrivateGetterAndPublicSetter()
        {
            _property_PrivateGetterAndPublicSetter.IsPrivate();
        }

        [Benchmark]
        public void IsPrivate_PrivateGetterAndNoSetter()
        {
            _property_PrivateGetterAndNoSetter.IsPrivate();
        }

        [Benchmark]
        public void IsPrivate_PublicGetterAndPrivateSetter()
        {
            _property_PublicGetterAndPrivateSetter.IsPrivate();
        }

        [Benchmark]
        public void IsPrivate_PublicGetterAndPublicSetter()
        {
            _property_PublicGetterAndPublicSetter.IsPrivate();
        }

        [Benchmark]
        public void IsPrivate_PublicGetterAndNoSetter()
        {
            _property_PublicGetterAndNoSetter.IsPrivate();
        }

        [Benchmark]
        public void GetPropertyFromDeclaredType_Indexer()
        {
            _property_NoIndexer_Base.GetPropertyFromDeclaredType(_property_Indexer, BindingFlags.Public | BindingFlags.Instance);
        }

        [Benchmark]
        public void GetPropertyFromDeclaredType_NoIndexer()
        {
            _property_Indexer.GetPropertyFromDeclaredType(_property_NoIndexer_Base, BindingFlags.Public | BindingFlags.Instance);
        }

        public abstract class MyServiceBase
        {
            public virtual string this[int index, string name]
            {
                get { return null; }
            }

            public string NoIndexer
            {
                get { return null; }
            }
        }

        public class MyService : MyServiceBase
        {
            private string PrivateGetterAndPrivateSetter { get; set; }
            public string PrivateGetterAndPublicSetter { private get; set; }
            private string PrivateGetterAndNoSetter { get; }
            public string PublicGetterAndPrivateSetter { private get; set; }
            public string PublicGetterAndPublicSetter { private get; set; }
            public string PublicGetterAndNoSetter { private get; set; }

            public override string this[int index, string name]
            {
                get { return null; }
            }
        }
    }
}
