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
using Ninject.Infrastructure;
using Ninject.Infrastructure.Disposal;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Syntax;
#endregion

namespace Ninject.Activation.Blocks
{
    /// <summary>
    /// A block used for deterministic disposal of activated instances. When the block is
    /// disposed, all instances activated via it will be deactivated.
    /// </summary>
    public class ActivationBlock : DisposableObject, IActivationBlock
    {
        /// <summary>
        /// Gets or sets the parent resolution root (usually the kernel).
        /// </summary>
        public IResolutionRoot Parent { get; private set; }

        /// <summary>
        /// Occurs when the object is disposed.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationBlock"/> class.
        /// </summary>
        /// <param name="parent">The parent resolution root.</param>
        public ActivationBlock(IResolutionRoot parent)
        {
            Contract.Requires(parent != null);
            Parent = parent;
        }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        /// <param name="disposing">A Boolean indicating whether release managed resource or not.</param>
        public override void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing && !IsDisposed)
                {
                    Disposed?.Invoke(this, EventArgs.Empty);
                    Disposed = null;
                }

                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns><c>True</c> if the request can be resolved; otherwise, <c>false</c>.</returns>
        public bool CanResolve(IRequest request)
        {
            Contract.Requires(request != null);
            return this.Parent.CanResolve(request);
        }

        /// <summary>
        /// Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="ignoreImplicitBindings">if set to <c>true</c> implicit bindings are ignored.</param>
        /// <returns>
        ///     <c>True</c> if the request can be resolved; otherwise, <c>false</c>.
        /// </returns>
        public bool CanResolve(IRequest request, bool ignoreImplicitBindings)
        {
            Contract.Requires(request != null);
            return this.Parent.CanResolve(request, ignoreImplicitBindings);
        }

        /// <summary>
        /// Resolves instances for the specified request. The instances are not actually resolved
        /// until a consumer iterates over the enumerator.
        /// </summary>
        /// <param name="request">The request to resolve.</param>
        /// <returns>An enumerator of instances that match the request.</returns>
        public IEnumerable<object> Resolve(IRequest request)
        {
            Contract.Requires(request != null);
            return Parent.Resolve(request);
        }

        /// <summary>
        /// Creates a request for the specified service.
        /// </summary>
        /// <param name="service">The service that is being requested.</param>
        /// <param name="constraint">The constraint to apply to the bindings to determine if they match the request.</param>
        /// <param name="parameters">The parameters to pass to the resolution.</param>
        /// <param name="isOptional"><c>True</c> if the request is optional; otherwise, <c>false</c>.</param>
        /// <param name="isUnique"><c>True</c> if the request should return a unique result; otherwise, <c>false</c>.</param>
        /// <returns>The created request.</returns>
        public virtual IRequest CreateRequest(Type service, Predicate<IBindingMetadata> constraint, IEnumerable<IParameter> parameters, bool isOptional, bool isUnique)
        {
            Contract.Requires(service != null);
            Contract.Requires(parameters != null);
            return new Request(service, constraint, parameters, () => this, isOptional, isUnique);
        }

        /// <summary>
        /// Deactivates and releases the specified instance if it is currently managed by Ninject.
        /// </summary>
        /// <param name="instance">The instance to release.</param>
        /// <returns><see langword="True"/> if the instance was found and released; otherwise <see langword="false"/>.</returns>
        /// <remarks></remarks>
        public bool Release(object instance)
        {
            return Parent.Release(instance);
        }
    }
}