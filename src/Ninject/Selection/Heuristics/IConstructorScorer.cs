using System;
using System.Reflection;
using Ninject.Components;

namespace Ninject.Selection.Heuristics
{
	public interface IConstructorScorer : INinjectComponent
	{
		int Score(ConstructorInfo constructor);
	}
}