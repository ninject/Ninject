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
using System.Security;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;
#endregion

namespace Ninject.Activation
{
    /// <summary>
    /// Holds an instance during activation or after it has been cached.
    /// </summary>
    public class InstanceReference
    {
        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        public object Instance { get; set; }

        /// <summary>
        /// Returns a value indicating whether the instance is of the specified type.
        /// </summary>
        /// <typeparam name="T">The type in question.</typeparam>
        /// <returns><see langword="True"/> if the instance is of the specified type, otherwise <see langword="false"/>.</returns>
      
        [SecuritySafeCritical]
        public bool Is<T>()
        {
#if !SILVERLIGHT && !WINDOWS_PHONE && !MONO && !PCL && !WINRT
            if (System.Runtime.Remoting.RemotingServices.IsTransparentProxy(Instance)
                && System.Runtime.Remoting.RemotingServices.GetRealProxy(Instance).GetType().Name == "RemotingProxy")
            {
// ReSharper disable UseIsOperator.1
// ReSharper disable PossibleMistakenCallToGetType.1
// ReSharper disable UseMethodIsInstanceOfType 
// Must call typeof(T).IsAssignableFrom(Instance.GetType()) to convert the TransparentProxy to the actual proxy type 
                return typeof(T).IsAssignableFrom(Instance.GetType());
// ReSharper restore UseMethodIsInstanceOfType
// ReSharper restore PossibleMistakenCallToGetType.1
// ReSharper restore UseIsOperator.1
            };
#endif

            return Instance is T;
        }

        /// <summary>
        /// Returns the instance as the specified type.
        /// </summary>
        /// <typeparam name="T">The requested type.</typeparam>
        /// <returns>The instance.</returns>
        public T As<T>()
        {
            return (T)Instance;
        }

        /// <summary>
        /// Executes the specified action if the instance if of the specified type.
        /// </summary>
        /// <typeparam name="T">The type in question.</typeparam>
        /// <param name="action">The action to execute.</param>
        public void IfInstanceIs<T>(Action<T> action)
        {
            if (this.Is<T>())
                action((T)Instance);
        }
    }
}