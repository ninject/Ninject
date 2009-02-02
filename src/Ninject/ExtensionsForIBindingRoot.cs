using System;
using Ninject.Syntax;

namespace Ninject
{
	public static class ExtensionsForIBindingRoot
	{
		public static IBindingToSyntax Bind<T>(this IBindingRoot root)
		{
			return root.Bind(typeof(T));
		}
	}
}
