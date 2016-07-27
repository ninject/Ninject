//-------------------------------------------------------------------------------------------------
// <copyright file="BindingActionStrategy.cs" company="Ninject Project Contributors">
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
    using Ninject.Infrastructure.Language;

    /// <summary>
    /// Executes actions defined on the binding during activation and deactivation.
    /// </summary>
    public class BindingActionStrategy : ActivationStrategy
    {
        /// <summary>
        /// Calls the activation actions defined on the binding.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being activated.</param>
        public override void Activate(IContext context, InstanceReference reference)
        {
            Contract.Requires(context != null);
            Contract.Requires(reference != null);
            context.Binding.ActivationActions.Map(action => action?.Invoke(context, reference.Instance));
        }

        /// <summary>
        /// Calls the deactivation actions defined on the binding.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being deactivated.</param>
        public override void Deactivate(IContext context, InstanceReference reference)
        {
            Contract.Requires(context != null);
            Contract.Requires(reference != null);
            context.Binding.DeactivationActions.Map(action => action?.Invoke(context, reference.Instance));
        }
    }
}