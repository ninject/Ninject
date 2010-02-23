using System;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Planning.Bindings;
using Ninject.Tests.Fakes;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Unit.CacheTests
{
	public class CacheContext
	{
		protected Mock<IPipeline> activatorMock;
		protected Mock<ICachePruner> cachePrunerMock;
		protected Mock<IBinding> bindingMock;
		protected Cache cache;

		public CacheContext()
		{
			activatorMock = new Mock<IPipeline>();
			cachePrunerMock = new Mock<ICachePruner>();
			bindingMock = new Mock<IBinding>();
			cache = new Cache(activatorMock.Object, cachePrunerMock.Object);
		}
	}

	public class WhenTryGetInstanceIsCalled : CacheContext
	{
		[Fact]
		public void ReturnsNullIfNoInstancesHaveBeenAddedToCache()
		{
			var scope = new object();

			var contextMock = new Mock<IContext>();
			contextMock.SetupGet(x => x.Binding).Returns(bindingMock.Object);
			contextMock.Setup(x => x.GetScope()).Returns(scope);

			object instance = cache.TryGet(contextMock.Object);

			instance.ShouldBeNull();
		}

		[Fact]
		public void ReturnsCachedInstanceIfOneHasBeenAddedWithinSpecifiedScope()
		{
			var scope = new object();
			var reference = new InstanceReference { Instance = new Sword() };

			var contextMock1 = new Mock<IContext>();
			contextMock1.SetupGet(x => x.Binding).Returns(bindingMock.Object);
			contextMock1.Setup(x => x.GetScope()).Returns(scope);

			cache.Remember(contextMock1.Object, reference);

			var contextMock2 = new Mock<IContext>();
			contextMock2.SetupGet(x => x.Binding).Returns(bindingMock.Object);
			contextMock2.Setup(x => x.GetScope()).Returns(scope);

			object instance = cache.TryGet(contextMock2.Object);

			instance.ShouldBeSameAs(reference.Instance);
		}

		[Fact]
		public void ReturnsNullIfNoInstancesHaveBeenAddedWithinSpecifiedScope()
		{
			var reference = new InstanceReference { Instance = new Sword() };

			var contextMock1 = new Mock<IContext>();
			contextMock1.SetupGet(x => x.Binding).Returns(bindingMock.Object);
			contextMock1.Setup(x => x.GetScope()).Returns(new object());

			cache.Remember(contextMock1.Object, reference);

			var contextMock2 = new Mock<IContext>();
			contextMock2.SetupGet(x => x.Binding).Returns(bindingMock.Object);
			contextMock2.Setup(x => x.GetScope()).Returns(new object());

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

			var contextMock1 = new Mock<IContext>();
			contextMock1.SetupGet(x => x.Binding).Returns(bindingMock.Object);
			contextMock1.SetupGet(x => x.HasInferredGenericArguments).Returns(true);
			contextMock1.SetupGet(x => x.GenericArguments).Returns(new[] { typeof(int) });
			contextMock1.Setup(x => x.GetScope()).Returns(scope);

			cache.Remember(contextMock1.Object, reference);

			var contextMock2 = new Mock<IContext>();
			contextMock2.SetupGet(x => x.Binding).Returns(bindingMock.Object);
			contextMock2.SetupGet(x => x.HasInferredGenericArguments).Returns(true);
			contextMock2.SetupGet(x => x.GenericArguments).Returns(new[] { typeof(int) });
			contextMock2.Setup(x => x.GetScope()).Returns(scope);

			object instance = cache.TryGet(contextMock2.Object);

			instance.ShouldBeSameAs(reference.Instance);
		}

		[Fact]
		public void ReturnsNullIfInstanceAddedToCacheHasDifferentGenericParameters()
		{
			var scope = new object();
			var reference = new InstanceReference { Instance = new Sword() };

			var contextMock1 = new Mock<IContext>();
			contextMock1.SetupGet(x => x.Binding).Returns(bindingMock.Object);
			contextMock1.SetupGet(x => x.HasInferredGenericArguments).Returns(true);
			contextMock1.SetupGet(x => x.GenericArguments).Returns(new[] { typeof(int) });
			contextMock1.Setup(x => x.GetScope()).Returns(scope);

			cache.Remember(contextMock1.Object, reference);

			var contextMock2 = new Mock<IContext>();
			contextMock2.SetupGet(x => x.Binding).Returns(bindingMock.Object);
			contextMock2.SetupGet(x => x.HasInferredGenericArguments).Returns(true);
			contextMock2.SetupGet(x => x.GenericArguments).Returns(new[] { typeof(double) });
			contextMock2.Setup(x => x.GetScope()).Returns(scope);

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

			var writeContext = new Mock<IContext>();
			writeContext.SetupGet(x => x.Binding).Returns(bindingMock.Object);
			writeContext.SetupGet(x => x.HasInferredGenericArguments).Returns(true);
			writeContext.SetupGet(x => x.GenericArguments).Returns(new[] { typeof(int) });
			writeContext.Setup(x => x.GetScope()).Returns(scope);

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

			var writeContext = new Mock<IContext>();
			writeContext.SetupGet(x => x.Binding).Returns(bindingMock.Object);
			writeContext.SetupGet(x => x.HasInferredGenericArguments).Returns(true);
			writeContext.SetupGet(x => x.GenericArguments).Returns(new[] { typeof(int) });
			writeContext.Setup(x => x.GetScope()).Returns(scope);

			cache.Remember(writeContext.Object, reference);

			var readContext = new Mock<IContext>();
			readContext.SetupGet(x => x.Binding).Returns(bindingMock.Object);
			readContext.SetupGet(x => x.HasInferredGenericArguments).Returns(true);
			readContext.SetupGet(x => x.GenericArguments).Returns(new[] { typeof(int) });
			readContext.Setup(x => x.GetScope()).Returns(scope);

			object instance1 = cache.TryGet(readContext.Object);
			instance1.ShouldBeSameAs(reference.Instance);

			bool result = cache.Release(instance1);
			result.ShouldBeTrue();

			object instance2 = cache.TryGet(readContext.Object);
			instance2.ShouldBeNull();
		}
	}
}