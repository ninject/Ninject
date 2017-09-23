// -------------------------------------------------------------------------------------------------
// <copyright file="WeakReferenceEqualityComparer.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Activation.Caching
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Ninject.Infrastructure;

    /// <summary>
    /// Compares ReferenceEqualWeakReferences to objects
    /// </summary>
    public class WeakReferenceEqualityComparer : IEqualityComparer<object>
    {
        /// <summary>
        /// Returns if the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object.</param>
        /// <param name="y">The second object.</param>
        /// <returns>True if the objects are equal; otherwise false</returns>
        public new bool Equals(object x, object y)
        {
            return x.Equals(y);
        }

        /// <summary>
        /// Returns the hash code of the specified object.
        /// </summary>
        /// <param name="obj">The object for which the hash code is calculated.</param>
        /// <returns>The hash code of the specified object.</returns>
        public int GetHashCode(object obj)
        {
            var weakReference = obj as ReferenceEqualWeakReference;
            return weakReference != null ? weakReference.GetHashCode() : RuntimeHelpers.GetHashCode(obj);
        }
    }
}