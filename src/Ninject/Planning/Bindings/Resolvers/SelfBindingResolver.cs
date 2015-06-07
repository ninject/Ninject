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
using System.Linq;
using System.Reflection;
using Ninject.Activation;
using Ninject.Activation.Providers;
using Ninject.Components;
using Ninject.Infrastructure;

#endregion

namespace Ninject.Planning.Bindings.Resolvers
{
    using Ninject.Selection;

    ///<summary>
    ///</summary>
    public class SelfBindingResolver : NinjectComponent, IMissingBindingResolver
    {
        private readonly ISelector selector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadonlyKernel"/> class.
        /// </summary>
        /// <param name="selector">Dependency injection for <see cref="ISelector"/></param>
        public SelfBindingResolver(ISelector selector)
        {
            this.selector = selector;
        }

        /// <summary>
        /// Returns any bindings from the specified collection that match the specified service.
        /// </summary>
        /// <param name="bindings">The multimap of all registered bindings.</param>
        /// <param name="request">The service in question.</param>
        /// <returns>The series of matching bindings.</returns>
        public IEnumerable<IBinding> Resolve(IDictionary<Type, IEnumerable<IBinding>> bindings, IRequest request)
        {
            var service = request.Service;
            if (!TypeIsSelfBindable(service))
            {
                return Enumerable.Empty<IBinding>();
            }
            return new[]
                        {
                            new Binding(service)
                            {
                                ProviderCallback = StandardProvider.GetCreationCallback(service, selector)
                            }
                        };
        }

        /// <summary>
        /// Returns a value indicating whether the specified service is self-bindable.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns><see langword="True"/> if the type is self-bindable; otherwise <see langword="false"/>.</returns>
        protected virtual bool TypeIsSelfBindable(Type service)
        {
            var sInfo = service.GetTypeInfo();
            return !sInfo.IsInterface
                && !sInfo.IsAbstract
                && !sInfo.IsValueType
                && service != typeof(string)
                && !sInfo.ContainsGenericParameters;
        }
    }
}