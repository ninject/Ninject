#region License
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Selection.Heuristics;
#endregion

namespace Ninject.Selection
{
	/// <summary>
	/// Selects members for injection.
	/// </summary>
	public class Selector : NinjectComponent, ISelector
	{
		private const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance;

		/// <summary>
		/// Gets or sets the constructor scorer.
		/// </summary>
		public IConstructorScorer ConstructorScorer { get; set; }

		/// <summary>
		/// Gets the property injection heuristics.
		/// </summary>
		public ICollection<IInjectionHeuristic> InjectionHeuristics { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Selector"/> class.
		/// </summary>
		/// <param name="constructorScorer">The constructor scorer.</param>
		/// <param name="injectionHeuristics">The injection heuristics.</param>
		public Selector(IConstructorScorer constructorScorer, IEnumerable<IInjectionHeuristic> injectionHeuristics)
		{
			Ensure.ArgumentNotNull(constructorScorer, "constructorScorer");
			Ensure.ArgumentNotNull(injectionHeuristics, "injectionHeuristics");

			ConstructorScorer = constructorScorer;
			InjectionHeuristics = injectionHeuristics.ToList();
		}

		/// <summary>
		/// Selects the constructor to call on the specified type, by using the constructor scorer.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>The selected constructor, or <see langword="null"/> if none were available.</returns>
		public ConstructorInfo SelectConstructor(Type type)
		{
			Ensure.ArgumentNotNull(type, "type");

			ConstructorInfo constructor = type.GetConstructors(Flags).OrderByDescending(c => ConstructorScorer.Score(c)).FirstOrDefault();

			if (constructor == null)
				constructor = type.GetConstructor(Type.EmptyTypes);

			return constructor;
		}

		/// <summary>
		/// Selects properties that should be injected.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>A series of the selected properties.</returns>
		public IEnumerable<PropertyInfo> SelectPropertiesForInjection(Type type)
		{
			Ensure.ArgumentNotNull(type, "type");
			return type.GetProperties(Flags).Where(p => InjectionHeuristics.Any(h => h.ShouldInject(p)));
		}

		/// <summary>
		/// Selects methods that should be injected.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>A series of the selected methods.</returns>
		public IEnumerable<MethodInfo> SelectMethodsForInjection(Type type)
		{
			Ensure.ArgumentNotNull(type, "type");
			return type.GetMethods(Flags).Where(m => InjectionHeuristics.Any(h => h.ShouldInject(m)));
		}
	}
}