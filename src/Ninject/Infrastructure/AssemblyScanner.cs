#if !SILVERLIGHT
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
#endregion

namespace Ninject.Infrastructure
{
	/// <summary>
	/// Finds types in assemblies without loading them into the main <see cref="AppDomain"/>.
	/// </summary>
	public static class AssemblyScanner
	{
		/// <summary>
		/// Finds matching types in the specified assemblies.
		/// </summary>
		/// <param name="assemblyOrFileNames">The assembly or file names to scan.</param>
		/// <param name="predicate">The predicate to match.</param>
		/// <returns>A series of the matching types.</returns>
		public static IEnumerable<Type> FindMatchingTypesInAssemblies(IEnumerable<string> assemblyOrFileNames, Func<Type, bool> predicate)
		{
			AppDomain temporaryDomain = CreateTemporaryAppDomain();

			foreach (string file in assemblyOrFileNames)
			{
				Assembly assembly;

				try
				{
					assembly = temporaryDomain.Load(GetAssemblyName(file));
				}
				catch (BadImageFormatException)
				{
					// Ignore native assemblies
					continue;
				}

				foreach (Type type in assembly.GetExportedTypes().Where(predicate))
					yield return type;
			}

			AppDomain.Unload(temporaryDomain);
		}

		private static AssemblyName GetAssemblyName(string assemblyOrFileName)
		{
			AssemblyName name;

			try { name = new AssemblyName(assemblyOrFileName); }
			catch { name = new AssemblyName { CodeBase = assemblyOrFileName }; }

			return name;
		}

		private static AppDomain CreateTemporaryAppDomain()
		{
			return AppDomain.CreateDomain(
				"NinjectAssemblyScanner",
				AppDomain.CurrentDomain.Evidence,
				AppDomain.CurrentDomain.BaseDirectory,
				AppDomain.CurrentDomain.RelativeSearchPath,
				false);
		}
	}
}
#endif //!SILVERLIGHT