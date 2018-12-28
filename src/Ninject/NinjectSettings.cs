// -------------------------------------------------------------------------------------------------
// <copyright file="NinjectSettings.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2017 Ninject Project Contributors. All rights reserved.
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   You may not use this file except in compliance with one of the Licenses.
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
// -------------------------------------------------------------------------------------------------

namespace Ninject
{
    using System;

    using Ninject.Activation;
    using Ninject.Infrastructure;

    /// <summary>
    /// Contains configuration options for Ninject.
    /// </summary>
    public class NinjectSettings : INinjectSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectSettings"/> class.
        /// </summary>
        public NinjectSettings()
        {
            this.InjectAttribute = typeof(InjectAttribute);
            this.CachePruningInterval = TimeSpan.FromSeconds(30);
            this.DefaultScopeCallback = StandardScopeCallbacks.Transient;
            this.LoadExtensions = true;
            this.ExtensionSearchPatterns = new[] { "Ninject.Extensions.*.dll", "Ninject.Web*.dll" };
        }

        /// <summary>
        /// Gets or sets the attribute that indicates that a member should be injected.
        /// </summary>
        /// <value>
        /// The type of the attribute that indicates that a member should be injected. The default
        /// is <see cref="InjectAttribute"/>.
        /// </value>
        public Type InjectAttribute { get; set; }

        /// <summary>
        /// Gets or sets the interval at which the GC should be polled.
        /// </summary>
        /// <value>
        /// The interval at which the GC should be polled. The default is <c>30</c> seconds.
        /// </value>
        public TimeSpan CachePruningInterval { get; set; }

        /// <summary>
        /// Gets or sets the default scope callback.
        /// </summary>
        public Func<IContext, object> DefaultScopeCallback { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the kernel should automatically load extensions at startup.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the kernel should automatically load extensions at startup; otherwise, <see langword="false"/>.
        /// The default is <see langword="true"/>.
        /// </value>
        public bool LoadExtensions { get; set; }

        /// <summary>
        /// Gets or sets the paths that should be searched for extensions.
        /// </summary>
        /// <value>
        /// The paths that should be searched for extensions. The default is &quot;Ninject.Extensions.*.dll&quot; and
        /// &quot;Ninject.Web*.dll&quot;.
        /// </value>
        public string[] ExtensionSearchPatterns { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Ninject should use reflection-based injection instead of
        /// the (usually faster) Expression build system.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if Ninject should use reflection-based injection; otherwise, <see langword="false"/>.
        /// The default is <see langword="false"/>.
        /// </value>
        public bool UseReflectionBasedInjection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Ninject should inject non public members.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if Ninject should inject non-public members; otherwise, <see langword="false"/>.
        /// The default is <see langword="false"/>.
        /// </value>
        [Obsolete("Injecting non public members is not recommended. Make the member(s) public instead.")]
        public bool InjectNonPublic { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Ninject should inject private properties of base classes.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if Ninject should inject into private properties of base classes; otherwise, <see langword="false"/>.
        /// The default is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// Activating this setting has an impact on the performance. It is recommended not
        /// to use this feature and use constructor injection instead.
        /// </remarks>
        [Obsolete("Injecting parent private properties is not recommended. Use constructor injection instead.")]
        public bool InjectParentPrivateProperties { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the activation cache is disabled.
        /// </summary>
        /// <remarks>
        /// If the activation cache is disabled less memory is used. But in some cases
        /// instances are activated or deactivated multiple times. e.g. in the following scenario:
        /// <code>
        /// Bind{A}().ToSelf();
        /// Bind{IA}().ToMethod(ctx => kernel.Get{IA}();
        /// </code>
        /// </remarks>
        /// <value>
        /// <see langword="true"/> if activation cache is disabled; otherwise, <see langword="false"/>.
        /// The default is <see langword="false"/>.
        /// </value>
        public bool ActivationCacheDisabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether <see langword="null"/> is a valid value for injection.
        /// By default this is disabled and whenever a provider returns null an exception is thrown.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if <see langword="null"/> is allowed as injected value; otherwise, <see langword="false"/>.
        /// The default is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// When <see langword="false"/>, an <see cref="ActivationException"/> is thrown whenever a provider returns <see langword="null"/>.
        /// </remarks>
        public bool AllowNullInjection { get; set; }
    }
}