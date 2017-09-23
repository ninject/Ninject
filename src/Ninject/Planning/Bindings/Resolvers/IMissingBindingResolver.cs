// -------------------------------------------------------------------------------------------------
// <copyright file="IMissingBindingResolver.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Planning.Bindings.Resolvers
{
    using System;
    using System.Collections.Generic;
    using Ninject.Activation;
    using Ninject.Components;
    using Ninject.Infrastructure;

    /// <summary>
    /// Contains logic about which bindings to use for a given service request
    /// when other attempts have failed.
    /// </summary>
    public interface IMissingBindingResolver : INinjectComponent
    {
        /// <summary>
        /// Returns any bindings from the specified collection that match the specified request.
        /// </summary>
        /// <param name="bindings">The multimap of all registered bindings.</param>
        /// <param name="request">The request in question.</param>
        /// <returns>The series of matching bindings.</returns>
        IEnumerable<IBinding> Resolve(Multimap<Type, IBinding> bindings, IRequest request);
    }
}