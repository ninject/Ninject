using System;
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
        private MethodInfo _method_Execute;
        private ConstructorInfo _constructor_NoArgs;
        private MemberInfo _member_property_Indexer;
        private MemberInfo _member_property_NoIndexer_Base;
        private MemberInfo _member_method_Execute;
        private MemberInfo _member_constructor_NoArgs;
        private Type _injectAttributeType;
        private Type _obsoleteAttributeType;

        public ExtensionsForMemberInfoBenchmark()
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            _property_PrivateGetterAndPrivateSetter = typeof(MyService).GetProperty("PrivateGetterAndPrivateSetter", bindingFlags);
            _property_PrivateGetterAndPublicSetter = typeof(MyService).GetProperty(nameof(MyService.PrivateGetterAndPublicSetter), bindingFlags);
            _property_PrivateGetterAndNoSetter = typeof(MyService).GetProperty("PrivateGetterAndNoSetter", bindingFlags);
            _property_PublicGetterAndPrivateSetter = typeof(MyService).GetProperty(nameof(MyService.PublicGetterAndPrivateSetter), bindingFlags);
            _property_PublicGetterAndPublicSetter = typeof(MyService).GetProperty(nameof(MyService.PublicGetterAndPublicSetter), bindingFlags);
            _property_PublicGetterAndNoSetter = typeof(MyService).GetProperty(nameof(MyService.PublicGetterAndNoSetter), bindingFlags);

            _property_Indexer = typeof(MyService).GetProperty("Item");
            _property_NoIndexer_Base = typeof(MyServiceBase).GetProperty(nameof(MyService.NoIndexer));
            _method_Execute = typeof(MyService).GetMethod(nameof(MyService.Execute));
            _constructor_NoArgs = typeof(MyService).GetConstructor(new Type[0]);

            _member_property_Indexer = typeof(MyService).GetProperty("Item");
            _member_property_NoIndexer_Base = typeof(MyServiceBase).GetProperty(nameof(MyService.NoIndexer));
            _member_method_Execute = typeof(MyService).GetMethod(nameof(MyService.Execute));
            _member_constructor_NoArgs = typeof(MyService).GetConstructor(new Type[0]);

            _injectAttributeType = typeof(InjectAttribute);
            _obsoleteAttributeType = typeof(ObsoleteAttribute);
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

        [Benchmark]
        public void HasAttribute_PropertyInfo_Match()
        {
            _property_Indexer.HasAttribute(_injectAttributeType);
        }

        [Benchmark]
        public void HasAttribute_PropertyInfo_NoMatch()
        {
            _property_Indexer.HasAttribute(_obsoleteAttributeType);
        }

        [Benchmark]
        public void HasAttribute_MethodInfo_Match()
        {
            _method_Execute.HasAttribute(_injectAttributeType);
        }

        [Benchmark]
        public void HasAttribute_MethodInfo_NoMatch()
        {
            _method_Execute.HasAttribute(_obsoleteAttributeType);
        }

        [Benchmark]
        public void HasAttribute_ConstructorInfo_Match()
        {
            _constructor_NoArgs.HasAttribute(_injectAttributeType);
        }

        [Benchmark]
        public void HasAttribute_ConstructorInfo_NoMatch()
        {
            _constructor_NoArgs.HasAttribute(_obsoleteAttributeType);
        }

        [Benchmark]
        public void HasAttribute_MemberInfo_PropertyInfo_Match()
        {
            _member_property_Indexer.HasAttribute(_injectAttributeType);
        }

        [Benchmark]
        public void HasAttribute_MemberInfo_PropertyInfo_NoMatch()
        {
            _member_property_Indexer.HasAttribute(_obsoleteAttributeType);
        }

        [Benchmark]
        public void HasAttribute_MemberInfo_MethodInfo_Match()
        {
            _member_method_Execute.HasAttribute(_injectAttributeType);
        }

        [Benchmark]
        public void HasAttribute_MemberInfo_MethodInfo_NoMatch()
        {
            _member_method_Execute.HasAttribute(_obsoleteAttributeType);
        }

        [Benchmark]
        public void HasAttribute_MemberInfo_ConstructorInfo_Match()
        {
            _member_constructor_NoArgs.HasAttribute(_injectAttributeType);
        }

        [Benchmark]
        public void HasAttribute_MemberInfo_ConstructorInfo_NoMatch()
        {
            _member_constructor_NoArgs.HasAttribute(_obsoleteAttributeType);
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

            [Inject]
            public MyService()
            {
            }

            [Inject]
            public override string this[int index, string name]
            {
                get { return null; }
            }

            [Inject]
            public void Execute()
            {
            }
        }
    }
}
