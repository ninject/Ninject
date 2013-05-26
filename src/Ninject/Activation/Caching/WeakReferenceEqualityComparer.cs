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
        /// Returns if the specifed objects are equal.
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