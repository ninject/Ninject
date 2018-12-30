// -------------------------------------------------------------------------------------------------
// <copyright file="Plan.cs" company="Ninject Project Contributors">
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

namespace Ninject.Planning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ninject.Infrastructure;
    using Ninject.Planning.Directives;

    /// <summary>
    /// Describes the means by which a type should be activated.
    /// </summary>
    public class Plan : IPlan
    {
        private readonly List<IDirective> directives;

        /// <summary>
        /// Initializes a new instance of the <see cref="Plan"/> class.
        /// </summary>
        /// <param name="type">The type the plan describes.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
        public Plan(Type type)
        {
            Ensure.ArgumentNotNull(type, nameof(type));

            this.Type = type;
            this.directives = new List<IDirective>();
        }

        /// <summary>
        /// Gets the type that the plan describes.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the directives defined in the plan.
        /// </summary>
        public ICollection<IDirective> Directives
        {
            get { return this.directives; }
        }

        /// <summary>
        /// Adds the specified directive to the plan.
        /// </summary>
        /// <param name="directive">The directive.</param>
        /// <exception cref="ArgumentNullException"><paramref name="directive"/> is <see langword="null"/>.</exception>
        public void Add(IDirective directive)
        {
            Ensure.ArgumentNotNull(directive, nameof(directive));

            this.directives.Add(directive);
        }

        /// <summary>
        /// Determines whether the plan contains one or more directives of the specified type.
        /// </summary>
        /// <typeparam name="TDirective">The type of directive.</typeparam>
        /// <returns>
        /// <see langword="true"/> if the plan has one or more directives of the type; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Has<TDirective>()
            where TDirective : IDirective
        {
            return this.GetAll<TDirective>().Any();
        }

        /// <summary>
        /// Gets the first directive of the specified type from the plan.
        /// </summary>
        /// <typeparam name="TDirective">The type of directive.</typeparam>
        /// <returns>
        /// The first directive, or <see langword="null"/> if no matching directives exist.
        /// </returns>
        public TDirective GetOne<TDirective>()
            where TDirective : IDirective
        {
            return this.GetAll<TDirective>().SingleOrDefault();
        }

        /// <summary>
        /// Gets all directives of the specified type that exist in the plan.
        /// </summary>
        /// <typeparam name="TDirective">The type of directive.</typeparam>
        /// <returns>
        /// A series of directives of the specified type.
        /// </returns>
        public IEnumerable<TDirective> GetAll<TDirective>()
            where TDirective : IDirective
        {
            var directiveCount = this.directives.Count;
            for (var i = 0; i < directiveCount; i++)
            {
                if (this.directives[i] is TDirective tdir)
                {
                    yield return tdir;
                }
            }
        }
    }
}