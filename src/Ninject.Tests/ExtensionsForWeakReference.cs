using System;
using System.Threading;

namespace Ninject.Tests
{
	public static class ExtensionsForWeakReference
	{
		public static void WaitUntilGarbageCollected(this WeakReference reference)
		{
			while (reference.IsAlive)
				Thread.Sleep(100);
		}
	}
}