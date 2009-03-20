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
using System.Reflection;
using Ninject.Activation.Blocks;
using Ninject.Components;
using Ninject.Infrastructure.Disposal;
using Ninject.Modules;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Syntax;
#endregion

namespace Ninject
{
	/// <summary>
	/// A super-factory that can create objects of all kinds, following hints provided by <see cref="IBinding"/>s.
	/// </summary>
	public interface IKernel : IBindingRoot, IResolutionRoot, INotifyWhenDisposed
	{
		/// <summary>
		/// Gets the kernel settings.
		/// </summary>
		INinjectSettings Settings { get; }

		/// <summary>
		/// Gets the component container, which holds components that contribute to Ninject.
		/// </summary>
		IComponentContainer Components { get; }

		/// <summary>
		/// Gets the modules that have been loaded into the kernel.
		/// </summary>
		/// <returns>A series of loaded modules.</returns>
		IEnumerable<INinjectModule> GetModules();

		/// <summary>
		/// Determines whether a module with the specified name has been loaded in the kernel.
		/// </summary>
		/// <param name="name">The name of the module.</param>
		/// <returns><c>True</c> if the specified module has been loaded; otherwise, <c>false</c>.</returns>
		bool HasModule(string name);

		/// <summary>
		/// Loads the module(s) into the kernel.
		/// </summary>
		/// <param name="modules">The modules to load.</param>
		void Load(IEnumerable<INinjectModule> modules);

		#if !SILVERLIGHT
		/// <summary>
		/// Loads modules from the files that match the specified pattern(s).
		/// </summary>
		/// <param name="filePatterns">The file patterns (i.e. "*.dll", "modules/*.rb") to match.</param>
		void Load(IEnumerable<string> filePatterns);

		/// <summary>
		/// Loads modules defined in the specified assemblies.
		/// </summary>
		/// <param name="assemblies">The assemblies to search.</param>
		void Load(IEnumerable<Assembly> assemblies);
		#endif

		/// <summary>
		/// Unloads the plugin with the specified name.
		/// </summary>
		/// <param name="name">The plugin's name.</param>
		void Unload(string name);

		/// <summary>
		/// Injects the specified existing instance, without managing its lifecycle.
		/// </summary>
		/// <param name="instance">The instance to inject.</param>
		/// <param name="parameters">The parameters to pass to the request.</param>
		void Inject(object instance, params IParameter[] parameters);

		/// <summary>
		/// Gets the bindings registered for the specified service.
		/// </summary>
		/// <param name="service">The service in question.</param>
		/// <returns>A series of bindings that are registered for the service.</returns>
		IEnumerable<IBinding> GetBindings(Type service);

		/// <summary>
		/// Begins a new activation block, which can be used to deterministically dispose resolved instances.
		/// </summary>
		/// <returns>The new activation block.</returns>
		IActivationBlock BeginBlock();
	}
}
