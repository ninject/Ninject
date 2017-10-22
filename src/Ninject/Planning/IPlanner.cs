// -------------------------------------------------------------------------------------------------
// <copyright file="IPlanner.cs" company="Ninject Project Contributors">
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

    using Ninject.Components;
    using Ninject.Planning.Strategies;

    /// <summary>
    /// Generates plans for how to activate instances.
    /// </summary>
    public interface IPlanner : INinjectComponent
    {
        /// <summary>
        /// Gets the strategies that contribute to the planning process.
        /// </summary>
        IList<IPlanningStrategy> Strategies { get; }

        /// <summary>
        /// Gets or creates an activation plan for the specified type.
        /// </summary>
        /// <param name="type">The type for which a plan should be created.</param>
        /// <returns>The type's activation plan.</returns>
        IPlan GetPlan(Type type);
    }
}