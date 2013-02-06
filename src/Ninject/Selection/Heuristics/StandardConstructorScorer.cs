//-------------------------------------------------------------------------------
// <copyright file="StandardConstructorScorer.cs" company="Ninject Project Contributors">
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
//-------------------------------------------------------------------------------

namespace Ninject.Selection.Heuristics
{
    using System;
    using System.Collections;
    using System.Linq;

    using Ninject.Activation;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Language;
    using Ninject.Parameters;
    using Ninject.Planning.Directives;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Scores constructors by either looking for the existence of an injection marker
    /// attribute, or by counting the number of parameters.
    /// </summary>
    public class StandardConstructorScorer : NinjectComponent, IConstructorScorer
    {
        /// <summary>
        /// Gets the score for the specified constructor.
        /// </summary>
        /// <param name="context">The injection context.</param>
        /// <param name="directive">The constructor.</param>
        /// <returns>The constructor's score.</returns>
        public virtual int Score(IContext context, ConstructorInjectionDirective directive)
        {
            Ensure.ArgumentNotNull(context, "context");
            Ensure.ArgumentNotNull(directive, "constructor");

            if (directive.Constructor.HasAttribute(Settings.InjectAttribute))
            {
                return int.MaxValue;
            }

            var score = 1;
            foreach (ITarget target in directive.Targets)
            {
                if (ParameterExists(context, target))
                {
                    score++;
                    continue;
                }
                
                if (BindingExists(context, target))
                {
                    score++;
                    continue;
                }

                score++;
                if (score > 0)
                {
                    score += int.MinValue;
                }
            }
            
            return score;
        }

        /// <summary>
        /// Checkes whether a binding exists for a given target.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>Whether a binding exists for the target in the given context.</returns>
        protected virtual bool BindingExists(IContext context, ITarget target)
        {
			return this.BindingExists(context.Kernel, context, target);
		}

        /// <summary>
        /// Checkes whether a binding exists for a given target on the specified kernel.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>Whether a binding exists for the target in the given context.</returns>
        protected virtual bool BindingExists(IKernel kernel, IContext context, ITarget target)
        {
            var targetType = GetTargetType(target);
            return kernel.GetBindings(targetType).Any(b => !b.IsImplicit)
                   || target.HasDefaultValue;
        }

        private Type GetTargetType(ITarget target)
        {
            var targetType = target.Type;
            if (targetType.IsArray)
            {
                targetType = targetType.GetElementType();
            }

            if (targetType.IsGenericType && targetType.GetInterfaces().Any(type => type == typeof(IEnumerable)))
            {
                targetType = targetType.GetGenericArguments()[0];
            }

            return targetType;
        }

        /// <summary>
        /// Checks whether any parameters exist for the geiven target..
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>Whether a parameter exists for the target in the given context.</returns>
        protected virtual bool ParameterExists(IContext context, ITarget target)
        {
            return context
                .Parameters.OfType<IConstructorArgument>()
                .Any(parameter => parameter.AppliesToTarget(context, target));
        }
    }
}
