using Moq;
using Ninject.Activation;
using Ninject.Activation.Caching;
using System;
using Xunit;

namespace Ninject.Tests.Unit.Activation.Caching
{
    public class CacheTest
    {
        private Mock<IPipeline> _pipelineMock;
        private Mock<ICachePruner> _cachePrunerMock;

        public CacheTest()
        {
            _pipelineMock = new Mock<IPipeline>(MockBehavior.Strict);
            _cachePrunerMock = new Mock<ICachePruner>(MockBehavior.Strict);
        }

        [Fact]
        public void Constructor_ShouldActiveCachePrunerForCache()
        {
            var cache = CreateCache();

            Assert.NotNull(cache.Pipeline);
            Assert.Same(_pipelineMock.Object, cache.Pipeline);

            _cachePrunerMock.Verify(p => p.Start(cache), Times.Once());
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullExceptionWhenPipelineIsNull()
        {
            const IPipeline pipeline = null;
            var cachePruner = _cachePrunerMock.Object;

            var actual = Assert.Throws<ArgumentNullException>(() => new Cache(pipeline, cachePruner));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(pipeline), actual.ParamName);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullExceptionWhenCachePrunerIsNull()
        {
            var pipeline = _pipelineMock.Object;
            const ICachePruner cachePruner = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new Cache(pipeline, cachePruner));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(cachePruner), actual.ParamName);
        }

        [Fact]
        public void Remember_ShouldThrowArgumentNullExceptionWhenContextIsNull()
        {
            const IContext context = null;
            var instanceReference = new InstanceReference { Instance = new object() };
            var cache = CreateCache();

            var actual = Assert.Throws<ArgumentNullException>(() => cache.Remember(context, instanceReference));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(context), actual.ParamName);
        }

        [Fact]
        public void TryGet_ShouldThrowArgumentNullExceptionWhenContextIsNull()
        {
            const IContext context = null;
            var cache = CreateCache();

            var actual = Assert.Throws<ArgumentNullException>(() => cache.TryGet(context));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(context), actual.ParamName);
        }

        private Cache CreateCache()
        {
            _cachePrunerMock.Setup(p => p.Start(It.IsNotNull<IPruneable>()));

            return new Cache(_pipelineMock.Object, _cachePrunerMock.Object);
        }
    }
}
