using System;
using Xunit;
using Ninject.Planning.Bindings;


namespace Ninject.Tests.Unit.Planning.Bindings
{
    public class BindingMetadataTests
    {
        private BindingMetadata _bindingMetadata;

        public BindingMetadataTests()
        {
            _bindingMetadata = new BindingMetadata();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetOfT_Key_ShouldThrowArgumentExceptionWhenKeyKeyIsNullOrEmpty(string key)
        {
            var actualException = Assert.Throws<ArgumentException>(() => _bindingMetadata.Get<int>(key));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(key), actualException.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetOfT_KeyAndDefaultValue_ShouldThrowArgumentExceptionWhenKeyIsNullOrEmpty(string key)
        {
            var actualException = Assert.Throws<ArgumentException>(() => _bindingMetadata.Get<int>(key, 5));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(key), actualException.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Has_ShouldThrowArgumentExceptionWhenKeyIsNullOrEmpty(string key)
        {
            var actualException = Assert.Throws<ArgumentException>(() => _bindingMetadata.Has(key));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(key), actualException.ParamName);
        }

        [Fact]
        public void Has_ShouldReturnFalseWhenKeyDoesNotExist()
        {
            Assert.False(_bindingMetadata.Has("name"));
            _bindingMetadata.Set("Name", "Foo");
            Assert.False(_bindingMetadata.Has("name"));
        }

        [Fact]
        public void Has_ShouldReturnTrueWhenKeyExists()
        {
            _bindingMetadata.Set("name", "Foo");
            Assert.True(_bindingMetadata.Has("name"));

            _bindingMetadata.Set("address", null);
            Assert.True(_bindingMetadata.Has("address"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Set_ShouldThrowArgumentExceptionWhenKeyIsNullOrEmpty(string key)
        {
            var actualException = Assert.Throws<ArgumentException>(() => _bindingMetadata.Set(key, 5));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(key), actualException.ParamName);
        }

        [Theory]
        [InlineData("name", "Foo")]
        [InlineData("name", "Bar")]
        [InlineData("age", 25)]
        [InlineData("name", null)]
        public void Set(string key, object value)
        {
            _bindingMetadata.Set(key, value);

            Assert.Equal(value, _bindingMetadata.Get<object>(key));
        }

        [Fact]
        public void Set_ShouldOverwriteExistingValue()
        {
            _bindingMetadata.Set("name", "Foo");
            _bindingMetadata.Set("name", "Bar");

            Assert.Equal("Bar", _bindingMetadata.Get<string>("name"));
        }
    }
}
