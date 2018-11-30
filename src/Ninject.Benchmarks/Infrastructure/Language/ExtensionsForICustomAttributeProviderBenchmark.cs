using System;
using System.Reflection;
using BenchmarkDotNet.Attributes;

using Ninject.Infrastructure.Language;
using Ninject.Tests.Fakes;

namespace Ninject.Benchmarks.Infrastructure.Language
{
    [MemoryDiagnoser]
    public class ExtensionsForICustomAttributeProviderBenchmark
    {
        private PropertyInfo _property_AttributeNotDefined;
        private PropertyInfo _property_AttributeDefinedOnDeclaringType;
        private PropertyInfo _property_AttributeDefinedOnBaseType;
        private Type _attributeType;

        public ExtensionsForICustomAttributeProviderBenchmark()
        {
            _property_AttributeNotDefined = typeof(MyService).GetProperty("Warrior");
            _property_AttributeDefinedOnDeclaringType = typeof(MyService).GetProperty("Id");
            _property_AttributeDefinedOnBaseType = typeof(MyService).GetProperty("Name");

            _attributeType = typeof(InjectAttribute);
        }

        [Benchmark]
        public void IsPrivate_PrivateGetterAndPrivateSetter()
        {
            _property_AttributeNotDefined.HasAttribute(_attributeType);
        }

        [Benchmark]
        public void IsPrivate_PrivateGetterAndPublicSetter()
        {
            _property_AttributeNotDefined.HasAttribute(_attributeType);
        }

        [Benchmark]
        public void IsPrivate_PrivateGetterAndNoSetter()
        {
            _property_AttributeNotDefined.HasAttribute(_attributeType);
        }

        [Benchmark]
        public void IsPrivate_PublicGetterAndPrivateSetter()
        {
            _property_AttributeNotDefined.HasAttribute(_attributeType);
        }

        [Benchmark]
        public void IsPrivate_PublicGetterAndPublicSetter()
        {
            _property_AttributeNotDefined.HasAttribute(_attributeType);
        }

        [Benchmark]
        public void IsPrivate_PublicGetterAndNoSetter()
        {
            _property_AttributeNotDefined.HasAttribute(_attributeType);
        }

        [Benchmark]
        public void HasAttribute_PropertyInfo_AttributeDefinedOnDeclaringType()
        {
            _property_AttributeDefinedOnDeclaringType.HasAttribute(_attributeType);
        }

        [Benchmark]
        public void HasAttribute_PropertyInfo_AttributeDefinedOnBaseType()
        {
            _property_AttributeDefinedOnBaseType.HasAttribute(_attributeType);
        }

        public abstract class MyServiceBase
        {
            [Inject]
            public abstract string Name { get; }

            public abstract int Id { get; }

            public abstract IWarrior Warrior { get; }
        }

        public abstract class MyService : MyServiceBase
        {
            public override string Name { get; }

            [Inject]
            public override int Id { get; }

            public override IWarrior Warrior
            {
                get { return new FootSoldier(); }
            }
        }
    }
}
