// -------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsForParameter.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2020 Ninject Project Contributors. All rights reserved.
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

namespace Ninject.Parameters
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods for <see cref="IParameter"/>.
    /// </summary>
    internal static class ExtensionsForParameter
    {
        /// <summary>
        /// Returns only parameters that should be inherited from the specified list of <see cref="IParameter"/>.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// The list of <see cref="IParameter"/> that should be inherited.
        /// </returns>
        /// <remarks>
        /// Since this is an extension for internal use only, we do not perform an argument (null) check.
        /// This was done for performance reasons.
        /// </remarks>
        internal static IReadOnlyList<IParameter> GetShouldInheritParameters(this IReadOnlyList<IParameter> parameters)
        {
            if (parameters.Count == 0)
            {
                return Array.Empty<IParameter>();
            }

            List<IParameter> inherit = new List<IParameter>();

            foreach (var param in parameters)
            {
                if (param.ShouldInherit)
                {
                    inherit.Add(param);
                }
            }

            return inherit;
        }
    }
}