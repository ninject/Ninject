using Moq;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Infrastructure.Disposal;
using Ninject.Planning.Bindings;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Ninject.Tests.Unit.Activation.Caching
{
    public class CacheTest
    {
        private Mock<IPipeline> _pipelineMock;
        private Mock<ICachePruner> _cachePrunerMock;
        private Mock<IContext> _contextMock1;
        private Mock<IContext> _contextMock2;
        private Mock<IBinding> _bindingMock1;
        private Mock<IBinding> _bindingMock2;
        private Mock<IBindingConfiguration> _bindingConfigurationMock1;
        private Mock<IBindingConfiguration> _bindingConfigurationMock2;
        private MockSequence _mockSequence;

        public CacheTest()
        {
            _pipelineMock = new Mock<IPipeline>(MockBehavior.Strict);
            _cachePrunerMock = new Mock<ICachePruner>(MockBehavior.Strict);
            _contextMock1 = new Mock<IContext>(MockBehavior.Strict);
            _contextMock2 = new Mock<IContext>(MockBehavior.Strict);
            _bindingMock1 = new Mock<IBinding>(MockBehavior.Strict);
            _bindingMock2 = new Mock<IBinding>(MockBehavior.Strict);
            _bindingConfigurationMock1 = new Mock<IBindingConfiguration>(MockBehavior.Strict);
            _bindingConfigurationMock2 = new Mock<IBindingConfiguration>(MockBehavior.Strict);

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
            var context = _contextMock1.Object;
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
            var context = _contextMock1.Object;
            var cache = CreateCache();
            var instance = new object();
            var instanceReference = new InstanceReference { Instance = instance };

            var scope = Remember(cache, _contextMock1, _bindingConfigurationMock1.Object, instanceReference);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.False(scope.IsAlive);
        }

        [Fact]
        public void Remember_ContextAndScopeAndReference_ShouldKeepWeakReferenceToDisposableScope()
        {
            var context = _contextMock1.Object;
            var cache = CreateCache();
            var instance = new object();
            var instanceReference = new InstanceReference { Instance = instance };

            _contextMock1.InSequence(_mockSequence).Setup(p => p.Binding).Returns(_bindingMock1.Object);
            _bindingMock1.InSequence(_mockSequence).Setup(p => p.BindingConfiguration).Returns(_bindingConfigurationMock1.Object);
            _pipelineMock.InSequence(_mockSequence).Setup(p => p.Deactivate(_contextMock1.Object, instanceReference));

            var disposabledScope = RememberDisposableScope(cache, _contextMock1, _bindingConfigurationMock1.Object, instanceReference);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.False(disposabledScope.IsAlive);

            _pipelineMock.Verify(p => p.Deactivate(_contextMock1.Object, instanceReference), Times.Never());
        }

        [Fact]
        public void Remember_ContextAndScopeAndReference_ShouldSubscripeToDisposedEventWhenScopeImplementsINotifyWhenDisposedAndClearCacheForTheScopeUponDispose()
        {
            var context = _contextMock1.Object;
            var scope = new DisposableScope();
            var cache = CreateCache();
            var instance = new object();
            var instanceReference1 = new InstanceReference { Instance = instance };
            var instanceReference2 = new InstanceReference { Instance = instance };

            Remember(cache, _contextMock1, new object(), _bindingConfigurationMock1.Object, instanceReference1);
            Remember(cache, _contextMock1, scope, _bindingConfigurationMock1.Object, instanceReference2);

            _pipelineMock.InSequence(_mockSequence).Setup(p => p.Deactivate(_contextMock1.Object, instanceReference2));

            Assert.Equal(2, cache.Count);

            scope.Dispose();

            Assert.Equal(1, cache.Count);
            Assert.Null(cache.TryGet(context, scope));

            _pipelineMock.Verify(p => p.Deactivate(_contextMock1.Object, instanceReference2));
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
            var context = _contextMock1.Object;
            const object scope = null;
            var cache = CreateCache();

            var actual = Assert.Throws<ArgumentNullException>(() => cache.TryGet(context, scope));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(scope), actual.ParamName);
        }

        [Fact]
        public void TryGet_ContextAndScope_ShouldReturnNullWhenNothingHasBeenRememberedForSpecifiedScope()
        {
            var context = _contextMock1.Object;
            var scope = new object();
            var cache = CreateCache();

            var actual = cache.TryGet(context, scope);

            Assert.Null(actual);
        }

        [Fact]
        public void TryGet_ContextAndScope_ShouldReturnNullWhenNothingHasBeenRememberedForBindingConfiguration()
        {
            var otherBindingConfiguration = new Mock<IBindingConfiguration>(MockBehavior.Strict).Object;
            var context = _contextMock1.Object;
            var scope = new object();
            var cache = CreateCache();
            var instance = new object();
            var instanceReference = new InstanceReference { Instance = instance };

            Remember(cache, _contextMock1, scope, otherBindingConfiguration, instanceReference);

            _contextMock1.InSequence(_mockSequence).Setup(p => p.Binding).Returns(_bindingMock1.Object);
            _bindingMock1.InSequence(_mockSequence).Setup(p => p.BindingConfiguration).Returns(_bindingConfigurationMock1.Object);

            var actual = cache.TryGet(context, scope);

            Assert.Null(actual);

            _contextMock1.Verify(p => p.Binding, Times.Once());
            _bindingMock1.Verify(p => p.BindingConfiguration, Times.Once());
        }

        [Fact]
        public void TryGet_ContextAndScope_ShouldReturnRememberedInstanceForBindingConfigurationWhenContextDoesNotHaveInferredGenericArguments()
        {
            var otherBindingConfiguration = new Mock<IBindingConfiguration>(MockBehavior.Strict).Object;
            var context = _contextMock1.Object;
            var scope = new object();
            var cache = CreateCache();
            var instance = new object();
            var instanceReference = new InstanceReference { Instance = instance };

            Remember(cache, _contextMock1, scope, _bindingConfigurationMock1.Object, instanceReference);

            _contextMock1.InSequence(_mockSequence).Setup(p => p.Binding).Returns(_bindingMock1.Object);
            _bindingMock1.InSequence(_mockSequence).Setup(p => p.BindingConfiguration).Returns(_bindingConfigurationMock1.Object);
            _contextMock1.InSequence(_mockSequence).SetupGet(p => p.HasInferredGenericArguments).Returns(false);

            var actual = cache.TryGet(context, scope);

            Assert.NotNull(actual);
            Assert.Same(instance, actual);

            _contextMock1.Verify(p => p.Binding, Times.Once());
            _bindingMock1.Verify(p => p.BindingConfiguration, Times.Once());
        }

        [Fact]
        public void TryGet_ContextAndScope_ShouldReturnFirstRememberedInstanceForBindingConfigurationWhenMoreThanOneInstanceIsRememberedAndContextDoesNotHaveInferredGenericArguments()
        {
            var otherBindingConfiguration = new Mock<IBindingConfiguration>(MockBehavior.Strict).Object;
            var context = _contextMock1.Object;
            var scope = new object();
            var cache = CreateCache();
            var instance = new object();
            var instanceReference = new InstanceReference { Instance = instance };

            Remember(cache, _contextMock1, scope, otherBindingConfiguration, new InstanceReference { Instance = new object() });
            Remember(cache, _contextMock1, scope, _bindingConfigurationMock1.Object, instanceReference);
            Remember(cache, _contextMock1, scope, _bindingConfigurationMock1.Object, new InstanceReference { Instance = new object() });
            Remember(cache, _contextMock1, scope, otherBindingConfiguration, new InstanceReference { Instance = new object() });

            _contextMock1.InSequence(_mockSequence).Setup(p => p.Binding).Returns(_bindingMock1.Object);
            _bindingMock1.InSequence(_mockSequence).Setup(p => p.BindingConfiguration).Returns(_bindingConfigurationMock1.Object);
            _contextMock1.InSequence(_mockSequence).SetupGet(p => p.HasInferredGenericArguments).Returns(false);

            var actual = cache.TryGet(context, scope);

            Assert.NotNull(actual);
            Assert.Same(instance, actual);

            _contextMock1.Verify(p => p.Binding, Times.Once());
            _bindingMock1.Verify(p => p.BindingConfiguration, Times.Once());
        }

        [Fact]
        public void TryGet_ContextAndScope_ShouldReturnNullWhenContextHasInferredGenericArgumentsAndContextOfOnlyRememberedInstanceForBindingConfigurationHasNoGenericArguments()
        {
            var rememberedInstanceContextMock = new Mock<IContext>(MockBehavior.Strict);
            var rememberedInstanceContextGenericArguments = new Type[0];
            var context = _contextMock1.Object;
            var contextGenericArguments = new[] { typeof(string) };
            var scope = new object();
            var cache = CreateCache();
            var instance = new object();
            var instanceReference = new InstanceReference { Instance = instance };

            Remember(cache, rememberedInstanceContextMock, scope, _bindingConfigurationMock1.Object, instanceReference);

            _contextMock1.InSequence(_mockSequence).Setup(p => p.Binding).Returns(_bindingMock1.Object);
            _bindingMock1.InSequence(_mockSequence).Setup(p => p.BindingConfiguration).Returns(_bindingConfigurationMock1.Object);
            _contextMock1.InSequence(_mockSequence).SetupGet(p => p.HasInferredGenericArguments).Returns(true);
            rememberedInstanceContextMock.InSequence(_mockSequence).Setup(p => p.GenericArguments).Returns(rememberedInstanceContextGenericArguments);
            _contextMock1.InSequence(_mockSequence).Setup(p => p.GenericArguments).Returns(contextGenericArguments);

            var actual = cache.TryGet(context, scope);

            Assert.Null(actual);

            _contextMock1.Verify(p => p.Binding, Times.Once());
            _bindingMock1.Verify(p => p.BindingConfiguration, Times.Once());
            _contextMock1.Verify(p => p.HasInferredGenericArguments, Times.Once());
            rememberedInstanceContextMock.Verify(p => p.GenericArguments, Times.Once());
            _contextMock1.Verify(p => p.GenericArguments, Times.Once());
        }

        [Fact]
        public void TryGet_ContextAndScope_ShouldReturnFirstRememberInstanceWithMatchingGenericArgumentsWhenContextHasInferredGenericArguments()
        {
            var rememberedInstance1ContextMock = new Mock<IContext>(MockBehavior.Strict);
            var rememberedInstance1ContextGenericArguments = new[] { typeof(string), typeof(int) };
            var rememberedInstance2ContextMock = new Mock<IContext>(MockBehavior.Strict);
            var rememberedInstance2ContextGenericArguments = new[] { typeof(string), typeof(bool) };
            var context = _contextMock1.Object;
            var contextGenericArguments = new[] { typeof(string), typeof(bool) };
            var scope = new object();
            var cache = CreateCache();
            var instance = new object();
            var instanceReference = new InstanceReference { Instance = instance };

            Remember(cache, rememberedInstance1ContextMock, scope, _bindingConfigurationMock1.Object, new InstanceReference { Instance = new object()});
            Remember(cache, rememberedInstance2ContextMock, scope, _bindingConfigurationMock1.Object, instanceReference);

            _contextMock1.InSequence(_mockSequence).Setup(p => p.Binding).Returns(_bindingMock1.Object);
            _bindingMock1.InSequence(_mockSequence).Setup(p => p.BindingConfiguration).Returns(_bindingConfigurationMock1.Object);
            _contextMock1.InSequence(_mockSequence).SetupGet(p => p.HasInferredGenericArguments).Returns(true);
            rememberedInstance1ContextMock.InSequence(_mockSequence).Setup(p => p.GenericArguments).Returns(rememberedInstance1ContextGenericArguments);
            _contextMock1.InSequence(_mockSequence).Setup(p => p.GenericArguments).Returns(contextGenericArguments);
            _contextMock1.InSequence(_mockSequence).SetupGet(p => p.HasInferredGenericArguments).Returns(true);
            rememberedInstance2ContextMock.InSequence(_mockSequence).Setup(p => p.GenericArguments).Returns(rememberedInstance2ContextGenericArguments);
            _contextMock1.InSequence(_mockSequence).Setup(p => p.GenericArguments).Returns(contextGenericArguments);

            var actual = cache.TryGet(context, scope);

            Assert.NotNull(actual);
            Assert.Same(instance, actual);

            _contextMock1.Verify(p => p.Binding, Times.Once());
            _bindingMock1.Verify(p => p.BindingConfiguration, Times.Once());
            _contextMock1.Verify(p => p.HasInferredGenericArguments, Times.Exactly(2));
            rememberedInstance1ContextMock.Verify(p => p.GenericArguments, Times.Once());
            rememberedInstance2ContextMock.Verify(p => p.GenericArguments, Times.Once());
            _contextMock1.Verify(p => p.GenericArguments, Times.Exactly(2));
        }

        [Fact]
        public void Release_NullInstance_NoCacheEntriesFound()
        {
            var rememberedInstance1ContextMock = new Mock<IContext>(MockBehavior.Strict);
            var rememberedInstance2ContextMock = new Mock<IContext>(MockBehavior.Strict);
            var scope1 = new object();
            var scope2 = new object();
            var cache = CreateCache();
            const object instance = null;

            Remember(cache, rememberedInstance1ContextMock, scope1, _bindingConfigurationMock1.Object, new InstanceReference { Instance = new object() });
            Remember(cache, rememberedInstance2ContextMock, scope2, _bindingConfigurationMock1.Object, new InstanceReference { Instance = new object() });

            var actual = cache.Release(instance);

            Assert.False(actual);
        }

        [Fact]
        public void Release_NullInstance_CacheEntriesFound()
        {
            var contextMock1 = new Mock<IContext>(MockBehavior.Strict);
            var contextMock2 = new Mock<IContext>(MockBehavior.Strict);
            var contextMock3 = new Mock<IContext>(MockBehavior.Strict);
            var bindingMock1 = new Mock<IBinding>(MockBehavior.Strict);
            var bindingMock2 = new Mock<IBinding>(MockBehavior.Strict);
            var bindingConfigurationMock1 = new Mock<IBindingConfiguration>(MockBehavior.Strict);
            var bindingConfigurationMock2 = new Mock<IBindingConfiguration>(MockBehavior.Strict);

            var scope1 = new object();
            var scope2 = new object();
            var cache = CreateCache();
            const object nullInstance = null;
            var nullInstanceReference1 = new InstanceReference { Instance = nullInstance };
            var nullInstanceReference2 = new InstanceReference { Instance = nullInstance };
            var nullInstanceReference3 = new InstanceReference { Instance = nullInstance };
            var notNullInstanceReference1 = new InstanceReference { Instance = new object() };
            var notNullInstanceReference2 = new InstanceReference { Instance = new object() };

            contextMock1.Setup(p => p.Binding).Returns(bindingMock1.Object);
            contextMock2.Setup(p => p.Binding).Returns(bindingMock1.Object);
            contextMock3.Setup(p => p.Binding).Returns(bindingMock2.Object);
            bindingMock1.Setup(p => p.BindingConfiguration).Returns(bindingConfigurationMock1.Object);
            bindingMock2.Setup(p => p.BindingConfiguration).Returns(bindingConfigurationMock2.Object);

            cache.Remember(contextMock1.Object, scope1, notNullInstanceReference1);
            cache.Remember(contextMock2.Object, scope2, nullInstanceReference1);
            cache.Remember(contextMock1.Object, scope1, nullInstanceReference2);
            cache.Remember(contextMock3.Object, scope2, notNullInstanceReference2);
            cache.Remember(contextMock1.Object, scope2, nullInstanceReference3);

            _pipelineMock.Setup(p => p.Deactivate(contextMock1.Object, nullInstanceReference3));
            _pipelineMock.Setup(p => p.Deactivate(contextMock2.Object, nullInstanceReference1));
            _pipelineMock.Setup(p => p.Deactivate(contextMock1.Object, nullInstanceReference2));

            var actualException = Assert.Throws<ArgumentNullException>(() => cache.Release(nullInstance));

            Assert.Null(actualException.InnerException);
            Assert.Equal("key", actualException.ParamName);

            _pipelineMock.Verify(p => p.Deactivate(contextMock1.Object, nullInstanceReference3), Times.Once());
            _pipelineMock.Verify(p => p.Deactivate(contextMock2.Object, nullInstanceReference1), Times.Once());
            _pipelineMock.Verify(p => p.Deactivate(contextMock1.Object, nullInstanceReference2), Times.Once());

            Assert.Equal(2, cache.Count);

            contextMock1.Setup(p => p.HasInferredGenericArguments).Returns(false);
            contextMock3.Setup(p => p.HasInferredGenericArguments).Returns(false);

            Assert.Same(notNullInstanceReference1.Instance, cache.TryGet(contextMock1.Object, scope1));
            Assert.Same(notNullInstanceReference2.Instance, cache.TryGet(contextMock3.Object, scope2));
        }

        [Fact]
        public void Release_NotNullInstance_NoCacheEntriesOrScopeFound()
        {
            var scope1 = new object();
            var scope2 = new object();
            var cache = CreateCache();
            var notNullInstanceReference1 = new InstanceReference { Instance = new object() };
            var notNullInstanceReference2 = new InstanceReference { Instance = new object() };

            _contextMock1.Setup(p => p.Binding).Returns(_bindingMock1.Object);
            _bindingMock1.Setup(p => p.BindingConfiguration).Returns(_bindingConfigurationMock1.Object);

            cache.Remember(_contextMock1.Object, scope1, notNullInstanceReference1);
            cache.Remember(_contextMock1.Object, scope2, notNullInstanceReference2);

            var actual = cache.Release(new object());

            Assert.False(actual);

            Assert.Equal(2, cache.Count);

            _contextMock1.Setup(p => p.HasInferredGenericArguments).Returns(false);

            Assert.Same(notNullInstanceReference1.Instance, cache.TryGet(_contextMock1.Object, scope1));
            Assert.Same(notNullInstanceReference2.Instance, cache.TryGet(_contextMock1.Object, scope2));
        }

        [Fact]
        public void Release_NotNullInstance_OnlyCacheEntriesFound()
        {
            var scope1 = new object();
            var scope2 = new object();
            var cache = CreateCache();
            var notNullInstanceReference1 = new InstanceReference { Instance = new object() };
            var notNullInstanceReference2 = new InstanceReference { Instance = new object() };
            var notNullInstanceReference3 = new InstanceReference { Instance = new object() };

            _contextMock1.Setup(p => p.Binding).Returns(_bindingMock1.Object);
            _bindingMock1.Setup(p => p.BindingConfiguration).Returns(_bindingConfigurationMock1.Object);
            _contextMock2.Setup(p => p.Binding).Returns(_bindingMock2.Object);
            _bindingMock2.Setup(p => p.BindingConfiguration).Returns(_bindingConfigurationMock2.Object);

            cache.Remember(_contextMock1.Object, scope1, notNullInstanceReference1);
            cache.Remember(_contextMock1.Object, scope2, notNullInstanceReference2);
            cache.Remember(_contextMock2.Object, scope1, notNullInstanceReference1);
            cache.Remember(_contextMock2.Object, scope2, notNullInstanceReference3);

            Assert.Equal(4, cache.Count);

            _pipelineMock.Setup(p => p.Deactivate(_contextMock1.Object, notNullInstanceReference1));
            _pipelineMock.Setup(p => p.Deactivate(_contextMock2.Object, notNullInstanceReference1));

            var actual = cache.Release(notNullInstanceReference1.Instance);

            Assert.True(actual);

            _pipelineMock.Verify(p => p.Deactivate(_contextMock1.Object, notNullInstanceReference1), Times.Once());
            _pipelineMock.Verify(p => p.Deactivate(_contextMock2.Object, notNullInstanceReference1), Times.Once());

            Assert.Equal(2, cache.Count);

            _contextMock1.Setup(p => p.HasInferredGenericArguments).Returns(false);
            _contextMock2.Setup(p => p.HasInferredGenericArguments).Returns(false);

            Assert.Same(notNullInstanceReference2.Instance, cache.TryGet(_contextMock1.Object, scope2));
            Assert.Same(notNullInstanceReference3.Instance, cache.TryGet(_contextMock2.Object, scope2));
        }

        [Fact]
        public void Release_NotNullInstance_OnlyScopeFound()
        {
            var scope1 = new object();
            var scope2 = new object();
            var cache = CreateCache();
            var notNullInstanceReference1 = new InstanceReference { Instance = new object() };
            var notNullInstanceReference2 = new InstanceReference { Instance = new object() };
            var notNullInstanceReference3 = new InstanceReference { Instance = new object() };

            _contextMock1.Setup(p => p.Binding).Returns(_bindingMock1.Object);
            _bindingMock1.Setup(p => p.BindingConfiguration).Returns(_bindingConfigurationMock1.Object);
            _contextMock2.Setup(p => p.Binding).Returns(_bindingMock2.Object);
            _bindingMock2.Setup(p => p.BindingConfiguration).Returns(_bindingConfigurationMock2.Object);

            cache.Remember(_contextMock1.Object, scope1, notNullInstanceReference1);
            cache.Remember(_contextMock1.Object, scope2, notNullInstanceReference2);

            Assert.Equal(2, cache.Count);

            var actual = cache.Release(scope2);

            Assert.False(actual);

            Assert.Equal(2, cache.Count);

            _contextMock1.Setup(p => p.HasInferredGenericArguments).Returns(false);
            _contextMock2.Setup(p => p.HasInferredGenericArguments).Returns(false);

            Assert.Same(notNullInstanceReference1.Instance, cache.TryGet(_contextMock1.Object, scope1));
            Assert.Same(notNullInstanceReference2.Instance, cache.TryGet(_contextMock1.Object, scope2));
        }

        /// <summary>
        /// scope 1
        ///     - reference 1
        ///     - reference 2 => scope 2
        /// scope 2
        ///     - reference 3
        ///     - reference 4 => scope 4
        /// scope 3
        ///     - reference 5
        /// scope 4
        ///     - reference 6
        ///     
        /// Removing the instance in reference 2 should remove scope 2 and scope 4.
        /// </summary>
        [Fact]
        public void Release_NotNullInstance_CacheEntriesAndScopeFound()
        {
            var scope1 = new object();
            var scope2 = new object();
            var scope3 = new object();
            var scope4 = new object();
            var cache = CreateCache();
            var notNullInstanceReference1 = new InstanceReference { Instance = new object() };
            var notNullInstanceReference2 = new InstanceReference { Instance = scope2 };
            var notNullInstanceReference3 = new InstanceReference { Instance = new object() };
            var notNullInstanceReference4 = new InstanceReference { Instance = scope4 };
            var notNullInstanceReference5 = new InstanceReference { Instance = new object() };
            var notNullInstanceReference6 = new InstanceReference { Instance = new object() };

            _contextMock1.Setup(p => p.Binding).Returns(_bindingMock1.Object);
            _bindingMock1.Setup(p => p.BindingConfiguration).Returns(_bindingConfigurationMock1.Object);
            _contextMock2.Setup(p => p.Binding).Returns(_bindingMock2.Object);
            _bindingMock2.Setup(p => p.BindingConfiguration).Returns(_bindingConfigurationMock2.Object);

            cache.Remember(_contextMock1.Object, scope1, notNullInstanceReference1);
            cache.Remember(_contextMock1.Object, scope1, notNullInstanceReference2);
            cache.Remember(_contextMock2.Object, scope2, notNullInstanceReference3);
            cache.Remember(_contextMock1.Object, scope2, notNullInstanceReference4);
            cache.Remember(_contextMock2.Object, scope3, notNullInstanceReference5);
            cache.Remember(_contextMock2.Object, scope4, notNullInstanceReference6);

            Assert.Equal(6, cache.Count);

            _pipelineMock.Setup(p => p.Deactivate(_contextMock1.Object, notNullInstanceReference2));
            _pipelineMock.Setup(p => p.Deactivate(_contextMock2.Object, notNullInstanceReference3));
            _pipelineMock.Setup(p => p.Deactivate(_contextMock1.Object, notNullInstanceReference4));
            _pipelineMock.Setup(p => p.Deactivate(_contextMock2.Object, notNullInstanceReference6));

            var actual = cache.Release(scope2);

            Assert.True(actual);

            _pipelineMock.Verify(p => p.Deactivate(_contextMock1.Object, notNullInstanceReference2), Times.Once());
            _pipelineMock.Verify(p => p.Deactivate(_contextMock2.Object, notNullInstanceReference3), Times.Once());
            _pipelineMock.Verify(p => p.Deactivate(_contextMock1.Object, notNullInstanceReference4), Times.Once());
            _pipelineMock.Verify(p => p.Deactivate(_contextMock2.Object, notNullInstanceReference6), Times.Once());

            Assert.Equal(2, cache.Count);

            _contextMock1.Setup(p => p.HasInferredGenericArguments).Returns(false);
            _contextMock2.Setup(p => p.HasInferredGenericArguments).Returns(false);

            Assert.Same(notNullInstanceReference1.Instance, cache.TryGet(_contextMock1.Object, scope1));
            Assert.Same(notNullInstanceReference5.Instance, cache.TryGet(_contextMock2.Object, scope3));
        }

        [Fact]
        public void Concurrency()
        {
            var cache = CreateCache();
            var scope = new object();
            var instance = new object();

            _contextMock1.Setup(p => p.Binding).Returns(_bindingMock1.Object);
            _bindingMock1.Setup(p => p.BindingConfiguration).Returns(_bindingConfigurationMock1.Object);
            _contextMock1.Setup(p => p.HasInferredGenericArguments).Returns(false);

            var tryGetTask = new Task(() =>
            {
                for (var i = 0; i < 5000; i++)
                {
                    cache.TryGet(_contextMock1.Object, scope);
                }
            }, TaskCreationOptions.LongRunning);

            var rememberTask = new Task(() =>
            {
                var reference = new InstanceReference { Instance = instance };

                for (var i = 0; i < 5000; i++)
                {
                    cache.Remember(_contextMock1.Object, scope, reference);
                }
            }, TaskCreationOptions.LongRunning);

            var releaseTask = new Task(() =>
            {
                for (var i = 0; i < 5000; i++)
                {
                    cache.Release(instance);
                }
            }, TaskCreationOptions.LongRunning);

            var clearScopeTask = new Task(() =>
            {
                for (var i = 0; i < 5000; i++)
                {
                    cache.Clear(scope);
                }
            }, TaskCreationOptions.LongRunning);

            var clearTask = new Task(() =>
            {
                for (var i = 0; i < 5000; i++)
                {
                    cache.Clear();
                }
            }, TaskCreationOptions.LongRunning);

            clearTask.Start();
            clearScopeTask.Start();
            rememberTask.Start();
            releaseTask.Start();
            tryGetTask.Start();

            clearTask.ConfigureAwait(false).GetAwaiter().GetResult();
            clearScopeTask.ConfigureAwait(false).GetAwaiter().GetResult();
            rememberTask.ConfigureAwait(false).GetAwaiter().GetResult();
            releaseTask.ConfigureAwait(false).GetAwaiter().GetResult();
            tryGetTask.ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private Cache CreateCache()
        {
            _cachePrunerMock.Setup(p => p.Start(It.IsNotNull<IPruneable>()));

            return new Cache(_pipelineMock.Object, _cachePrunerMock.Object);
        }

        private void Remember(Cache cache, Mock<IContext> contextMock, object scope, IBindingConfiguration bindingConfiguration, InstanceReference instanceReference)
        {
            contextMock.InSequence(_mockSequence).Setup(p => p.Binding).Returns(_bindingMock1.Object);
            _bindingMock1.InSequence(_mockSequence).Setup(p => p.BindingConfiguration).Returns(bindingConfiguration);

            cache.Remember(contextMock.Object, scope, instanceReference);

            contextMock.Reset();
            _bindingMock1.Reset();
        }

        private WeakReference Remember(Cache cache, Mock<IContext> contextMock, IBindingConfiguration bindingConfiguration, InstanceReference instanceReference)
        {
            var scope = new object();

            contextMock.InSequence(_mockSequence).Setup(p => p.Binding).Returns(_bindingMock1.Object);
            _bindingMock1.InSequence(_mockSequence).Setup(p => p.BindingConfiguration).Returns(bindingConfiguration);

            cache.Remember(contextMock.Object, scope, instanceReference);

            contextMock.Reset();
            _bindingMock1.Reset();

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