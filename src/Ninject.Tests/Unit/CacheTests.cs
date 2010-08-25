namespace Ninject.Tests.Unit.CacheTests
{
    using System;
    using Moq;
    using Ninject.Activation;
    using Ninject.Activation.Caching;
    using Ninject.Infrastructure.Disposal;
    using Ninject.Planning.Bindings;
    using Ninject.Tests.Fakes;
    using Xunit;
    using Xunit.Should;

    public class CacheContext
    {
        protected Mock<ICachePruner> cachePrunerMock;
        protected Mock<IBinding> bindingMock;
        protected Cache cache;
        protected Mock<IPipeline> pipelineMock;

        public CacheContext()
        {
            this.cachePrunerMock = new Mock<ICachePruner>();
            this.bindingMock = new Mock<IBinding>();
            this.pipelineMock = new Mock<IPipeline>();
            this.cache = new Cache(this.pipelineMock.Object, this.cachePrunerMock.Object);
        }

        protected static Mock<IContext> CreateContextMock(object scope, IBinding binding, params Type[] genericArguments)
        {
            var contextMock = new Mock<IContext>();
            contextMock.SetupGet(context => context.Binding).Returns(binding);
            contextMock.Setup(context => context.GetScope()).Returns(scope);
            contextMock.SetupGet(context => context.GenericArguments).Returns(genericArguments);
            contextMock.SetupGet(context => context.HasInferredGenericArguments).Returns(genericArguments != null && genericArguments.Length > 0);
            return contextMock;
        }
    }

    public class WhenTryGetInstanceIsCalled : CacheContext
    {
        [Fact]
        public void ReturnsNullIfNoInstancesHaveBeenAddedToCache()
        {
            var scope = new object();
            var contextMock = CreateContextMock(scope, bindingMock.Object);

            var instance = cache.TryGet(contextMock.Object);

            instance.ShouldBeNull();
        }

        [Fact]
        public void ReturnsCachedInstanceIfOneHasBeenAddedWithinSpecifiedScope()
        {
            var scope = new object();
            var reference = new InstanceReference { Instance = new Sword() };
            var contextMock1 = CreateContextMock(scope, bindingMock.Object);
            var contextMock2 = CreateContextMock(scope, bindingMock.Object);

            cache.Remember(contextMock1.Object, reference);
            object instance = cache.TryGet(contextMock2.Object);

            instance.ShouldBeSameAs(reference.Instance);
        }

        [Fact]
        public void ReturnsNullIfNoInstancesHaveBeenAddedWithinSpecifiedScope()
        {
            var reference = new InstanceReference { Instance = new Sword() };
            var contextMock1 = CreateContextMock(new object(), bindingMock.Object);
            var contextMock2 = CreateContextMock(new object(), bindingMock.Object);

            cache.Remember(contextMock1.Object, reference);
            object instance = cache.TryGet(contextMock2.Object);

            instance.ShouldBeNull();
        }

        [Fact]
        public void ReturnsNullIfScopeIsNull()
        {
            var reference = new InstanceReference { Instance = new Sword() };
            var contextMock1 = CreateContextMock(new object(), bindingMock.Object);
            var contextMock2 = CreateContextMock(null, bindingMock.Object);

            cache.Remember(contextMock1.Object, reference);
            object instance = cache.TryGet(contextMock2.Object);

            instance.ShouldBeNull();
        }
    }

    public class WhenTryGetInstanceIsCalledForContextWithGenericInference : CacheContext
    {
        [Fact]
        public void ReturnsInstanceIfOneHasBeenCachedWithSameGenericParameters()
        {
            var scope = new object();
            var reference = new InstanceReference { Instance = new Sword() };
            var contextMock1 = CreateContextMock(scope, bindingMock.Object, typeof(int));
            var contextMock2 = CreateContextMock(scope, bindingMock.Object, typeof(int));

            cache.Remember(contextMock1.Object, reference);
            object instance = cache.TryGet(contextMock2.Object);

            instance.ShouldBeSameAs(reference.Instance);
        }

        [Fact]
        public void ReturnsNullIfInstanceAddedToCacheHasDifferentGenericParameters()
        {
            var scope = new object();
            var reference = new InstanceReference { Instance = new Sword() };
            var contextMock1 = CreateContextMock(scope, bindingMock.Object, typeof(int));
            var contextMock2 = CreateContextMock(scope, bindingMock.Object, typeof(double));

            cache.Remember(contextMock1.Object, reference);
            object instance = cache.TryGet(contextMock2.Object);

            instance.ShouldBeNull();
        }
    }

    public class WhenReleaseIsCalled : CacheContext
    {
        [Fact]
        public void ReturnsFalseIfInstanceIsNotTracked()
        {
            bool result = cache.Release(new object());
            result.ShouldBeFalse();
        }

        [Fact]
        public void ReturnsTrueIfInstanceIsTracked()
        {
            var scope = new object();
            var instance = new Sword();
            var reference = new InstanceReference { Instance = instance };
            var writeContext = CreateContextMock(scope, bindingMock.Object, typeof(int));

            cache.Remember(writeContext.Object, reference);
            bool result = cache.Release(instance);

            result.ShouldBeTrue();
        }

        [Fact]
        public void InstanceIsRemovedFromCache()
        {
            var scope = new object();
            var sword = new Sword();
            var reference = new InstanceReference { Instance = sword };
            var writeContext = CreateContextMock(scope, bindingMock.Object, typeof(int));
            var readContext = CreateContextMock(scope, bindingMock.Object, typeof(int));

            cache.Remember(writeContext.Object, reference);
            object instance1 = cache.TryGet(readContext.Object);
            bool result = cache.Release(instance1);
            object instance2 = cache.TryGet(readContext.Object);

            instance1.ShouldBeSameAs(reference.Instance);
            result.ShouldBeTrue();
            instance2.ShouldBeNull();
        }
    }

    public class WhenClearIsCalled : CacheContext
    {
        [Fact]
        public void WhenScopeIsDefinedItsEntriesAreReleased()
        {
            var scope = new object();
            var sword = new Sword();
            var reference = new InstanceReference { Instance = sword };
            var context1 = CreateContextMock(scope, bindingMock.Object);
            var context2 = CreateContextMock(new object(), bindingMock.Object);

            cache.Remember(context1.Object, reference);
            cache.Remember(context2.Object, reference);
            cache.Clear(scope);
            var instance1 = cache.TryGet(context1.Object);
            var instance2 = cache.TryGet(context2.Object);

            instance1.ShouldBeNull();
            instance2.ShouldNotBeNull();
        }
        
        [Fact]
        public void WhenNoScopeIsDefinedAllEntriesAreReleased()
         {
            var sword = new Sword();
            var reference = new InstanceReference { Instance = sword };
            var context1 = CreateContextMock(new object(), bindingMock.Object);
            var context2 = CreateContextMock(new object(), bindingMock.Object);

            cache.Remember(context1.Object, reference);
            cache.Remember(context2.Object, reference);
            cache.Clear();
            var instance1 = cache.TryGet(context1.Object);
            var instance2 = cache.TryGet(context2.Object);

            instance1.ShouldBeNull();
            instance2.ShouldBeNull();
         }
    }

    public class WhenNotifiesWhenDisposedScopeIsDisposed : CacheContext
    {
        [Fact]
        public void CachedObjectsAreReleased()
        {
            var scopeMock = new Mock<INotifyWhenDisposed>();
            var sword = new Sword();
            var reference = new InstanceReference { Instance = sword };
            var context = CreateContextMock(scopeMock.Object, bindingMock.Object);

            cache.Remember(context.Object, reference);
            scopeMock.Raise(scope => scope.Disposed += null, EventArgs.Empty);
            object instance = cache.TryGet(context.Object);

            instance.ShouldBeNull();

        }
    }

    public class WhenScopeIsReleasedFormCache : CacheContext
    {
        [Fact]
        public void CachedObjectsAreReleased()
        {
            var scope = new object();
            var scopeOfScope = new object();
            var sword = new Sword();
            var context = CreateContextMock(scope, bindingMock.Object);

            cache.Remember(context.Object, new InstanceReference { Instance = sword });
            cache.Remember(CreateContextMock(scopeOfScope, bindingMock.Object).Object, new InstanceReference { Instance = scope });
            cache.Clear(scopeOfScope);
            var instance = cache.TryGet(context.Object);

            instance.ShouldBeNull();
        }
    }
}