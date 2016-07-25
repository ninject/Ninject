#region License
//
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
//
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
//
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
            return GetResolutionIterator(root, typeof(T), null, parameters, false, true).Cast<T>().Single();
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
            return GetResolutionIterator(root, typeof(T), b => b.Name == name, parameters, false, true).Cast<T>().Single();
        }

        /// <summary>
        /// Gets an instance of the specified service by using the first binding that matches the specified constraint.
        /// </summary>
        /// <typeparam name="T">The service to resolve.</typeparam>
        /// <param name="root">The resolution root.</param>
        /// <param name="constraint">The constraint to apply to the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>An instance of the service.</returns>
        public static T Get<T>(this IResolutionRoot root, Predicate<IBindingMetadata> constraint, params IParameter[] parameters)
        {
            return GetResolutionIterator(root, typeof(T), constraint, parameters, false, true).Cast<T>().Single();
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
            return TryGet(() => GetResolutionIterator(root, typeof(T), null, parameters, true, true).Cast<T>());
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
            return TryGet(() => GetResolutionIterator(root, typeof(T), b => b.Name == name, parameters, true, true).Cast<T>());
        }

        /// <summary>
        /// Tries to get an instance of the specified service by using the first binding that matches the specified constraint.
        /// </summary>
        /// <typeparam name="T">The service to resolve.</typeparam>
        /// <param name="root">The resolution root.</param>
        /// <param name="constraint">The constraint to apply to the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>An instance of the service, or <see langword="null"/> if no implementation was available.</returns>
        public static T TryGet<T>(this IResolutionRoot root, Predicate<IBindingMetadata> constraint, params IParameter[] parameters)
        {
            return TryGet(() => GetResolutionIterator(root, typeof(T), constraint, parameters, true, true).Cast<T>());
        }

        /// <summary>
        /// Tries to get an instance of the specified service.
        /// </summary>
        /// <typeparam name="T">The service to resolve.</typeparam>
        /// <param name="root">The resolution root.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>An instance of the service, or <see langword="null"/> if no implementation was available.</returns>
        public static T TryGetAndThrowOnInvalidBinding<T>(this IResolutionRoot root, params IParameter[] parameters)
        {
            return DoTryGetAndThrowOnInvalidBinding<T>(root, null, parameters);
        }

        /// <summary>
        /// Tries to get an instance of the specified service by using the first binding with the specified name.
        /// </summary>
        /// <typeparam name="T">The service to resolve.</typeparam>
        /// <param name="root">The resolution root.</param>
        /// <param name="name">The name of the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>An instance of the service, or <see langword="null"/> if no implementation was available.</returns>
        public static T TryGetAndThrowOnInvalidBinding<T>(this IResolutionRoot root, string name, params IParameter[] parameters)
        {
            return DoTryGetAndThrowOnInvalidBinding<T>(root, b => b.Name == name, parameters);
        }

        /// <summary>
        /// Tries to get an instance of the specified service by using the first binding that matches the specified constraint.
        /// </summary>
        /// <typeparam name="T">The service to resolve.</typeparam>
        /// <param name="root">The resolution root.</param>
        /// <param name="constraint">The constraint to apply to the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>An instance of the service, or <see langword="null"/> if no implementation was available.</returns>
        public static T TryGetAndThrowOnInvalidBinding<T>(this IResolutionRoot root, Predicate<IBindingMetadata> constraint, params IParameter[] parameters)
        {
            return DoTryGetAndThrowOnInvalidBinding<T>(root, constraint, parameters);
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
            return GetResolutionIterator(root, typeof(T), null, parameters, true, false).Cast<T>();
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
            return GetResolutionIterator(root, typeof(T), b => b.Name == name, parameters, true, false).Cast<T>();
        }

        /// <summary>
        /// Gets all instances of the specified service by using the bindings that match the specified constraint.
        /// </summary>
        /// <typeparam name="T">The service to resolve.</typeparam>
        /// <param name="root">The resolution root.</param>
        /// <param name="constraint">The constraint to apply to the bindings.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>A series of instances of the service.</returns>
        public static IEnumerable<T> GetAll<T>(this IResolutionRoot root, Predicate<IBindingMetadata> constraint, params IParameter[] parameters)
        {
            return GetResolutionIterator(root, typeof(T), constraint, parameters, true, false).Cast<T>();
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
            return GetResolutionIterator(root, service, null, parameters, false, true).Single();
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
            return GetResolutionIterator(root, service, b => b.Name == name, parameters, false, true).Single();
        }

        /// <summary>
        /// Gets an instance of the specified service by using the first binding that matches the specified constraint.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="service">The service to resolve.</param>
        /// <param name="constraint">The constraint to apply to the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>An instance of the service.</returns>
        public static object Get(this IResolutionRoot root, Type service, Predicate<IBindingMetadata> constraint, params IParameter[] parameters)
        {
            return GetResolutionIterator(root, service, constraint, parameters, false, true).Single();
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
            return TryGet(() => GetResolutionIterator(root, service, null, parameters, true, true));
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
            return TryGet(() => GetResolutionIterator(root, service, b => b.Name == name, parameters, true, false));
        }

        /// <summary>
        /// Tries to get an instance of the specified service by using the first binding that matches the specified constraint.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="service">The service to resolve.</param>
        /// <param name="constraint">The constraint to apply to the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>An instance of the service, or <see langword="null"/> if no implementation was available.</returns>
        public static object TryGet(this IResolutionRoot root, Type service, Predicate<IBindingMetadata> constraint, params IParameter[] parameters)
        {
            return TryGet(() => GetResolutionIterator(root, service, constraint, parameters, true, false));
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
            return GetResolutionIterator(root, service, null, parameters, true, false);
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
            return GetResolutionIterator(root, service, b => b.Name == name, parameters, true, false);
        }

        /// <summary>
        /// Gets all instances of the specified service by using the bindings that match the specified constraint.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="service">The service to resolve.</param>
        /// <param name="constraint">The constraint to apply to the bindings.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>A series of instances of the service.</returns>
        public static IEnumerable<object> GetAll(this IResolutionRoot root, Type service, Predicate<IBindingMetadata> constraint, params IParameter[] parameters)
        {
            return GetResolutionIterator(root, service, constraint, parameters, true, false);
        }

        /// <summary>
        /// Evaluates if an instance of the specified service can be resolved.
        /// </summary>
        /// <typeparam name="T">The service to resolve.</typeparam>
        /// <param name="root">The resolution root.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns><c>True</c> if the request can be resolved; otherwise, <c>false</c>.</returns>
        public static bool CanResolve<T>(this IResolutionRoot root, params IParameter[] parameters)
        {
            return CanResolve(root, typeof(T), null, parameters, false, true);
        }

        /// <summary>
        /// Evaluates if  an instance of the specified service by using the first binding with the specified name can be resolved.
        /// </summary>
        /// <typeparam name="T">The service to resolve.</typeparam>
        /// <param name="root">The resolution root.</param>
        /// <param name="name">The name of the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns><c>True</c> if the request can be resolved; otherwise, <c>false</c>.</returns>
        public static bool CanResolve<T>(this IResolutionRoot root, string name, params IParameter[] parameters)
        {
            return CanResolve(root, typeof(T), b => b.Name == name, parameters, false, true);
        }

        /// <summary>
        /// Evaluates if  an instance of the specified service by using the first binding that matches the specified constraint can be resolved.
        /// </summary>
        /// <typeparam name="T">The service to resolve.</typeparam>
        /// <param name="root">The resolution root.</param>
        /// <param name="constraint">The constraint to apply to the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns><c>True</c> if the request can be resolved; otherwise, <c>false</c>.</returns>
        public static bool CanResolve<T>(this IResolutionRoot root, Predicate<IBindingMetadata> constraint, params IParameter[] parameters)
        {
            return CanResolve(root, typeof(T), constraint, parameters, false, true);
        }

        /// <summary>
        /// Gets an instance of the specified service.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="service">The service to resolve.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns><c>True</c> if the request can be resolved; otherwise, <c>false</c>.</returns>
        public static bool CanResolve(this IResolutionRoot root, Type service, params IParameter[] parameters)
        {
            return CanResolve(root, service, null, parameters, false, true);
        }

        /// <summary>
        /// Gets an instance of the specified service by using the first binding with the specified name.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="service">The service to resolve.</param>
        /// <param name="name">The name of the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns><c>True</c> if the request can be resolved; otherwise, <c>false</c>.</returns>
        public static bool CanResolve(this IResolutionRoot root, Type service, string name, params IParameter[] parameters)
        {
            return CanResolve(root, service, b => b.Name == name, parameters, false, true);
        }

        /// <summary>
        /// Gets an instance of the specified service by using the first binding that matches the specified constraint.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="service">The service to resolve.</param>
        /// <param name="constraint">The constraint to apply to the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns><c>True</c> if the request can be resolved; otherwise, <c>false</c>.</returns>
        public static bool CanResolve(this IResolutionRoot root, Type service, Predicate<IBindingMetadata> constraint, params IParameter[] parameters)
        {
            return CanResolve(root, service, constraint, parameters, false, true);
        }

        private static bool CanResolve(IResolutionRoot root, Type service, Predicate<IBindingMetadata> constraint, IEnumerable<IParameter> parameters, bool isOptional, bool isUnique)
        {
            Contract.Requires(root != null);
            Contract.Requires(service != null);
            Contract.Requires(parameters != null);

            var request = root.CreateRequest(service, constraint, parameters, isOptional, isUnique);
            return root.CanResolve(request);
        }

        private static IEnumerable<object> GetResolutionIterator(IResolutionRoot root, Type service, Predicate<IBindingMetadata> constraint, IEnumerable<IParameter> parameters, bool isOptional, bool isUnique)
        {
            Contract.Requires(root != null);
            Contract.Requires(service != null);
            Contract.Requires(parameters != null);

            var request = root.CreateRequest(service, constraint, parameters, isOptional, isUnique);
            return root.Resolve(request);
        }

        private static IEnumerable<object> GetResolutionIterator(IResolutionRoot root, Type service, Predicate<IBindingMetadata> constraint, IEnumerable<IParameter> parameters, bool isOptional, bool isUnique, bool forceUnique)
        {
            Contract.Requires(root != null);
            Contract.Requires(service != null);
            Contract.Requires(parameters != null);

            var request = root.CreateRequest(service, constraint, parameters, isOptional, isUnique);
            request.ForceUnique = forceUnique;
            return root.Resolve(request);
        }

        private static T TryGet<T>(Func<IEnumerable<T>> iterator)
        {
            try
            {
                return iterator().SingleOrDefault();
            }
            catch (ActivationException)
            {
                return default(T);
            }
        }

        private static T DoTryGetAndThrowOnInvalidBinding<T>(IResolutionRoot root, Predicate<IBindingMetadata> constraint, IEnumerable<IParameter> parameters)
        {
            return GetResolutionIterator(root, typeof(T), constraint, parameters, true, true, true).Cast<T>().SingleOrDefault();
        }
    }
}
