#if !NO_MOQ
namespace Ninject.Tests.Unit.CacheTests
{
    using System;
    using FluentAssertions;
    using Moq;
    using Ninject.Activation;
    using Ninject.Activation.Caching;
    using Ninject.Infrastructure.Disposal;
    using Ninject.Planning.Bindings;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class CacheContext
    {
        protected Mock<ICachePruner> cachePrunerMock;
        protected IBindingConfiguration bindingConfiguration;
        protected Cache cache;
        protected Mock<IPipeline> pipelineMock;

        public CacheContext()
        {
            this.cachePrunerMock = new Mock<ICachePruner>();
            this.bindingConfiguration = new Mock<IBindingConfiguration>().Object;
            this.pipelineMock = new Mock<IPipeline>();
            this.cache = new Cache(this.pipelineMock.Object, this.cachePrunerMock.Object);
        }

        protected static IContext CreateContext(object scope, IBindingConfiguration bindingConfiguration, params Type[] genericArguments)
        {
            var contextMock = new Mock<IContext>();
            var bindingMock = new Mock<IBinding>();
            bindingMock.SetupGet(binding => binding.BindingConfiguration).Returns(bindingConfiguration);
            contextMock.SetupGet(context => context.Binding).Returns(bindingMock.Object);
            contextMock.Setup(context => context.GetScope()).Returns(scope);
            contextMock.SetupGet(context => context.GenericArguments).Returns(genericArguments);
            contextMock.SetupGet(context => context.HasInferredGenericArguments).Returns(genericArguments != null && genericArguments.Length > 0);
            return contextMock.Object;
        }
    }

    public class WhenTryGetInstanceIsCalled : CacheContext
    {
        [Fact]
        public void ReturnsNullIfNoInstancesHaveBeenAddedToCache()
        {
            var scope = new TestObject(42);
            var context = CreateContext(scope, this.bindingConfiguration);

            var instance = cache.TryGet(context);

            instance.Should().BeNull();
        }

        [Fact]
        public void ReturnsCachedInstanceIfOneHasBeenAddedWithinSpecifiedScope()
        {
            var scope = new TestObject(42);
            var reference = new InstanceReference { Instance = new Sword() };
            var context1 = CreateContext(scope, this.bindingConfiguration);
            var context2 = CreateContext(scope, this.bindingConfiguration);

            cache.Remember(context1, reference);
            object instance = cache.TryGet(context2);

            instance.Should().BeSameAs(reference.Instance);
        }

        [Fact]
        public void ReturnsNullIfNoInstancesHaveBeenAddedWithinSpecifiedScope()
        {
            var reference = new InstanceReference { Instance = new Sword() };
            var context1 = CreateContext(new TestObject(42), this.bindingConfiguration);
            var context2 = CreateContext(new TestObject(42), this.bindingConfiguration);

            cache.Remember(context1, reference);
            object instance = cache.TryGet(context2);

            instance.Should().BeNull();
        }

        [Fact]
        public void ReturnsNullIfScopeIsNull()
        {
            var reference = new InstanceReference { Instance = new Sword() };
            var context1 = CreateContext(new TestObject(42), this.bindingConfiguration);
            var context2 = CreateContext(null, this.bindingConfiguration);

            cache.Remember(context1, reference);
            object instance = cache.TryGet(context2);

            instance.Should().BeNull();
        }
    }

    public class WhenTryGetInstanceIsCalledForContextWithGenericInference : CacheContext
    {
        [Fact]
        public void ReturnsInstanceIfOneHasBeenCachedWithSameGenericParameters()
        {
            var scope = new TestObject(42);
            var reference = new InstanceReference { Instance = new Sword() };
            var context1 = CreateContext(scope, this.bindingConfiguration, typeof(int));
            var context2 = CreateContext(scope, this.bindingConfiguration, typeof(int));

            cache.Remember(context1, reference);
            object instance = cache.TryGet(context2);

            instance.Should().BeSameAs(reference.Instance);
        }

        [Fact]
        public void ReturnsNullIfInstanceAddedToCacheHasDifferentGenericParameters()
        {
            var scope = new TestObject(42);
            var reference = new InstanceReference { Instance = new Sword() };
            var context1 = CreateContext(scope, this.bindingConfiguration, typeof(int));
            var context2 = CreateContext(scope, this.bindingConfiguration, typeof(double));

            cache.Remember(context1, reference);
            object instance = cache.TryGet(context2);

            instance.Should().BeNull();
        }
    }

    public class WhenReleaseIsCalled : CacheContext
    {
        [Fact]
        public void ReturnsFalseIfInstanceIsNotTracked()
        {
            bool result = cache.Release(new TestObject(42));
            result.Should().BeFalse();
        }

        [Fact]
        public void ReturnsTrueIfInstanceIsTracked()
        {
            var scope = new TestObject(42);
            var instance = new Sword();
            var reference = new InstanceReference { Instance = instance };
            var writeContext = CreateContext(scope, this.bindingConfiguration, typeof(int));

            cache.Remember(writeContext, reference);
            bool result = cache.Release(instance);

            result.Should().BeTrue();
        }

        [Fact]
        public void InstanceIsRemovedFromCache()
        {
            var scope = new TestObject(42);
            var sword = new Sword();
            var reference = new InstanceReference { Instance = sword };
            var writeContext = CreateContext(scope, this.bindingConfiguration, typeof(int));
            var readContext = CreateContext(scope, this.bindingConfiguration, typeof(int));

            cache.Remember(writeContext, reference);
            object instance1 = cache.TryGet(readContext);
            bool result = cache.Release(instance1);
            object instance2 = cache.TryGet(readContext);

            instance1.Should().BeSameAs(reference.Instance);
            result.Should().BeTrue();
            instance2.Should().BeNull();
        }
    }

    public class WhenClearIsCalled : CacheContext
    {
        [Fact]
        public void WhenScopeIsDefinedItsEntriesAreReleased()
        {
            var scope = new TestObject(42);
            var sword = new Sword();
            var reference = new InstanceReference { Instance = sword };
            var context1 = CreateContext(scope, this.bindingConfiguration);
            var context2 = CreateContext(new TestObject(42), this.bindingConfiguration);

            cache.Remember(context1, reference);
            cache.Remember(context2, reference);
            cache.Clear(scope);
            var instance1 = cache.TryGet(context1);
            var instance2 = cache.TryGet(context2);

            instance1.Should().BeNull();
            instance2.Should().NotBeNull();
        }
        
        [Fact]
        public void WhenNoScopeIsDefinedAllEntriesAreReleased()
         {
            var sword = new Sword();
            var reference = new InstanceReference { Instance = sword };
            var context1 = CreateContext(new TestObject(42), this.bindingConfiguration);
            var context2 = CreateContext(new TestObject(42), this.bindingConfiguration);

            cache.Remember(context1, reference);
            cache.Remember(context2, reference);
            cache.Clear();
            var instance1 = cache.TryGet(context1);
            var instance2 = cache.TryGet(context2);

            instance1.Should().BeNull();
            instance2.Should().BeNull();
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
            var context = CreateContext(scopeMock.Object, this.bindingConfiguration);

            cache.Remember(context, reference);
            scopeMock.Raise(scope => scope.Disposed += null, EventArgs.Empty);
            object instance = cache.TryGet(context);

            instance.Should().BeNull();

        }
    }

    public class WhenScopeIsReleasedFormCache : CacheContext
    {
        [Fact]
        public void CachedObjectsAreReleased()
        {
            var scope = new TestObject(42);
            var scopeOfScope = new TestObject(42);
            var sword = new Sword();
            var context = CreateContext(scope, this.bindingConfiguration);

            cache.Remember(context, new InstanceReference { Instance = sword });
            cache.Remember(CreateContext(scopeOfScope, this.bindingConfiguration), new InstanceReference { Instance = scope });
            cache.Clear(scopeOfScope);
            var instance = cache.TryGet(context);

            instance.Should().BeNull();
        }
    }
}
#endif