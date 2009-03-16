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
using Ninject.Infrastructure;
using Ninject.Infrastructure.Language;
using Ninject.Planning.Bindings;
using Ninject.Syntax;
#endregion

namespace Ninject.Modules
{
	/// <summary>
	/// A pluggable unit that can be loaded into a kernel.
	/// </summary>
	public abstract class Module : BindingRoot, INinjectModule
	{
		/// <summary>
		/// Gets the kernel that the module is loaded into.
		/// </summary>
		public IKernel Kernel { get; private set; }

		/// <summary>
		/// Gets the module's name. Only a single module with a given name can be loaded at one time.
		/// </summary>
		public virtual string Name
		{
			get { return GetType().Name; }
		}

		/// <summary>
		/// Gets the bindings that were registered by the module.
		/// </summary>
		public ICollection<IBinding> Bindings { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Module"/> class.
		/// </summary>
		protected Module()
		{
			Bindings = new List<IBinding>();
		}

		/// <summary>
		/// Called when the module is loaded into a kernel.
		/// </summary>
		/// <param name="kernel">The kernel that is loading the module.</param>
		public void OnLoad(IKernel kernel)
		{
			Ensure.ArgumentNotNull(kernel, "kernel");
			Kernel = kernel;
			Load();
		}

		/// <summary>
		/// Called when the module is unloaded from a kernel.
		/// </summary>
		/// <param name="kernel">The kernel that is unloading the module.</param>
		public void OnUnload(IKernel kernel)
		{
			Ensure.ArgumentNotNull(kernel, "kernel");
			Unload();
			Bindings.Map(Kernel.RemoveBinding);
			Kernel = null;
		}

		/// <summary>
		/// Loads the module into the kernel.
		/// </summary>
		public abstract void Load();

		/// <summary>
		/// Unloads this module from the kernel.
		/// </summary>
		public virtual void Unload() { }

		/// <summary>
		/// Unregisters all bindings for the specified service.
		/// </summary>
		/// <param name="service">The service to unbind.</param>
		public override void Unbind(Type service)
		{
			Kernel.Unbind(service);
		}

		/// <summary>
		/// Registers the specified binding.
		/// </summary>
		/// <param name="binding">The binding to add.</param>
		public override void AddBinding(IBinding binding)
		{
			Ensure.ArgumentNotNull(binding, "binding");

			Kernel.AddBinding(binding);
			Bindings.Add(binding);
		}

		/// <summary>
		/// Unregisters the specified binding.
		/// </summary>
		/// <param name="binding">The binding to remove.</param>
		public override void RemoveBinding(IBinding binding)
		{
			Ensure.ArgumentNotNull(binding, "binding");

			Kernel.RemoveBinding(binding);
			Bindings.Remove(binding);
		}
	}
}