//-------------------------------------------------------------------------------------------------
// <copyright file="MethodInjectionStrategy.cs" company="Ninject Project Contributors">
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
namespace Ninject.Activation.Strategies
{
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Ninject.Planning.Directives;

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
        public override void Activate(IContext context, InstanceReference reference)
        {
            Contract.Requires(context != null);
            Contract.Requires(reference != null);

            foreach (var directive in context.Plan.GetAll<MethodInjectionDirective>())
            {
                var arguments = directive.Targets.Select(target => target.ResolveWithin(context));
                directive.Injector(reference.Instance, arguments.ToArray());
            }
        }
    }
}