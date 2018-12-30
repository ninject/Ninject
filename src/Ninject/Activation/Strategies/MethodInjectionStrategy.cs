// -------------------------------------------------------------------------------------------------
// <copyright file="MethodInjectionStrategy.cs" company="Ninject Project Contributors">
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

namespace Ninject.Activation.Strategies
{
    using System;

    using Ninject.Infrastructure;
    using Ninject.Planning.Directives;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Injects methods on an instance during activation.
    /// </summary>
    public class MethodInjectionStrategy : ActivationStrategy
    {
        /// <summary>
        /// Injects values into the properties as described by <see cref="MethodInjectionDirective"/>s
        /// contained in the plan.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being activated.</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="reference"/> is <see langword="null"/>.</exception>
        public override void Activate(IContext context, InstanceReference reference)
        {
            Ensure.ArgumentNotNull(context, nameof(context));
            Ensure.ArgumentNotNull(reference, nameof(reference));

            foreach (var directive in context.Plan.GetAll<MethodInjectionDirective>())
            {
                directive.Injector(reference.Instance, GetMethodArguments(directive.Targets, context));
            }
        }

        private static object[] GetMethodArguments(ITarget[] targets, IContext context)
        {
            var arguments = new object[targets.Length];

            for (var i = 0; i < targets.Length; i++)
            {
                arguments[i] = targets[i].ResolveWithin(context);
            }

            return arguments;
        }
    }
}