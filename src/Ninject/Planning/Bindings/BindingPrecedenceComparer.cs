// -------------------------------------------------------------------------------------------------
// <copyright file="BindingPrecedenceComparer.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Planning.Bindings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ninject.Components;

    /// <summary>
    /// Implements the binding precedence comparer interface
    /// </summary>
    public class BindingPrecedenceComparer : NinjectComponent, IBindingPrecedenceComparer
    {
        /// <summary>
        /// Compares the two bindings.
        /// </summary>
        /// <param name="x">The first binding.</param>
        /// <param name="y">The second binding.</param>
        /// <returns>Less than zero if x is less than y; Zero is x equals y; Greater than zero if x is greater than y.</returns>
        public int Compare(IBinding x, IBinding y)
        {
            if (x == y)
            {
                return 0;
            }

            // Each function represents a level of precedence.
            var funcs = new List<Func<IBinding, bool>>
                            {
                                b => b != null,       // null bindings should never happen, but just in case
                                b => b.IsConditional, // conditional bindings > unconditional
                                b => !b.Service.ContainsGenericParameters, // closed generics > open generics
                                b => !b.IsImplicit,   // explicit bindings > implicit
                            };

            var q = from func in funcs
                    let xVal = func(x)
                    where xVal != func(y)
                    select xVal ? 1 : -1;

            // returns the value of the first function that represents a difference
            // between the bindings, or else returns 0 (equal)
            return q.FirstOrDefault();
        }
    }
}