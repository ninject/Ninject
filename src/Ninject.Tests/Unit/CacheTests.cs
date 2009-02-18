using System;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Infrastructure;
using Ninject.Planning.Bindings;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Unit.CacheTests
{
	public class CacheContext
	{
		protected Mock<IPipeline> activatorMock;
		protected Mock<IGarbageCollectionWatcher> gcWatcherMock;
		protected Mock<IBinding> bindingMock;
		protected Cache cache;

		public CacheContext()
		{
			activatorMock = new Mock<IPipeline>();
			gcWatcherMock = new Mock<IGarbageCollectionWatcher>();
			bindingMock = new Mock<IBinding>();
			cache = new Cache(activatorMock.Object) { GCWatcher = gcWatcherMock.Object };
		}
	}

	public class WhenCacheIsDisposed : CacheContext
	{
		[Fact]
		public void DisposesOfGCWatcher()
		{
			cache.Dispose();
			gcWatcherMock.Verify(x => x.Dispose());
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
			var sword = new Sword();

			var contextMock1 = new Mock<IContext>();
			contextMock1.SetupGet(x => x.Binding).Returns(bindingMock.Object);
			contextMock1.SetupGet(x => x.Instance).Returns(sword);
			contextMock1.Setup(x => x.GetScope()).Returns(scope);

			cache.Remember(contextMock1.Object);

			var contextMock2 = new Mock<IContext>();
			contextMock2.SetupGet(x => x.Binding).Returns(bindingMock.Object);
			contextMock2.Setup(x => x.GetScope()).Returns(scope);

			object instance = cache.TryGet(contextMock2.Object);

			instance.ShouldBeSameAs(sword);
		}

		[Fact]
		public void ReturnsNullIfNoInstancesHaveBeenAddedWithinSpecifiedScope()
		{
			var sword = new Sword();

			var contextMock1 = new Mock<IContext>();
			contextMock1.SetupGet(x => x.Binding).Returns(bindingMock.Object);
			contextMock1.SetupGet(x => x.Instance).Returns(sword);
			contextMock1.Setup(x => x.GetScope()).Returns(new object());

			cache.Remember(contextMock1.Object);

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
			var sword = new Sword();

			var contextMock1 = new Mock<IContext>();
			contextMock1.SetupGet(x => x.Binding).Returns(bindingMock.Object);
			contextMock1.SetupGet(x => x.Instance).Returns(sword);
			contextMock1.SetupGet(x => x.HasInferredGenericArguments).Returns(true);
			contextMock1.SetupGet(x => x.GenericArguments).Returns(new[] { typeof(int) });
			contextMock1.Setup(x => x.GetScope()).Returns(scope);

			cache.Remember(contextMock1.Object);

			var contextMock2 = new Mock<IContext>();
			contextMock2.SetupGet(x => x.Binding).Returns(bindingMock.Object);
			contextMock2.SetupGet(x => x.HasInferredGenericArguments).Returns(true);
			contextMock2.SetupGet(x => x.GenericArguments).Returns(new[] { typeof(int) });
			contextMock2.Setup(x => x.GetScope()).Returns(scope);

			object instance = cache.TryGet(contextMock2.Object);

			instance.ShouldBeSameAs(sword);
		}

		[Fact]
		public void ReturnsNullIfInstanceAddedToCacheHasDifferentGenericParameters()
		{
			var scope = new object();
			var sword = new Sword();

			var contextMock1 = new Mock<IContext>();
			contextMock1.SetupGet(x => x.Binding).Returns(bindingMock.Object);
			contextMock1.SetupGet(x => x.Instance).Returns(sword);
			contextMock1.SetupGet(x => x.HasInferredGenericArguments).Returns(true);
			contextMock1.SetupGet(x => x.GenericArguments).Returns(new[] { typeof(int) });
			contextMock1.Setup(x => x.GetScope()).Returns(scope);

			cache.Remember(contextMock1.Object);

			var contextMock2 = new Mock<IContext>();
			contextMock2.SetupGet(x => x.Binding).Returns(bindingMock.Object);
			contextMock2.SetupGet(x => x.HasInferredGenericArguments).Returns(true);
			contextMock2.SetupGet(x => x.GenericArguments).Returns(new[] { typeof(double) });
			contextMock2.Setup(x => x.GetScope()).Returns(scope);

			object instance = cache.TryGet(contextMock2.Object);

			instance.ShouldBeNull();
		}
	}
}