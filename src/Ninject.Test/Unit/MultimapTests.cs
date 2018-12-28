namespace Ninject.Tests.Unit
{
    using System;

    using FluentAssertions;

    using Ninject.Infrastructure;

    using Xunit;

    public class MultimapTests
    {
        [Fact]
        public void CanAddMultipleValuesToSameKey()
        {
            var testee = new Multimap<int, int>();

            testee.Add(1, 1);
            testee.Add(1, 2);

            testee[1].Should().Contain(1);
            testee[1].Should().Contain(2);
        }

        [Fact]
        public void Add_ShouldThrowArgumentNullExceptionWhenKeyIsNull()
        {
            const string key = null;
            const string value = "foo";
            var target = new Multimap<string, string>();

            var actualException = Assert.Throws<ArgumentNullException>(() => target.Add(key, value));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(key), actualException.ParamName);
        }

        [Fact]
        public void Add_ShouldThrowArgumentNullExceptionWhenValueIsNull()
        {
            const string key = "FOO";
            const string value = null;
            var target = new Multimap<string, string>();

            var actualException = Assert.Throws<ArgumentNullException>(() => target.Add(key, value));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(value), actualException.ParamName);
        }

        [Fact]
        public void ContainsKey_ShouldThrowArgumentNullExceptionWhenKeyIsNull()
        {
            const string key = null;
            var target = new Multimap<string, int>();

            var actualException = Assert.Throws<ArgumentNullException>(() => target.ContainsKey(key));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(key), actualException.ParamName);
        }

        [Fact]
        public void ContainsValue_ShouldThrowArgumentNullExceptionWhenKeyIsNull()
        {
            const string key = null;
            const string value = "123";
            var target = new Multimap<string, string>();

            var actualException = Assert.Throws<ArgumentNullException>(() => target.ContainsValue(key, value));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(key), actualException.ParamName);
        }

        [Fact]
        public void ContainsValue_ShouldThrowArgumentNullExceptionWhenValueIsNull()
        {
            const string key = "SVC";
            const string value = null;
            var target = new Multimap<string, string>();

            var actualException = Assert.Throws<ArgumentNullException>(() => target.ContainsValue(key, value));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(value), actualException.ParamName);
        }

        [Fact]
        public void Indexer_ShouldThrowArgumentNullExceptionWhenKeyIsNull()
        {
            const string key = null;
            var target = new Multimap<string, string>();

            var actualException = Assert.Throws<ArgumentNullException>(() => target[key]);

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(key), actualException.ParamName);
        }

        [Fact]
        public void Indexer_ShouldCreateEmptyListOfValuesWhenSpecifiedKeyDoesNotExist()
        {
            const string key = "SVC";
            var target = new Multimap<string, string>();

            var actual = target[key];

            Assert.NotNull(actual);
            Assert.Empty(actual);
            Assert.True(target.ContainsKey(key));
            Assert.Same(actual, target[key]);
        }

        [Fact]
        public void Remove_ShouldThrowArgumentNullExceptionWhenKeyIsNull()
        {
            const string key = null;
            const string value = "ABC";
            var target = new Multimap<string, string>();

            var actualException = Assert.Throws<ArgumentNullException>(() => target.Remove(key, value));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(key), actualException.ParamName);
        }

        [Fact]
        public void Remove_ShouldThrowArgumentNullExceptionWhenValueIsNull()
        {
            const string key = "SVC";
            const string value = null;
            var target = new Multimap<string, string>();

            var actualException = Assert.Throws<ArgumentNullException>(() => target.Remove(key, value));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(value), actualException.ParamName);
        }

        [Fact]
        public void Remove_ShouldReturnFalseWhenNoValuesExistForSpecifiedKey()
        {
            const string key = "SVC";
            const string value = "ABC";
            var target = new Multimap<string, string>();

            var actual = target.Remove(key, value);

            Assert.False(actual);
        }

        [Fact]
        public void Remove_ShouldReturnFalseWhenValueDoesNotExistForSpecifiedKey()
        {
            const string key = "SVC";
            const string value = "ABC";
            var target = new Multimap<string, string>();
            target.Add(key, "FOO");

            var actual = target.Remove(key, value);

            Assert.False(actual);
        }

        [Fact]
        public void Remove_ShouldReturnTrueWhenValueExistsForSpecifiedKey()
        {
            const string key = "SVC";
            const string value = "ABC";
            var target = new Multimap<string, string>();
            target.Add(key, value);

            var actual = target.Remove(key, value);

            Assert.True(actual);
            Assert.False(target.ContainsValue(key, value));
            Assert.Empty(target[key]);
        }

        [Fact]
        public void RemoveAll_ShouldThrowArgumentNullExceptionWhenKeyIsNull()
        {
            const string key = null;
            var target = new Multimap<string, string>();

            var actualException = Assert.Throws<ArgumentNullException>(() => target.RemoveAll(key));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(key), actualException.ParamName);
        }
    }
}