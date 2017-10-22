// -------------------------------------------------------------------------------------------------
// <copyright file="Planner.cs" company="Ninject Project Contributors">
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
    using System.Threading;

    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Language;
    using Ninject.Planning.Strategies;

    /// <summary>
    /// Generates plans for how to activate instances.
    /// </summary>
    public class Planner : NinjectComponent, IPlanner
    {
        private readonly ReaderWriterLockSlim plannerLock = new ReaderWriterLockSlim();
        private readonly Dictionary<Type, IPlan> plans = new Dictionary<Type, IPlan>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Planner"/> class.
        /// </summary>
        /// <param name="strategies">The strategies to execute during planning.</param>
        public Planner(IEnumerable<IPlanningStrategy> strategies)
        {
            Ensure.ArgumentNotNull(strategies, "strategies");

            this.Strategies = strategies.ToList();
        }

        /// <summary>
        /// Gets the strategies that contribute to the planning process.
        /// </summary>
        public IList<IPlanningStrategy> Strategies { get; private set; }

        /// <summary>
        /// Gets or creates an activation plan for the specified type.
        /// </summary>
        /// <param name="type">The type for which a plan should be created.</param>
        /// <returns>The type's activation plan.</returns>
        public IPlan GetPlan(Type type)
        {
            Ensure.ArgumentNotNull(type, "type");

            this.plannerLock.EnterUpgradeableReadLock();

            try
            {
                return this.plans.TryGetValue(type, out IPlan plan) ? plan : this.CreateNewPlan(type);
            }
            finally
            {
                this.plannerLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Creates an empty plan for the specified type.
        /// </summary>
        /// <param name="type">The type for which a plan should be created.</param>
        /// <returns>The created plan.</returns>
        protected virtual IPlan CreateEmptyPlan(Type type)
        {
            Ensure.ArgumentNotNull(type, "type");

            return new Plan(type);
        }

        /// <summary>
        /// Creates a new plan for the specified type.
        /// This method requires an active reader lock!
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The newly created plan.</returns>
        private IPlan CreateNewPlan(Type type)
        {
            this.plannerLock.EnterWriteLock();

            try
            {
                if (this.plans.TryGetValue(type, out IPlan plan))
                {
                    return plan;
                }

                plan = this.CreateEmptyPlan(type);
                this.plans.Add(type, plan);
                this.Strategies.Map(s => s.Execute(plan));

                return plan;
            }
            finally
            {
                this.plannerLock.ExitWriteLock();
            }
        }
    }
}