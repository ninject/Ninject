using System;
using System.Reflection;
using Ninject.Components;
using Ninject.Infrastructure.Language;

namespace Ninject.Selection.Heuristics
{
	public class StandardConstructorScorer : NinjectComponent, IConstructorScorer
	{
		public int Score(ConstructorInfo constructor)
		{
			return constructor.HasAttribute(Kernel.Settings.InjectAttribute) ? Int32.MaxValue : constructor.GetParameters().Length;
		}
	}
}