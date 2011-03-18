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
#endregion

namespace Ninject
{
    /// <summary>
    /// Contains configuration options for Ninject.
    /// </summary>
    public interface INinjectSettings
    {
        /// <summary>
        /// Gets the attribute that indicates that a member should be injected.
        /// </summary>
        Type InjectAttribute { get; }

        /// <summary>
        /// Gets the interval at which the cache should be pruned.
        /// </summary>
        TimeSpan CachePruningInterval { get; }

        #if !NO_ASSEMBLY_SCANNING
        /// <summary>
        /// Gets a value indicating whether the kernel should automatically load extensions at startup.
        /// </summary>
        bool LoadExtensions { get; }

        /// <summary>
        /// Gets the paths that should be searched for extensions.
        /// </summary>
        string[] ExtensionSearchPatterns { get; }
        #endif //!NO_ASSEMBLY_SCANNING

        #if !NO_LCG
        /// <summary>
        /// Gets a value indicating whether Ninject should use reflection-based injection instead of
        /// the (usually faster) lightweight code generation system.
        /// </summary>
        bool UseReflectionBasedInjection { get; }
        #endif //!NO_LCG

        #if !SILVERLIGHT
        /// <summary>
        /// Gets a value indicating whether Ninject should inject non public members.
        /// </summary>
        bool InjectNonPublic { get; set; }

        /// <summary>
        /// Gets a value indicating whether Ninject should inject private properties of base classes.
        /// </summary>
        /// <remarks>
        /// Activating this setting has an impact on the performance. It is recomended not
        /// to use this feature and use constructor injection instead.
        /// </remarks>
        bool InjectParentPrivateProperties { get; set; }
        #endif //!SILVERLIGHT

        /// <summary>
        /// Gets or sets a value indicating whether the activation cache is disabled.
        /// If the activation cache is disabled less memory is used. But in some cases
        /// instances are activated or deactivated multiple times. e.g. in the following scenario:
        /// Bind{A}().ToSelf();
        /// Bind{IA}().ToMethod(ctx => kernel.Get{IA}();
        /// </summary>
        /// <value>
        ///     <c>true</c> if activation cache is disabled; otherwise, <c>false</c>.
        /// </value>
        bool ActivationCacheDisabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Null is a valid value for injection.
        /// By defuault this is disabled and whenever a provider returns null an exception is thrown.
        /// </summary>
        /// <value><c>true</c> if null is allowed as injected value otherwise false.</value>
        bool AllowNullInjection { get; set; }

        /// <summary>
        /// Gets the value for the specified key.
        /// </summary>
        /// <typeparam name="T">The type of value to return.</typeparam>
        /// <param name="key">The setting's key.</param>
        /// <param name="defaultValue">The value to return if no setting is available.</param>
        /// <returns>The value, or the default value if none was found.</returns>
        T Get<T>(string key, T defaultValue);

        /// <summary>
        /// Sets the value for the specified key.
        /// </summary>
        /// <param name="key">The setting's key.</param>
        /// <param name="value">The setting's value.</param>
        void Set(string key, object value);
    }
}
