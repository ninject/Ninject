using System;
using System.Threading;
using Moq;
using Ninject.Activation.Caching;
using Xunit;

namespace Ninject.Tests.CachePrunerTests
{
	public class CachePrunerContext
	{
		protected readonly CachePruner pruner;
		protected readonly Mock<ICache> cacheMock;

		public CachePrunerContext()
		{
			pruner = new CachePruner();
			cacheMock = new Mock<ICache>();
		}
	}

	public class WhenStartPruningIsCalled : CachePrunerContext
	{
		[Fact(Skip = "Need to rethink time-based tests")]
		public void CacheIsPrunedEverySecondIfGarbageCollectionHasOccurred()
		{
			cacheMock.Setup(x => x.Prune()).AtMostOnce();

			pruner.StartPruning(cacheMock.Object);

			GC.Collect();
			Thread.Sleep(1500);

			cacheMock.Verify(x => x.Prune());
		}
	}
}