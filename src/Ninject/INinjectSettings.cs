// -------------------------------------------------------------------------------------------------
// <copyright file="INinjectSettings.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2019 Ninject Project Contributors. All rights reserved.
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

        /// <summary>
        /// Gets the default scope callback.
        /// </summary>
        Func<IContext, object> DefaultScopeCallback { get; }

        /// <summary>
        /// Gets a value indicating whether the kernel should automatically load extensions at startup.
        /// </summary>
        bool LoadExtensions { get; }

        /// <summary>
        /// Gets the paths that should be searched for extensions.
        /// </summary>
        string[] ExtensionSearchPatterns { get; }

        /// <summary>
        /// Gets a value indicating whether Ninject should use reflection-based injection instead of
        /// the (usually faster) lightweight code generation system.
        /// </summary>
        bool UseReflectionBasedInjection { get; }

        /// <summary>
        /// Gets or sets a value indicating whether Ninject should inject non public members.
        /// </summary>
        [Obsolete("Injecting non public members is not recommended. Make the member(s) public instead.")]
        bool InjectNonPublic { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Ninject should inject private properties of base classes.
        /// </summary>
        /// <remarks>
        /// Activating this setting has an impact on the performance. It is recommended not
        /// to use this feature and use constructor injection instead.
        /// </remarks>
        [Obsolete("Injecting parent private properties is not recommended. Use constructor injection instead.")]
        bool InjectParentPrivateProperties { get; set; }

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
        bool ActivationCacheDisabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether <see langword="null"/> is a valid value for injection.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if <see langword="null"/> is allowed as injected value; otherwise, <see langword="false"/>.
        /// The default is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// When <see langword="false"/>, an <see cref="ActivationException"/> is thrown whenever a provider returns <see langword="null"/>.
        /// </remarks>
        bool AllowNullInjection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether method injection should enabled.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if method injection is enabled; otherwise, <see langword="false"/>. The default
        /// is <see langword="true"/>.
        /// </value>
        bool MethodInjection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether property injection should enabled.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if property injection is enabled; otherwise, <see langword="false"/>. The default
        /// is <see langword="true"/>.
        /// </value>
        bool PropertyInjection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the old (&lt;= 3.3.4) behavior of <see cref="IServiceProvider.GetService(Type)"/>
        /// should be used which thorows an exception if the requested service cannot be found. Note that the documentation
        /// of that method https://docs.microsoft.com/en-us/dotnet/api/system.iserviceprovider.getservice?view=netframework-4.6.2
        /// states that the method should return <see langword="null"/> if there is no such service.
        /// </summary>
        bool ThrowOnGetServiceNotFound { get; set; }
    }
}