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
        protected Mock<IBindingConfiguration> bindingConfigurationMock;
        protected Cache cache;
        protected Mock<IPipeline> pipelineMock;

        public CacheContext()
        {
            this.cachePrunerMock = new Mock<ICachePruner>();
            this.bindingConfigurationMock = new Mock<IBindingConfiguration>();
            this.pipelineMock = new Mock<IPipeline>();
            this.cache = new Cache(this.pipelineMock.Object, this.cachePrunerMock.Object);
        }

        protected static Mock<IContext> CreateContextMock(object scope, IBindingConfiguration bindingConfiguration, params Type[] genericArguments)
        {
            var contextMock = new Mock<IContext>();
            var bindingMock = new Mock<IBinding>();
            bindingMock.SetupGet(binding => binding.BindingConfiguration).Returns(bindingConfiguration);
            contextMock.SetupGet(context => context.Binding).Returns(bindingMock.Object);
            contextMock.Setup(context => context.GetScope()).Returns(scope);
            contextMock.SetupGet(context => context.GenericArguments).Returns(genericArguments);
            contextMock.SetupGet(context => context.HasInferredGenericArguments).Returns(genericArguments != null && genericArguments.Length > 0);
            return contextMock;
        }
    }

    public class WhenTryGetInstanceIsCalled : CacheContext
    {
#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void ReturnsNullIfNoInstancesHaveBeenAddedToCache()
        {
            var scope = new object();
            var contextMock = CreateContextMock(scope, this.bindingConfigurationMock.Object);

            var instance = cache.TryGet(contextMock.Object);

            instance.Should().BeNull();
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void ReturnsCachedInstanceIfOneHasBeenAddedWithinSpecifiedScope()
        {
            var scope = new object();
            var reference = new InstanceReference { Instance = new Sword() };
            var contextMock1 = CreateContextMock(scope, this.bindingConfigurationMock.Object);
            var contextMock2 = CreateContextMock(scope, this.bindingConfigurationMock.Object);

            cache.Remember(contextMock1.Object, reference);
            object instance = cache.TryGet(contextMock2.Object);

            instance.Should().BeSameAs(reference.Instance);
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void ReturnsNullIfNoInstancesHaveBeenAddedWithinSpecifiedScope()
        {
            var reference = new InstanceReference { Instance = new Sword() };
            var contextMock1 = CreateContextMock(new object(), this.bindingConfigurationMock.Object);
            var contextMock2 = CreateContextMock(new object(), this.bindingConfigurationMock.Object);

            cache.Remember(contextMock1.Object, reference);
            object instance = cache.TryGet(contextMock2.Object);

            instance.Should().BeNull();
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void ReturnsNullIfScopeIsNull()
        {
            var reference = new InstanceReference { Instance = new Sword() };
            var contextMock1 = CreateContextMock(new object(), this.bindingConfigurationMock.Object);
            var contextMock2 = CreateContextMock(null, this.bindingConfigurationMock.Object);

            cache.Remember(contextMock1.Object, reference);
            object instance = cache.TryGet(contextMock2.Object);

            instance.Should().BeNull();
        }
    }

    public class WhenTryGetInstanceIsCalledForContextWithGenericInference : CacheContext
    {
#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void ReturnsInstanceIfOneHasBeenCachedWithSameGenericParameters()
        {
            var scope = new object();
            var reference = new InstanceReference { Instance = new Sword() };
            var contextMock1 = CreateContextMock(scope, this.bindingConfigurationMock.Object, typeof(int));
            var contextMock2 = CreateContextMock(scope, this.bindingConfigurationMock.Object, typeof(int));

            cache.Remember(contextMock1.Object, reference);
            object instance = cache.TryGet(contextMock2.Object);

            instance.Should().BeSameAs(reference.Instance);
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void ReturnsNullIfInstanceAddedToCacheHasDifferentGenericParameters()
        {
            var scope = new object();
            var reference = new InstanceReference { Instance = new Sword() };
            var contextMock1 = CreateContextMock(scope, this.bindingConfigurationMock.Object, typeof(int));
            var contextMock2 = CreateContextMock(scope, this.bindingConfigurationMock.Object, typeof(double));

            cache.Remember(contextMock1.Object, reference);
            object instance = cache.TryGet(contextMock2.Object);

            instance.Should().BeNull();
        }
    }

    public class WhenReleaseIsCalled : CacheContext
    {
#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void ReturnsFalseIfInstanceIsNotTracked()
        {
            bool result = cache.Release(new object());
            result.Should().BeFalse();
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void ReturnsTrueIfInstanceIsTracked()
        {
            var scope = new object();
            var instance = new Sword();
            var reference = new InstanceReference { Instance = instance };
            var writeContext = CreateContextMock(scope, this.bindingConfigurationMock.Object, typeof(int));

            cache.Remember(writeContext.Object, reference);
            bool result = cache.Release(instance);

            result.Should().BeTrue();
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void InstanceIsRemovedFromCache()
        {
            var scope = new object();
            var sword = new Sword();
            var reference = new InstanceReference { Instance = sword };
            var writeContext = CreateContextMock(scope, this.bindingConfigurationMock.Object, typeof(int));
            var readContext = CreateContextMock(scope, this.bindingConfigurationMock.Object, typeof(int));

            cache.Remember(writeContext.Object, reference);
            object instance1 = cache.TryGet(readContext.Object);
            bool result = cache.Release(instance1);
            object instance2 = cache.TryGet(readContext.Object);

            instance1.Should().BeSameAs(reference.Instance);
            result.Should().BeTrue();
            instance2.Should().BeNull();
        }
    }

    public class WhenClearIsCalled : CacheContext
    {
#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void WhenScopeIsDefinedItsEntriesAreReleased()
        {
            var scope = new object();
            var sword = new Sword();
            var reference = new InstanceReference { Instance = sword };
            var context1 = CreateContextMock(scope, this.bindingConfigurationMock.Object);
            var context2 = CreateContextMock(new object(), this.bindingConfigurationMock.Object);

            cache.Remember(context1.Object, reference);
            cache.Remember(context2.Object, reference);
            cache.Clear(scope);
            var instance1 = cache.TryGet(context1.Object);
            var instance2 = cache.TryGet(context2.Object);

            instance1.Should().BeNull();
            instance2.Should().NotBeNull();
        }
        
#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void WhenNoScopeIsDefinedAllEntriesAreReleased()
         {
            var sword = new Sword();
            var reference = new InstanceReference { Instance = sword };
            var context1 = CreateContextMock(new object(), this.bindingConfigurationMock.Object);
            var context2 = CreateContextMock(new object(), this.bindingConfigurationMock.Object);

            cache.Remember(context1.Object, reference);
            cache.Remember(context2.Object, reference);
            cache.Clear();
            var instance1 = cache.TryGet(context1.Object);
            var instance2 = cache.TryGet(context2.Object);

            instance1.Should().BeNull();
            instance2.Should().BeNull();
         }
    }

    public class WhenNotifiesWhenDisposedScopeIsDisposed : CacheContext
    {
#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void CachedObjectsAreReleased()
        {
            var scopeMock = new Mock<INotifyWhenDisposed>();
            var sword = new Sword();
            var reference = new InstanceReference { Instance = sword };
            var context = CreateContextMock(scopeMock.Object, this.bindingConfigurationMock.Object);

            cache.Remember(context.Object, reference);
            scopeMock.Raise(scope => scope.Disposed += null, EventArgs.Empty);
            object instance = cache.TryGet(context.Object);

            instance.Should().BeNull();

        }
    }

    public class WhenScopeIsReleasedFormCache : CacheContext
    {
#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void CachedObjectsAreReleased()
        {
            var scope = new object();
            var scopeOfScope = new object();
            var sword = new Sword();
            var context = CreateContextMock(scope, this.bindingConfigurationMock.Object);

            cache.Remember(context.Object, new InstanceReference { Instance = sword });
            cache.Remember(CreateContextMock(scopeOfScope, this.bindingConfigurationMock.Object).Object, new InstanceReference { Instance = scope });
            cache.Clear(scopeOfScope);
            var instance = cache.TryGet(context.Object);

            instance.Should().BeNull();
        }
    }
}
#endif