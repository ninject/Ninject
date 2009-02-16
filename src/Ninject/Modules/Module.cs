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
using Ninject.Events;
using Ninject.Infrastructure.Language;
using Ninject.Planning.Bindings;
using Ninject.Syntax;
#endregion

namespace Ninject.Modules
{
	/// <summary>
	/// A pluggable unit that can be loaded into a kernel.
	/// </summary>
	public abstract class Module : IModule
	{
		/// <summary>
		/// Gets or sets the kernel that the module is loaded into.
		/// </summary>
		public IKernel Kernel { get; set; }

		/// <summary>
		/// Gets the bindings that were registered by the module.
		/// </summary>
		public ICollection<IBinding> Bindings { get; private set; }

		/// <summary>
		/// Occurs when a binding is added.
		/// </summary>
		public event EventHandler<BindingEventArgs> BindingAdded;

		/// <summary>
		/// Occurs when a binding is removed.
		/// </summary>
		public event EventHandler<BindingEventArgs> BindingRemoved;

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
			Kernel = kernel;
			Load();
		}

		/// <summary>
		/// Called when the module is unloaded from a kernel.
		/// </summary>
		/// <param name="kernel">The kernel that is unloading the module.</param>
		public void OnUnload(IKernel kernel)
		{
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
		/// Declares a binding for the specified service using the fluent syntax.
		/// </summary>
		/// <typeparam name="T">The service to bind.</typeparam>
		public IBindingToSyntax<T> Bind<T>()
		{
			return RegisterBindingAndCreateBuilder<T>(typeof(T));
		}

		/// <summary>
		/// Declares a binding for the specified service using the fluent syntax.
		/// </summary>
		/// <param name="service">The service to bind.</param>
		public IBindingToSyntax<object> Bind(Type service)
		{
			return RegisterBindingAndCreateBuilder<object>(service);
		}

		/// <summary>
		/// Registers the specified binding.
		/// </summary>
		/// <param name="binding">The binding to add.</param>
		public void AddBinding(IBinding binding)
		{
			Kernel.AddBinding(binding);
			Bindings.Add(binding);
			BindingAdded.Raise(this, new BindingEventArgs(binding));
		}

		/// <summary>
		/// Unregisters the specified binding.
		/// </summary>
		/// <param name="binding">The binding to remove.</param>
		public void RemoveBinding(IBinding binding)
		{
			Kernel.RemoveBinding(binding);
			Bindings.Remove(binding);
			BindingRemoved.Raise(this, new BindingEventArgs(binding));
		}

		/// <summary>
		/// Registers the specified binding and creates a builder to complete it.
		/// </summary>
		/// <typeparam name="T">The service being bound, or <see cref="object"/> if the non-generic version was used.</typeparam>
		/// <param name="service">The service being bound.</param>
		/// <returns>The builder that can be used to complete the binding.</returns>
		protected virtual BindingBuilder<T> RegisterBindingAndCreateBuilder<T>(Type service)
		{
			var binding = new Binding(service);
			AddBinding(binding);
			return new BindingBuilder<T>(binding);
		}
	}
}