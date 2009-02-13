using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Syntax;

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
			return root.Resolve(typeof(T), null, parameters).Select(hook => hook.Resolve()).Cast<T>().FirstOrDefault();
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
			return root.Get<T>(m => m.Name == name, parameters);
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
			return (T)root.Resolve(typeof(T), new[] { constraint }, parameters).Select(hook => hook.Resolve()).FirstOrDefault();
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
			return root.Resolve(typeof(T), null, parameters).Select(hook => hook.Resolve()).Cast<T>();
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
			return root.GetAll<T>(m => m.Name == name, parameters);
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
			return root.Resolve(typeof(T), new[] { constraint }, parameters).Select(hook => hook.Resolve()).Cast<T>();
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
			return root.Resolve(service, null, parameters).Select(hook => hook.Resolve()).FirstOrDefault();
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
			return root.Get(service, m => m.Name == name, parameters);
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
			return root.Resolve(service, new[] { constraint }, parameters).Select(hook => hook.Resolve()).FirstOrDefault();
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
			return root.Resolve(service, null, parameters).Select(hook => hook.Resolve());
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
			return root.GetAll(service, m => m.Name == name, parameters);
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
			return root.Resolve(service, new[] { constraint }, parameters).Select(hook => hook.Resolve());
		}
	}
}
