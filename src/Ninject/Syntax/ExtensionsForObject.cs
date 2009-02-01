using System;

namespace Ninject.Syntax
{
	public static class ExtensionsForObject
	{
		public static void TryDispose(this object obj)
		{
			var disposable = obj as IDisposable;
			if (disposable != null) disposable.Dispose();
		}
	}
}