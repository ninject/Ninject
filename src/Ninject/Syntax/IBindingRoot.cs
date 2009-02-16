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
using Ninject.Events;
using Ninject.Planning.Bindings;
#endregion

namespace Ninject.Syntax
{
	/// <summary>
	/// Provides a path to register bindings.
	/// </summary>
	public interface IBindingRoot
	{
		/// <summary>
		/// Occurs when a binding is added.
		/// </summary>
		event EventHandler<BindingEventArgs> BindingAdded;

		/// <summary>
		/// Occurs when a binding is removed.
		/// </summary>
		event EventHandler<BindingEventArgs> BindingRemoved;

		/// <summary>
		/// Declares a binding for the specified service using the fluent syntax.
		/// </summary>
		/// <typeparam name="T">The service to bind.</typeparam>
		IBindingToSyntax<T> Bind<T>();

		/// <summary>
		/// Declares a binding for the specified service using the fluent syntax.
		/// </summary>
		/// <param name="service">The service to bind.</param>
		IBindingToSyntax<object> Bind(Type service);

		/// <summary>
		/// Registers the specified binding.
		/// </summary>
		/// <param name="binding">The binding to add.</param>
		void AddBinding(IBinding binding);

		/// <summary>
		/// Unregisters the specified binding.
		/// </summary>
		/// <param name="binding">The binding to remove.</param>
		void RemoveBinding(IBinding binding);
	}
}