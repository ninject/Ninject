using System;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Planning.Bindings;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Unit.CacheTests
{
	public class CacheContext
	{
		protected Mock<IPipeline> activatorMock;
		protected Mock<ICachePruner> prunerMock;
		protected Mock<IBinding> bindingMock;
		protected Cache cache;

		public CacheContext()
		{
			activatorMock = new Mock<IPipeline>();
			prunerMock = new Mock<ICachePruner>();
			bindingMock = new Mock<IBinding>();
			cache = new Cache(activatorMock.Object, prunerMock.Object);
		}
	}

	public class WhenCacheIsCreated
	{
		[Fact]
		public void AsksPrunerToStartPruning()
		{
			var activatorMock = new Mock<IPipeline>();
			var prunerMock = new Mock<ICachePruner>();

			prunerMock.Setup(x => x.StartPruning(It.IsAny<Cache>())).AtMostOnce();

			var cache = new Cache(activatorMock.Object, prunerMock.Object);

			prunerMock.Verify(x => x.StartPruning(cache));
		}
	}

	public class WhenCacheIsDisposed : CacheContext
	{
		[Fact]
		public void AsksPrunerToStopPruning()
		{
			prunerMock.Setup(x => x.StopPruning()).AtMostOnce();
			cache.Dispose();
			prunerMock.Verify(x => x.StopPruning());
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

			Assert.Null(instance);
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

			Assert.Same(sword, instance);
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

			Assert.Null(instance);
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

			Assert.Same(sword, instance);
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

			Assert.Null(instance);
		}
	}
}