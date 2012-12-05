using System;
using FluentAssertions;
using Ninject.Infrastructure.Introspection;
using Ninject.Tests.Fakes;
using Xunit;

class TypeWithNoNamespace
{
    
}

namespace Ninject.Tests.Unit
{
    public class TestGenericType<TOuter>
    {
        public class InnerType {}

        public class InnerGenericType<TInner> {}
    }

    public class FormatTypeTests
    {
        public class InnerType {}

        [Fact]
        public void BuiltInTypesFormatttedAsCSharpName()
        {
            typeof(bool).Format().Should().Be("bool");
            typeof(char).Format().Should().Be("char");
            typeof(sbyte).Format().Should().Be("sbyte");
            typeof(short).Format().Should().Be("short");
            typeof(ushort).Format().Should().Be("ushort");
            typeof(int).Format().Should().Be("int");
            typeof(uint).Format().Should().Be("uint");
            typeof(long).Format().Should().Be("long");
            typeof(ulong).Format().Should().Be("ulong");
            typeof(float).Format().Should().Be("float");
            typeof(double).Format().Should().Be("double");
            typeof(decimal).Format().Should().Be("decimal");
            typeof(DateTime).Format().Should().Be("DateTime");
            typeof(string).Format().Should().Be("string");
        }

        [Fact]
        public void SimpleTypeFormattedAsShortName()
        {
            typeof(Sword).Format().Should().Be("Sword");
            typeof(TypeWithNoNamespace).Format().Should().Be("TypeWithNoNamespace");
        }

        [Fact]
        public void InnerTypeFormattedWithOuterTypeName()
        {
            typeof(InnerType).Format().Should().Be("FormatTypeTests+InnerType");
            typeof(TestGenericType<>.InnerType).Format().Should().Be("TestGenericType{TOuter}+InnerType");
            typeof(TestGenericType<int>.InnerType).Format().Should().Be("TestGenericType{int}+InnerType");
        }

        [Fact]
        public void GenericTypeFormattedWithGenericArguments()
        {
            typeof(TestGenericType<>).Format().Should().Be("TestGenericType{TOuter}");
            typeof(TestGenericType<int>).Format().Should().Be("TestGenericType{int}");
        }

        [Fact]
        public void GenericInnerTypeFormattedWithOuterTypeNameAndGenericArguments()
        {
            typeof(TestGenericType<>.InnerGenericType<>).Format().Should().Be("TestGenericType{TOuter}+InnerGenericType{TInner}");
            typeof(TestGenericType<int>.InnerGenericType<long>).Format().Should().Be("TestGenericType{int}+InnerGenericType{long}");
        }

        [Fact]
        public void AnonymousTypeFormattedSimply()
        {
            var o = new {
                Hello = "World!",
                a = 1,
                b = false,
                c = DateTime.MinValue,
                d = new Uri("http://example.org"),
                e = new {
                    x = 0,
                },
                f = 3,
                g = 4,
                h = 5,
                i = 15,
            };

            o.GetType().Format().Should().Be("AnonymousType");
        }
    }
}
