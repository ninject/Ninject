using Moq;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Infrastructure.Disposal;
using Ninject.Planning.Bindings;
using System;
using Xunit;

namespace Ninject.Tests.Unit.Activation.Caching
{
    public class CacheTest
    {
        private Mock<IPipeline> _pipelineMock;
        private Mock<ICachePruner> _cachePrunerMock;
        private Mock<IContext> _contextMock;
        private Mock<IBinding> _bindingMock;
        private Mock<IBindingConfiguration> _bindingConfigurationMock;
        private MockSequence _mockSequence;

        public CacheTest()
        {
            _pipelineMock = new Mock<IPipeline>(MockBehavior.Strict);
            _cachePrunerMock = new Mock<ICachePruner>(MockBehavior.Strict);
            _contextMock = new Mock<IContext>(MockBehavior.Strict);
            _bindingMock = new Mock<IBinding>(MockBehavior.Strict);
            _bindingConfigurationMock = new Mock<IBindingConfiguration>(MockBehavior.Strict);

            _mockSequence = new MockSequence();
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
        public void Remember_ContextAndReference_ShouldThrowArgumentNullExceptionWhenContextIsNull()
        {
            const IContext context = null;
            var instanceReference = new InstanceReference { Instance = new object() };
            var cache = CreateCache();

            var actual = Assert.Throws<ArgumentNullException>(() => cache.Remember(context, instanceReference));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(context), actual.ParamName);
        }

        [Fact]
        public void Remember_ContextAndScopeAndReference_ShouldThrowArgumentNullExceptionWhenContextIsNull()
        {
            const IContext context = null;
            var scope = new object();
            var instanceReference = new InstanceReference { Instance = new object() };
            var cache = CreateCache();

            var actual = Assert.Throws<ArgumentNullException>(() => cache.Remember(context, scope, instanceReference));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(context), actual.ParamName);
        }

        [Fact]
        public void Remember_ContextAndScopeAndReference_ShouldThrowArgumentNullExceptionWhenScopeIsNull()
        {
            var context = _contextMock.Object;
            const object scope = null;
            var instanceReference = new InstanceReference { Instance = new object() };
            var cache = CreateCache();

            var actual = Assert.Throws<ArgumentNullException>(() => cache.Remember(context, scope, instanceReference));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(scope), actual.ParamName);
        }

        [Fact]
        public void Remember_ContextAndScopeAndReference_ShouldKeepWeakReferenceToScope()
        {
            var context = _contextMock.Object;
            var cache = CreateCache();
            var instance = new object();
            var instanceReference = new InstanceReference { Instance = instance };

            var scope = Remember(cache, _contextMock, _bindingConfigurationMock.Object, instanceReference);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.False(scope.IsAlive);
        }

        [Fact]
        public void Remember_ContextAndScopeAndReference_ShouldKeepWeakReferenceToDisposableScope()
        {
            var context = _contextMock.Object;
            var cache = CreateCache();
            var instance = new object();
            var instanceReference = new InstanceReference { Instance = instance };

            _contextMock.InSequence(_mockSequence).Setup(p => p.Binding).Returns(_bindingMock.Object);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.BindingConfiguration).Returns(_bindingConfigurationMock.Object);
            _pipelineMock.InSequence(_mockSequence).Setup(p => p.Deactivate(_contextMock.Object, instanceReference));

            var disposabledScope = RememberDisposableScope(cache, _contextMock, _bindingConfigurationMock.Object, instanceReference);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.False(disposabledScope.IsAlive);

            _pipelineMock.Verify(p => p.Deactivate(_contextMock.Object, instanceReference), Times.Never());
        }

        [Fact]
        public void Remember_ContextAndScopeAndReference_ShouldSubscripeToDisposedEventWhenScopeImplementsINotifyWhenDisposedAndClearCacheForTheScopeUponDispose()
        {
            var context = _contextMock.Object;
            var scope = new DisposableScope();
            var cache = CreateCache();
            var instance = new object();
            var instanceReference1 = new InstanceReference { Instance = instance };
            var instanceReference2 = new InstanceReference { Instance = instance };

            Remember(cache, _contextMock, new object(), _bindingConfigurationMock.Object, instanceReference1);
            Remember(cache, _contextMock, scope, _bindingConfigurationMock.Object, instanceReference2);

            _pipelineMock.InSequence(_mockSequence).Setup(p => p.Deactivate(_contextMock.Object, instanceReference2));

            Assert.Equal(2, cache.Count);

            scope.Dispose();

            Assert.Equal(1, cache.Count);
            Assert.Null(cache.TryGet(context, scope));

            _pipelineMock.Verify(p => p.Deactivate(_contextMock.Object, instanceReference2));
        }

        [Fact]
        public void TryGet_Context_ShouldThrowArgumentNullExceptionWhenContextIsNull()
        {
            const IContext context = null;
            var cache = CreateCache();

            var actual = Assert.Throws<ArgumentNullException>(() => cache.TryGet(context));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(context), actual.ParamName);
        }

        [Fact]
        public void TryGet_ContextAndScope_ShouldThrowArgumentNullExceptionWhenContextIsNull()
        {
            const IContext context = null;
            var scope = new object();
            var cache = CreateCache();

            var actual = Assert.Throws<ArgumentNullException>(() => cache.TryGet(context, scope));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(context), actual.ParamName);
        }

        [Fact]
        public void TryGet_ContextAndScope_ShouldThrowArgumentNullExceptionWhenScopeIsNull()
        {
            var context = _contextMock.Object;
            const object scope = null;
            var cache = CreateCache();

            var actual = Assert.Throws<ArgumentNullException>(() => cache.TryGet(context, scope));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(scope), actual.ParamName);
        }

        [Fact]
        public void TryGet_ContextAndScope_ShouldReturnNullWhenNothingHasBeenRememberedForSpecifiedScope()
        {
            var context = _contextMock.Object;
            var scope = new object();
            var cache = CreateCache();

            var actual = cache.TryGet(context, scope);

            Assert.Null(actual);
        }

        [Fact]
        public void TryGet_ContextAndScope_ShouldReturnNullWhenNothingHasBeenRememberedForBindingConfiguration()
        {
            var otherBindingConfiguration = new Mock<IBindingConfiguration>(MockBehavior.Strict).Object;
            var context = _contextMock.Object;
            var scope = new object();
            var cache = CreateCache();
            var instance = new object();
            var instanceReference = new InstanceReference { Instance = instance };

            Remember(cache, _contextMock, scope, otherBindingConfiguration, instanceReference);

            _contextMock.InSequence(_mockSequence).Setup(p => p.Binding).Returns(_bindingMock.Object);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.BindingConfiguration).Returns(_bindingConfigurationMock.Object);

            var actual = cache.TryGet(context, scope);

            Assert.Null(actual);

            _contextMock.Verify(p => p.Binding, Times.Once());
            _bindingMock.Verify(p => p.BindingConfiguration, Times.Once());
        }

        [Fact]
        public void TryGet_ContextAndScope_ShouldReturnRememberedInstanceForBindingConfigurationWhenContextDoesNotHaveInferredGenericArguments()
        {
            var otherBindingConfiguration = new Mock<IBindingConfiguration>(MockBehavior.Strict).Object;
            var context = _contextMock.Object;
            var scope = new object();
            var cache = CreateCache();
            var instance = new object();
            var instanceReference = new InstanceReference { Instance = instance };

            Remember(cache, _contextMock, scope, _bindingConfigurationMock.Object, instanceReference);

            _contextMock.InSequence(_mockSequence).Setup(p => p.Binding).Returns(_bindingMock.Object);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.BindingConfiguration).Returns(_bindingConfigurationMock.Object);
            _contextMock.InSequence(_mockSequence).SetupGet(p => p.HasInferredGenericArguments).Returns(false);

            var actual = cache.TryGet(context, scope);

            Assert.NotNull(actual);
            Assert.Same(instance, actual);

            _contextMock.Verify(p => p.Binding, Times.Once());
            _bindingMock.Verify(p => p.BindingConfiguration, Times.Once());
        }

        [Fact]
        public void TryGet_ContextAndScope_ShouldReturnFirstRememberedInstanceForBindingConfigurationWhenMoreThanOneInstanceIsRememberedAndContextDoesNotHaveInferredGenericArguments()
        {
            var otherBindingConfiguration = new Mock<IBindingConfiguration>(MockBehavior.Strict).Object;
            var context = _contextMock.Object;
            var scope = new object();
            var cache = CreateCache();
            var instance = new object();
            var instanceReference = new InstanceReference { Instance = instance };

            Remember(cache, _contextMock, scope, otherBindingConfiguration, new InstanceReference { Instance = new object() });
            Remember(cache, _contextMock, scope, _bindingConfigurationMock.Object, instanceReference);
            Remember(cache, _contextMock, scope, _bindingConfigurationMock.Object, new InstanceReference { Instance = new object() });
            Remember(cache, _contextMock, scope, otherBindingConfiguration, new InstanceReference { Instance = new object() });

            _contextMock.InSequence(_mockSequence).Setup(p => p.Binding).Returns(_bindingMock.Object);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.BindingConfiguration).Returns(_bindingConfigurationMock.Object);
            _contextMock.InSequence(_mockSequence).SetupGet(p => p.HasInferredGenericArguments).Returns(false);

            var actual = cache.TryGet(context, scope);

            Assert.NotNull(actual);
            Assert.Same(instance, actual);

            _contextMock.Verify(p => p.Binding, Times.Once());
            _bindingMock.Verify(p => p.BindingConfiguration, Times.Once());
        }

        [Fact]
        public void TryGet_ContextAndScope_ShouldReturnNullWhenContextHasInferredGenericArgumentsAndContextOfOnlyRememberedInstanceForBindingConfigurationHasNoGenericArguments()
        {
            var rememberedInstanceContextMock = new Mock<IContext>(MockBehavior.Strict);
            var rememberedInstanceContextGenericArguments = new Type[0];
            var context = _contextMock.Object;
            var contextGenericArguments = new[] { typeof(string) };
            var scope = new object();
            var cache = CreateCache();
            var instance = new object();
            var instanceReference = new InstanceReference { Instance = instance };

            Remember(cache, rememberedInstanceContextMock, scope, _bindingConfigurationMock.Object, instanceReference);

            _contextMock.InSequence(_mockSequence).Setup(p => p.Binding).Returns(_bindingMock.Object);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.BindingConfiguration).Returns(_bindingConfigurationMock.Object);
            _contextMock.InSequence(_mockSequence).SetupGet(p => p.HasInferredGenericArguments).Returns(true);
            rememberedInstanceContextMock.InSequence(_mockSequence).Setup(p => p.GenericArguments).Returns(rememberedInstanceContextGenericArguments);
            _contextMock.InSequence(_mockSequence).Setup(p => p.GenericArguments).Returns(contextGenericArguments);

            var actual = cache.TryGet(context, scope);

            Assert.Null(actual);

            _contextMock.Verify(p => p.Binding, Times.Once());
            _bindingMock.Verify(p => p.BindingConfiguration, Times.Once());
            _contextMock.Verify(p => p.HasInferredGenericArguments, Times.Once());
            rememberedInstanceContextMock.Verify(p => p.GenericArguments, Times.Once());
            _contextMock.Verify(p => p.GenericArguments, Times.Once());
        }

        [Fact]
        public void TryGet_ContextAndScope_ShouldReturnFirstRememberInstanceWithMatchingGenericArgumentsWhenContextHasInferredGenericArguments()
        {
            var rememberedInstance1ContextMock = new Mock<IContext>(MockBehavior.Strict);
            var rememberedInstance1ContextGenericArguments = new[] { typeof(string), typeof(int) };
            var rememberedInstance2ContextMock = new Mock<IContext>(MockBehavior.Strict);
            var rememberedInstance2ContextGenericArguments = new[] { typeof(string), typeof(bool) };
            var context = _contextMock.Object;
            var contextGenericArguments = new[] { typeof(string), typeof(bool) };
            var scope = new object();
            var cache = CreateCache();
            var instance = new object();
            var instanceReference = new InstanceReference { Instance = instance };

            Remember(cache, rememberedInstance1ContextMock, scope, _bindingConfigurationMock.Object, new InstanceReference { Instance = new object()});
            Remember(cache, rememberedInstance2ContextMock, scope, _bindingConfigurationMock.Object, instanceReference);

            _contextMock.InSequence(_mockSequence).Setup(p => p.Binding).Returns(_bindingMock.Object);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.BindingConfiguration).Returns(_bindingConfigurationMock.Object);
            _contextMock.InSequence(_mockSequence).SetupGet(p => p.HasInferredGenericArguments).Returns(true);
            rememberedInstance1ContextMock.InSequence(_mockSequence).Setup(p => p.GenericArguments).Returns(rememberedInstance1ContextGenericArguments);
            _contextMock.InSequence(_mockSequence).Setup(p => p.GenericArguments).Returns(contextGenericArguments);
            _contextMock.InSequence(_mockSequence).SetupGet(p => p.HasInferredGenericArguments).Returns(true);
            rememberedInstance2ContextMock.InSequence(_mockSequence).Setup(p => p.GenericArguments).Returns(rememberedInstance2ContextGenericArguments);
            _contextMock.InSequence(_mockSequence).Setup(p => p.GenericArguments).Returns(contextGenericArguments);

            var actual = cache.TryGet(context, scope);

            Assert.NotNull(actual);
            Assert.Same(instance, actual);

            _contextMock.Verify(p => p.Binding, Times.Once());
            _bindingMock.Verify(p => p.BindingConfiguration, Times.Once());
            _contextMock.Verify(p => p.HasInferredGenericArguments, Times.Exactly(2));
            rememberedInstance1ContextMock.Verify(p => p.GenericArguments, Times.Once());
            rememberedInstance2ContextMock.Verify(p => p.GenericArguments, Times.Once());
            _contextMock.Verify(p => p.GenericArguments, Times.Exactly(2));
        }

        private Cache CreateCache()
        {
            _cachePrunerMock.Setup(p => p.Start(It.IsNotNull<IPruneable>()));

            return new Cache(_pipelineMock.Object, _cachePrunerMock.Object);
        }

        private void Remember(Cache cache, Mock<IContext> contextMock, object scope, IBindingConfiguration bindingConfiguration, InstanceReference instanceReference)
        {
            contextMock.InSequence(_mockSequence).Setup(p => p.Binding).Returns(_bindingMock.Object);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.BindingConfiguration).Returns(bindingConfiguration);

            cache.Remember(contextMock.Object, scope, instanceReference);

            contextMock.Reset();
            _bindingMock.Reset();
        }

        private WeakReference Remember(Cache cache, Mock<IContext> contextMock, IBindingConfiguration bindingConfiguration, InstanceReference instanceReference)
        {
            var scope = new object();

            contextMock.InSequence(_mockSequence).Setup(p => p.Binding).Returns(_bindingMock.Object);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.BindingConfiguration).Returns(bindingConfiguration);

            cache.Remember(contextMock.Object, scope, instanceReference);

            contextMock.Reset();
            _bindingMock.Reset();

            return new WeakReference(scope);
        }

        private WeakReference RememberDisposableScope(Cache cache, Mock<IContext> contextMock, IBindingConfiguration bindingConfiguration, InstanceReference instanceReference)
        {
            var disposableScope = new DisposableScope();

            cache.Remember(contextMock.Object, disposableScope, instanceReference);

            return new WeakReference(disposableScope);
        }

        public class DisposableScope : DisposableObject
        {
        }
    }
}