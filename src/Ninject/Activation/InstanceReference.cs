// -------------------------------------------------------------------------------------------------
// <copyright file="InstanceReference.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Activation
{
    using System;
    using System.Security;

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
#if !NO_REMOTING
            if (System.Runtime.Remoting.RemotingServices.IsTransparentProxy(this.Instance)
                && System.Runtime.Remoting.RemotingServices.GetRealProxy(this.Instance).GetType().Name == "RemotingProxy")
            {
                return typeof(T).IsAssignableFrom(this.Instance.GetType());
            }
#endif
            return this.Instance is T;
        }

        /// <summary>
        /// Returns the instance as the specified type.
        /// </summary>
        /// <typeparam name="T">The requested type.</typeparam>
        /// <returns>The instance.</returns>
        public T As<T>()
        {
            return (T)this.Instance;
        }

        /// <summary>
        /// Executes the specified action if the instance if of the specified type.
        /// </summary>
        /// <typeparam name="T">The type in question.</typeparam>
        /// <param name="action">The action to execute.</param>
        public void IfInstanceIs<T>(Action<T> action)
        {
            if (this.Is<T>())
            {
                action((T)this.Instance);
            }
        }
    }
}