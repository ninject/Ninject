using System.Reflection;
using Xunit;

using Ninject.Infrastructure.Language;
using System;

namespace Ninject.Tests.Unit.Infrastructure.Language
{
    public class ExtensionsForMemberInfoTests
    {
        private BindingFlags _bindingFlags;

        public ExtensionsForMemberInfoTests()
        {
            _bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        }

        [Fact]
        public void IsPrivate_PrivateGetterAndPrivateSetter()
        {
            var property = typeof(MyService).GetProperty("PrivateGetterAndPrivateSetter", _bindingFlags);
            Assert.True(property.IsPrivate());
        }

        [Fact]
        public void IsPrivate_PrivateGetterAndPublicSetter()
        {
            var property = typeof(MyService).GetProperty(nameof(MyService.PrivateGetterAndPublicSetter), _bindingFlags);
            Assert.False(property.IsPrivate());
        }

        [Fact]
        public void IsPrivate_PrivateGetterAndNoSetter()
        {
            var property = typeof(MyService).GetProperty("PrivateGetterAndNoSetter", _bindingFlags);
            Assert.True(property.IsPrivate());
        }

        [Fact]
        public void IsPrivate_PublicGetterAndPrivateSetter()
        {
            var property = typeof(MyService).GetProperty(nameof(MyService.PublicGetterAndPrivateSetter), _bindingFlags);
            Assert.False(property.IsPrivate());
        }

        [Fact]
        public void IsPrivate_PublicGetterAndPublicSetter()
        {
            var property = typeof(MyService).GetProperty(nameof(MyService.PublicGetterAndPublicSetter), _bindingFlags);
            Assert.False(property.IsPrivate());
        }

        [Fact]
        public void IsPrivate_PublicGetterAndNoSetter()
        {
            var property = typeof(MyService).GetProperty(nameof(MyService.PublicGetterAndNoSetter), _bindingFlags);
            Assert.False(property.IsPrivate());
        }

        [Fact]
        public void GetPropertyFromDeclaredType_Indexer()
        {
            var indexer = typeof(MyService).GetProperty("Item", new[] { typeof(int), typeof(string) });
            var noIndexerBase = typeof(MyServiceBase).GetProperty(nameof(MyService.NoIndexer));

            var actual = noIndexerBase.GetPropertyFromDeclaredType(indexer, BindingFlags.Public | BindingFlags.Instance);

            Assert.NotNull(actual);
            Assert.Equal(typeof(MyServiceBase), actual.DeclaringType);
            Assert.Equal(indexer.Name, actual.Name);
            Assert.Equal(indexer.PropertyType, actual.PropertyType);

            var actualIndexParameters = actual.GetIndexParameters();

            Assert.Equal(2, actualIndexParameters.Length);
            Assert.Equal("index", actualIndexParameters[0].Name);
            Assert.Equal(typeof(int), actualIndexParameters[0].ParameterType);
            Assert.Equal("name", actualIndexParameters[1].Name);
            Assert.Equal(typeof(string), actualIndexParameters[1].ParameterType);
        }

        [Fact]
        public void GetPropertyFromDeclaredType_NoIndexer()
        {
            var indexer = typeof(MyService).GetProperty("Item", new[] { typeof(int), typeof(string) });
            var noIndexerBase = typeof(MyServiceBase).GetProperty(nameof(MyService.NoIndexer));

            var actual = indexer.GetPropertyFromDeclaredType(noIndexerBase, BindingFlags.Public | BindingFlags.Instance);

            Assert.NotNull(actual);
            Assert.Equal(typeof(MyServiceBase), actual.DeclaringType);
            Assert.Equal(noIndexerBase.Name, actual.Name);
            Assert.Equal(noIndexerBase.PropertyType, actual.PropertyType);
            Assert.Equal(noIndexerBase.GetIndexParameters(), actual.GetIndexParameters());
        }

        [Fact]
        public void HasAttribute_PropertyInfo_Match_AttributeOnBaseType()
        {
            var propertyInitialized = typeof(MyService).GetProperty(nameof(MyService.Initialized));
            Assert.True(propertyInitialized.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyServiceBase), propertyInitialized.DeclaringType);

            var propertyName = typeof(MyService).GetProperty(nameof(MyService.Name));
            Assert.True(propertyName.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyService), propertyName.DeclaringType);
        }

        [Fact]
        public void HasAttribute_PropertyInfo_Match_AttributeOnDeclaringType()
        {
            var propertyId = typeof(MyService).GetProperty(nameof(MyService.Id));
            Assert.True(propertyId.HasAttribute(typeof(InjectAttribute)));

            var propertyInitialized = typeof(MyServiceBase).GetProperty(nameof(MyServiceBase.Initialized));
            Assert.True(propertyInitialized.HasAttribute(typeof(InjectAttribute)));

            var propertyName = typeof(MyServiceBase).GetProperty(nameof(MyService.Name));
            Assert.True(propertyName.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyServiceBase), propertyName.DeclaringType);
        }

        [Fact]
        public void HasAttribute_PropertyInfo_NoMatch()
        {
            var propertyId = typeof(MyService).GetProperty(nameof(MyService.Id));
            Assert.False(propertyId.HasAttribute(typeof(ObsoleteAttribute)));

            var propertyType = typeof(MyService).GetProperty(nameof(MyService.Type));
            Assert.False(propertyType.HasAttribute(typeof(InjectAttribute)));

            var propertyInitialized = typeof(MyServiceBase).GetProperty(nameof(MyServiceBase.Initialized));
            Assert.False(propertyInitialized.HasAttribute(typeof(ObsoleteAttribute)));
        }

        [Fact]
        public void HasAttribute_MethodInfo_Match_AttributeOnBaseType()
        {
            var executeBool = typeof(MyService).GetMethod(nameof(MyService.Execute), new[] { typeof(bool) });
            Assert.True(executeBool.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyServiceBase), executeBool.DeclaringType);

            var executeNoArgs = typeof(MyService).GetMethod(nameof(MyService.Execute), new Type[0]);
            Assert.True(executeNoArgs.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyService), executeNoArgs.DeclaringType);
        }

        [Fact]
        public void HasAttribute_MethodInfo_Match_AttributeOnDeclaringType()
        {
            var executeNoArgs = typeof(MyServiceBase).GetMethod(nameof(MyService.Execute), new Type[0]);
            Assert.True(executeNoArgs.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyServiceBase), executeNoArgs.DeclaringType);

            var executeInt = typeof(MyService).GetMethod(nameof(MyService.Execute), new[] { typeof(int) });
            Assert.True(executeInt.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyService), executeInt.DeclaringType);
        }

        [Fact]
        public void HasAttribute_MethodInfo_NoMatch()
        {
            var executeInt = typeof(MyServiceBase).GetMethod(nameof(MyService.Execute), new[] { typeof(int) });
            Assert.False(executeInt.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyServiceBase), executeInt.DeclaringType);

            var executeNoArgs = typeof(MyServiceBase).GetMethod(nameof(MyService.Execute), new Type[0]);
            Assert.False(executeNoArgs.HasAttribute(typeof(ObsoleteAttribute)));
            Assert.Equal(typeof(MyServiceBase), executeNoArgs.DeclaringType);
        }

        [Fact]
        public void HasAttribute_ConstructorInfo_Match_AttributeOnDeclaringType()
        {
            var constructorNoArgs = typeof(MyServiceBase).GetConstructor(new Type[0]);
            Assert.True(constructorNoArgs.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyServiceBase), constructorNoArgs.DeclaringType);

            var constructorInt = typeof(MyService).GetConstructor(new[] { typeof(int) });
            Assert.True(constructorInt.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyService), constructorInt.DeclaringType);
        }

        [Fact]
        public void HasAttribute_ConstructorInfo_NoMatch()
        {
            var constructorInt = typeof(MyServiceBase).GetConstructor(new[] { typeof(int) });
            Assert.False(constructorInt.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyServiceBase), constructorInt.DeclaringType);

            var constructorNoArgs = typeof(MyServiceBase).GetConstructor(new Type[0]);
            Assert.False(constructorNoArgs.HasAttribute(typeof(ObsoleteAttribute)));
            Assert.Equal(typeof(MyServiceBase), constructorNoArgs.DeclaringType);

            var constructorBool = typeof(MyService).GetConstructor(new[] { typeof(bool) });
            Assert.False(constructorBool.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyService), constructorBool.DeclaringType);
        }

        [Fact]
        public void HasAttribute_MemberInfo_Match_AttributeOnBaseType()
        {
            var propertyInitialized = (MemberInfo) typeof(MyService).GetProperty(nameof(MyService.Initialized));
            Assert.True(propertyInitialized.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyServiceBase), propertyInitialized.DeclaringType);

            var propertyName = (MemberInfo) typeof(MyService).GetProperty(nameof(MyService.Name));
            Assert.True(propertyName.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyService), propertyName.DeclaringType);

            var executeBool = (MemberInfo) typeof(MyService).GetMethod(nameof(MyService.Execute), new[] { typeof(bool) });
            Assert.True(executeBool.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyServiceBase), executeBool.DeclaringType);

            var executeNoArgs = (MemberInfo) typeof(MyService).GetMethod(nameof(MyService.Execute), new Type[0]);
            Assert.True(executeNoArgs.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyService), executeNoArgs.DeclaringType);
        }

        [Fact]
        public void HasAttribute_MemberInfo_Match_AttributeOnDeclaringType()
        {
            var propertyId = (MemberInfo) typeof(MyService).GetProperty(nameof(MyService.Id));
            Assert.True(propertyId.HasAttribute(typeof(InjectAttribute)));

            var propertyInitialized = (MemberInfo) typeof(MyServiceBase).GetProperty(nameof(MyServiceBase.Initialized));
            Assert.True(propertyInitialized.HasAttribute(typeof(InjectAttribute)));

            var propertyName = (MemberInfo) typeof(MyServiceBase).GetProperty(nameof(MyService.Name));
            Assert.True(propertyName.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyServiceBase), propertyName.DeclaringType);

            var executeNoArgs = (MemberInfo) typeof(MyServiceBase).GetMethod(nameof(MyService.Execute), new Type[0]);
            Assert.True(executeNoArgs.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyServiceBase), executeNoArgs.DeclaringType);

            var executeInt = (MemberInfo) typeof(MyService).GetMethod(nameof(MyService.Execute), new[] { typeof(int) });
            Assert.True(executeInt.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyService), executeInt.DeclaringType);

            var constructorNoArgs = (MemberInfo) typeof(MyServiceBase).GetConstructor(new Type[0]);
            Assert.True(constructorNoArgs.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyServiceBase), constructorNoArgs.DeclaringType);

            var constructorInt = (MemberInfo) typeof(MyService).GetConstructor(new[] { typeof(int) });
            Assert.True(constructorInt.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyService), constructorInt.DeclaringType);
        }

        [Fact]
        public void HasAttribute_MemberInfo_NoMatch()
        {
            var propertyId = (MemberInfo) typeof(MyService).GetProperty(nameof(MyService.Id));
            Assert.False(propertyId.HasAttribute(typeof(ObsoleteAttribute)));

            var propertyType = (MemberInfo)typeof(MyService).GetProperty(nameof(MyService.Type));
            Assert.False(propertyType.HasAttribute(typeof(InjectAttribute)));

            var propertyInitialized = (MemberInfo) typeof(MyServiceBase).GetProperty(nameof(MyServiceBase.Initialized));
            Assert.False(propertyInitialized.HasAttribute(typeof(ObsoleteAttribute)));

            var executeInt = (MemberInfo) typeof(MyServiceBase).GetMethod(nameof(MyService.Execute), new[] { typeof(int) });
            Assert.False(executeInt.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyServiceBase), executeInt.DeclaringType);

            var executeNoArgs = (MemberInfo) typeof(MyServiceBase).GetMethod(nameof(MyService.Execute), new Type[0]);
            Assert.False(executeNoArgs.HasAttribute(typeof(ObsoleteAttribute)));
            Assert.Equal(typeof(MyServiceBase), executeNoArgs.DeclaringType);

            var constructorInt = (MemberInfo) typeof(MyServiceBase).GetConstructor(new[] { typeof(int) });
            Assert.False(constructorInt.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyServiceBase), constructorInt.DeclaringType);

            var constructorNoArgs = (MemberInfo) typeof(MyServiceBase).GetConstructor(new Type[0]);
            Assert.False(constructorNoArgs.HasAttribute(typeof(ObsoleteAttribute)));
            Assert.Equal(typeof(MyServiceBase), constructorNoArgs.DeclaringType);

            var constructorBool = (MemberInfo) typeof(MyService).GetConstructor(new[] { typeof(bool) });
            Assert.False(constructorBool.HasAttribute(typeof(InjectAttribute)));
            Assert.Equal(typeof(MyService), constructorBool.DeclaringType);
        }

        public abstract class MyServiceBase
        {
            [Inject]
            public MyServiceBase()
            {
            }

            public MyServiceBase(int iterations)
            {
            }

            [Inject]
            public MyServiceBase(bool immediately)
            {
            }

            public virtual string this[int index, string name]
            {
                get { return null; }
            }

            public string NoIndexer
            {
                get { return null; }
            }

            [Inject]
            public virtual string Name
            {
                get { return null; }
            }

            public virtual Type Type
            {
                get { return null; }
            }

            public virtual int Id
            {
                get { return 0; }
            }

            [Inject]
            public bool Initialized
            {
                get { return true; }
            }

            [Inject]
            public virtual void Execute()
            {
            }

            public virtual void Execute(int iterations)
            {
            }

            [Inject]
            public void Execute(bool immediately)
            {
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

            public MyService()
            {
            }

            [Inject]
            public MyService (int iterations)
                : base(iterations)
            {
            }

            public MyService(bool immediately)
                : base(immediately)
            {
            }


            public override string this[int index, string name]
            {
                get { return null; }
            }

            public override string Name
            {
                get { return null; }
            }

            public override Type Type
            {
                get { return null; }
            }

            [Inject]
            public override int Id
            {
                get { return 0; }
            }

            public override void Execute()
            {
            }

            [Inject]
            public override void Execute(int iterations)
            {
            }
        }
    }
}
