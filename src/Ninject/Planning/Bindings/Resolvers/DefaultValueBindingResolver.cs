// -------------------------------------------------------------------------------------------------
// <copyright file="DefaultValueBindingResolver.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Planning.Bindings.Resolvers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ninject.Activation;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Represents a binding resolver that takes the target default value as the resolved object.
    /// </summary>
    public class DefaultValueBindingResolver : NinjectComponent, IMissingBindingResolver
    {
        /// <summary>
        /// Returns any bindings from the specified collection that match the specified service.
        /// </summary>
        /// <param name="bindings">The multimap of all registered bindings.</param>
        /// <param name="request">The service in question.</param>
        /// <returns>The series of matching bindings.</returns>
        public IEnumerable<IBinding> Resolve(Multimap<Type, IBinding> bindings, IRequest request)
        {
            var service = request.Service;
            return HasDefaultValue(request.Target)
                       ? new[]
                             {
                                 new Binding(service)
                                     {
                                         Condition = r => HasDefaultValue(r.Target),
                                         ProviderCallback = _ => new DefaultParameterValueProvider(service),
                                     },
                             }
                       : Enumerable.Empty<IBinding>();
        }

        private static bool HasDefaultValue(ITarget target)
        {
            return target != null && target.HasDefaultValue;
        }

        private class DefaultParameterValueProvider : IProvider
        {
            public DefaultParameterValueProvider(Type type)
            {
                this.Type = type;
            }

            public Type Type { get; private set; }

            public object Create(IContext context)
            {
                var target = context.Request.Target;
                return target?.DefaultValue;
            }
        }
    }
}