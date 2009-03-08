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
using Ninject.Activation;
using Ninject.Infrastructure;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Syntax;
#endregion

namespace Ninject
{
	/// <summary>
	/// Extensions that enhance resolution of services.
	/// </summary>
	public static class ResolutionExtensions
	{
		/// <summary>
		/// Gets an instance of the specified service.
		/// </summary>
		/// <typeparam name="T">The service to resolve.</typeparam>
		/// <param name="root">The resolution root.</param>
		/// <param name="parameters">The parameters to pass to the request.</param>
		/// <returns>An instance of the service.</returns>
		public static T Get<T>(this IResolutionRoot root, params IParameter[] parameters)
		{
			return GetResolutionIterator(root, typeof(T), null, parameters, false).Cast<T>().FirstOrDefault();
		}

		/// <summary>
		/// Gets an instance of the specified service by using the first binding with the specified name.
		/// </summary>
		/// <typeparam name="T">The service to resolve.</typeparam>
		/// <param name="root">The resolution root.</param>
		/// <param name="name">The name of the binding.</param>
		/// <param name="parameters">The parameters to pass to the request.</param>
		/// <returns>An instance of the service.</returns>
		public static T Get<T>(this IResolutionRoot root, string name, params IParameter[] parameters)
		{
			return GetResolutionIterator(root, typeof(T), b => b.Name == name, parameters, false).Cast<T>().FirstOrDefault();
		}

		/// <summary>
		/// Gets an instance of the specified service by using the first binding that matches the specified constraint.
		/// </summary>
		/// <typeparam name="T">The service to resolve.</typeparam>
		/// <param name="root">The resolution root.</param>
		/// <param name="constraint">The constraint to apply to the binding.</param>
		/// <param name="parameters">The parameters to pass to the request.</param>
		/// <returns>An instance of the service.</returns>
		public static T Get<T>(this IResolutionRoot root, Func<IBindingMetadata, bool> constraint, params IParameter[] parameters)
		{
			return GetResolutionIterator(root, typeof(T), constraint, parameters, false).Cast<T>().FirstOrDefault();
		}

		/// <summary>
		/// Tries to get an instance of the specified service.
		/// </summary>
		/// <typeparam name="T">The service to resolve.</typeparam>
		/// <param name="root">The resolution root.</param>
		/// <param name="parameters">The parameters to pass to the request.</param>
		/// <returns>An instance of the service, or <see langword="null"/> if no implementation was available.</returns>
		public static T TryGet<T>(this IResolutionRoot root, params IParameter[] parameters)
		{
			return GetResolutionIterator(root, typeof(T), null, parameters, true).Cast<T>().FirstOrDefault();
		}

		/// <summary>
		/// Tries to get an instance of the specified service by using the first binding with the specified name.
		/// </summary>
		/// <typeparam name="T">The service to resolve.</typeparam>
		/// <param name="root">The resolution root.</param>
		/// <param name="name">The name of the binding.</param>
		/// <param name="parameters">The parameters to pass to the request.</param>
		/// <returns>An instance of the service, or <see langword="null"/> if no implementation was available.</returns>
		public static T TryGet<T>(this IResolutionRoot root, string name, params IParameter[] parameters)
		{
			return GetResolutionIterator(root, typeof(T), b => b.Name == name, parameters, true).Cast<T>().FirstOrDefault();
		}

		/// <summary>
		/// Tries to get an instance of the specified service by using the first binding that matches the specified constraint.
		/// </summary>
		/// <typeparam name="T">The service to resolve.</typeparam>
		/// <param name="root">The resolution root.</param>
		/// <param name="constraint">The constraint to apply to the binding.</param>
		/// <param name="parameters">The parameters to pass to the request.</param>
		/// <returns>An instance of the service, or <see langword="null"/> if no implementation was available.</returns>
		public static T TryGet<T>(this IResolutionRoot root, Func<IBindingMetadata, bool> constraint, params IParameter[] parameters)
		{
			return GetResolutionIterator(root, typeof(T), constraint, parameters, true).Cast<T>().FirstOrDefault();
		}

		/// <summary>
		/// Gets all available instances of the specified service.
		/// </summary>
		/// <typeparam name="T">The service to resolve.</typeparam>
		/// <param name="root">The resolution root.</param>
		/// <param name="parameters">The parameters to pass to the request.</param>
		/// <returns>A series of instances of the service.</returns>
		public static IEnumerable<T> GetAll<T>(this IResolutionRoot root, params IParameter[] parameters)
		{
			return GetResolutionIterator(root, typeof(T), null, parameters, true).Cast<T>();
		}

		/// <summary>
		/// Gets all instances of the specified service using bindings registered with the specified name.
		/// </summary>
		/// <typeparam name="T">The service to resolve.</typeparam>
		/// <param name="root">The resolution root.</param>
		/// <param name="name">The name of the binding.</param>
		/// <param name="parameters">The parameters to pass to the request.</param>
		/// <returns>A series of instances of the service.</returns>
		public static IEnumerable<T> GetAll<T>(this IResolutionRoot root, string name, params IParameter[] parameters)
		{
			return GetResolutionIterator(root, typeof(T), b => b.Name == name, parameters, true).Cast<T>();
		}

		/// <summary>
		/// Gets all instances of the specified service by using the bindings that match the specified constraint.
		/// </summary>
		/// <typeparam name="T">The service to resolve.</typeparam>
		/// <param name="root">The resolution root.</param>
		/// <param name="constraint">The constraint to apply to the bindings.</param>
		/// <param name="parameters">The parameters to pass to the request.</param>
		/// <returns>A series of instances of the service.</returns>
		public static IEnumerable<T> GetAll<T>(this IResolutionRoot root, Func<IBindingMetadata, bool> constraint, params IParameter[] parameters)
		{
			return GetResolutionIterator(root, typeof(T), constraint, parameters, true).Cast<T>();
		}

		/// <summary>
		/// Gets an instance of the specified service.
		/// </summary>
		/// <param name="root">The resolution root.</param>
		/// <param name="service">The service to resolve.</param>
		/// <param name="parameters">The parameters to pass to the request.</param>
		/// <returns>An instance of the service.</returns>
		public static object Get(this IResolutionRoot root, Type service, params IParameter[] parameters)
		{
			return GetResolutionIterator(root, service, null, parameters, false).FirstOrDefault();
		}

		/// <summary>
		/// Gets an instance of the specified service by using the first binding with the specified name.
		/// </summary>
		/// <param name="root">The resolution root.</param>
		/// <param name="service">The service to resolve.</param>
		/// <param name="name">The name of the binding.</param>
		/// <param name="parameters">The parameters to pass to the request.</param>
		/// <returns>An instance of the service.</returns>
		public static object Get(this IResolutionRoot root, Type service, string name, params IParameter[] parameters)
		{
			return GetResolutionIterator(root, service, b => b.Name == name, parameters, false).FirstOrDefault();
		}

		/// <summary>
		/// Gets an instance of the specified service by using the first binding that matches the specified constraint.
		/// </summary>
		/// <param name="root">The resolution root.</param>
		/// <param name="service">The service to resolve.</param>
		/// <param name="constraint">The constraint to apply to the binding.</param>
		/// <param name="parameters">The parameters to pass to the request.</param>
		/// <returns>An instance of the service.</returns>
		public static object Get(this IResolutionRoot root, Type service, Func<IBindingMetadata, bool> constraint, params IParameter[] parameters)
		{
			return GetResolutionIterator(root, service, constraint, parameters, false).FirstOrDefault();
		}

		/// <summary>
		/// Tries to get an instance of the specified service.
		/// </summary>
		/// <param name="root">The resolution root.</param>
		/// <param name="service">The service to resolve.</param>
		/// <param name="parameters">The parameters to pass to the request.</param>
		/// <returns>An instance of the service, or <see langword="null"/> if no implementation was available.</returns>
		public static object TryGet(this IResolutionRoot root, Type service, params IParameter[] parameters)
		{
			return GetResolutionIterator(root, service, null, parameters, true).FirstOrDefault();
		}

		/// <summary>
		/// Tries to get an instance of the specified service by using the first binding with the specified name.
		/// </summary>
		/// <param name="root">The resolution root.</param>
		/// <param name="service">The service to resolve.</param>
		/// <param name="name">The name of the binding.</param>
		/// <param name="parameters">The parameters to pass to the request.</param>
		/// <returns>An instance of the service, or <see langword="null"/> if no implementation was available.</returns>
		public static object TryGet(this IResolutionRoot root, Type service, string name, params IParameter[] parameters)
		{
			return GetResolutionIterator(root, service, b => b.Name == name, parameters, true).FirstOrDefault();
		}

		/// <summary>
		/// Tries to get an instance of the specified service by using the first binding that matches the specified constraint.
		/// </summary>
		/// <param name="root">The resolution root.</param>
		/// <param name="service">The service to resolve.</param>
		/// <param name="constraint">The constraint to apply to the binding.</param>
		/// <param name="parameters">The parameters to pass to the request.</param>
		/// <returns>An instance of the service, or <see langword="null"/> if no implementation was available.</returns>
		public static object TryGet(this IResolutionRoot root, Type service, Func<IBindingMetadata, bool> constraint, params IParameter[] parameters)
		{
			return GetResolutionIterator(root, service, constraint, parameters, true).FirstOrDefault();
		}

		/// <summary>
		/// Gets all available instances of the specified service.
		/// </summary>
		/// <param name="root">The resolution root.</param>
		/// <param name="service">The service to resolve.</param>
		/// <param name="parameters">The parameters to pass to the request.</param>
		/// <returns>A series of instances of the service.</returns>
		public static IEnumerable<object> GetAll(this IResolutionRoot root, Type service, params IParameter[] parameters)
		{
			return GetResolutionIterator(root, service, null, parameters, true);
		}

		/// <summary>
		/// Gets all instances of the specified service using bindings registered with the specified name.
		/// </summary>
		/// <param name="root">The resolution root.</param>
		/// <param name="service">The service to resolve.</param>
		/// <param name="name">The name of the binding.</param>
		/// <param name="parameters">The parameters to pass to the request.</param>
		/// <returns>A series of instances of the service.</returns>
		public static IEnumerable<object> GetAll(this IResolutionRoot root, Type service, string name, params IParameter[] parameters)
		{
			return GetResolutionIterator(root, service, b => b.Name == name, parameters, true);
		}

		/// <summary>
		/// Gets all instances of the specified service by using the bindings that match the specified constraint.
		/// </summary>
		/// <param name="root">The resolution root.</param>
		/// <param name="service">The service to resolve.</param>
		/// <param name="constraint">The constraint to apply to the bindings.</param>
		/// <param name="parameters">The parameters to pass to the request.</param>
		/// <returns>A series of instances of the service.</returns>
		public static IEnumerable<object> GetAll(this IResolutionRoot root, Type service, Func<IBindingMetadata, bool> constraint, params IParameter[] parameters)
		{
			return GetResolutionIterator(root, service, constraint, parameters, true);
		}

		private static IEnumerable<object> GetResolutionIterator(IResolutionRoot root, Type service, Func<IBindingMetadata, bool> constraint, IEnumerable<IParameter> parameters, bool isOptional)
		{
			Ensure.ArgumentNotNull(root, "root");
			Ensure.ArgumentNotNull(service, "service");
			Ensure.ArgumentNotNull(parameters, "parameters");

			IRequest request = root.CreateRequest(service, constraint, parameters, isOptional);
			return root.Resolve(request);
		}
	}
}
