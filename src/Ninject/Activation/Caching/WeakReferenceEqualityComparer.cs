namespace Ninject.Activation.Caching
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    using Ninject.Infrastructure;

    public class WeakReferenceEqualityComparer : IEqualityComparer<object>
    {
        public new bool Equals(object x, object y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(object obj)
        {
            var weakReference = obj as ReferenceEqualWeakReference;
            return weakReference != null ? weakReference.GetHashCode() : RuntimeHelpers.GetHashCode(obj);
        }
    }
}