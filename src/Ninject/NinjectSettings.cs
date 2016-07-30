//-------------------------------------------------------------------------------------------------
// <copyright file="NinjectSettings.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2009, Enkari, Ltd.
//   Copyright (c) 2009-2011 Ninject Project Contributors
//   Authors: Nate Kohari (nate@enkari.com)
//            Remo Gloor (remo.gloor@gmail.com)
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   you may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------------------------
namespace Ninject
{
    using System;
    using System.Collections.Generic;
    using Ninject.Activation;
    using Ninject.Infrastructure;

    /// <summary>
    /// Contains configuration options for Ninject.
    /// </summary>
    public class NinjectSettings : INinjectSettings
    {
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the attribute that indicates that a member should be injected.
        /// </summary>
        public Type InjectAttribute
        {
            get { return this.Get(nameof(this.InjectAttribute), typeof(InjectAttribute)); }
            set { this.Set(nameof(this.InjectAttribute), value); }
        }

        /// <summary>
        /// Gets or sets the interval at which the GC should be polled.
        /// </summary>
        public TimeSpan CachePruningInterval
        {
            get { return this.Get(nameof(this.CachePruningInterval), TimeSpan.FromSeconds(30)); }
            set { this.Set(nameof(this.CachePruningInterval), value); }
        }

        /// <summary>
        /// Gets or sets the default scope callback.
        /// </summary>
        public Func<IContext, object> DefaultScopeCallback
        {
            get { return this.Get(nameof(this.DefaultScopeCallback), StandardScopeCallbacks.Transient); }
            set { this.Set(nameof(this.DefaultScopeCallback), value); }
        }

#if !NO_ASSEMBLY_SCANNING
        /// <summary>
        /// Gets or sets a value indicating whether the kernel should automatically load extensions at startup.
        /// </summary>
        public bool LoadExtensions
        {
            get { return this.Get(nameof(this.LoadExtensions), true); }
            set { this.Set(nameof(this.LoadExtensions), value); }
        }

        /// <summary>
        /// Gets or sets the paths that should be searched for extensions.
        /// </summary>
        public string[] ExtensionSearchPatterns
        {
            get { return this.Get(nameof(this.ExtensionSearchPatterns), new[] { "Ninject.Extensions.*.dll", "Ninject.Web*.dll" }); }
            set { this.Set(nameof(this.ExtensionSearchPatterns), value); }
        }
#endif //!NO_ASSEMBLY_SCANNING

#if !NO_LCG
        /// <summary>
        /// Gets or sets a value indicating whether Ninject should use reflection-based injection instead of
        /// the (usually faster) lightweight code generation system.
        /// </summary>
        public bool UseReflectionBasedInjection
        {
            get { return this.Get(nameof(this.UseReflectionBasedInjection), false); }
            set { this.Set(nameof(this.UseReflectionBasedInjection), value); }
        }
#endif //!NO_LCG

        /// <summary>
        /// Gets or sets a value indicating whether Ninject should inject non public members.
        /// </summary>
        public bool InjectNonPublic
        {
            get { return this.Get(nameof(this.InjectNonPublic), false); }
            set { this.Set(nameof(this.InjectNonPublic), value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Ninject should inject private properties of base classes.
        /// </summary>
        /// <remarks>
        /// Activating this setting has an impact on the performance. It is recommended not
        /// to use this feature and use constructor injection instead.
        /// </remarks>
        public bool InjectParentPrivateProperties
        {
            get { return this.Get(nameof(this.InjectParentPrivateProperties), false); }
            set { this.Set(nameof(this.InjectParentPrivateProperties), value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the activation cache is disabled.
        /// If the activation cache is disabled less memory is used. But in some cases
        /// instances are activated or deactivated multiple times. e.g. in the following scenario:
        /// Bind{A}().ToSelf();
        /// Bind{IA}().ToMethod(ctx =&gt; kernel.Get{IA}();
        /// </summary>
        /// <value>
        /// <c>true</c> if activation cache is disabled; otherwise, <c>false</c>.
        /// </value>
        public bool ActivationCacheDisabled
        {
            get { return this.Get(nameof(this.ActivationCacheDisabled), false); }
            set { this.Set(nameof(this.ActivationCacheDisabled), value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Null is a valid value for injection.
        /// By default this is disabled and whenever a provider returns null an exception is thrown.
        /// </summary>
        /// <value>
        /// <c>true</c> if null is allowed as injected value otherwise false.
        /// </value>
        public bool AllowNullInjection
        {
            get { return this.Get(nameof(this.AllowNullInjection), false); }
            set { this.Set(nameof(this.AllowNullInjection), value); }
        }

        /// <summary>
        /// Gets the value for the specified key.
        /// </summary>
        /// <typeparam name="T">The type of value to return.</typeparam>
        /// <param name="key">The setting's key.</param>
        /// <param name="defaultValue">The value to return if no setting is available.</param>
        /// <returns>The value, or the default value if none was found.</returns>
        public T Get<T>(string key, T defaultValue)
        {
            object value;
            return this.values.TryGetValue(key, out value) ? (T)value : defaultValue;
        }

        /// <summary>
        /// Sets the value for the specified key.
        /// </summary>
        /// <param name="key">The setting's key.</param>
        /// <param name="value">The setting's value.</param>
        public void Set(string key, object value)
        {
            this.values[key] = value;
        }
    }
}