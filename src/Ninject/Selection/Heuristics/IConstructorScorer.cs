using System;
using System.Reflection;
using Ninject.Infrastructure.Components;

namespace Ninject.Selection.Heuristics
{
	public interface IConstructorScorer : INinjectComponent
	{
		int Score(ConstructorInfo constructor);
	}
}