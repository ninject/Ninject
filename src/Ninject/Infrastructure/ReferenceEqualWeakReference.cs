#region License
// 
// Author: Remo Gloor (remo.gloor@bbv.ch)
// Copyright (c) 2010, bbv Software Services AG
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion
namespace Ninject.Infrastructure
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Weak reference that can be used in collections. It is equal to the
    /// object it references and has the same hash code.
    /// </summary>
    public class ReferenceEqualWeakReference : WeakReference
    {
        private int cashedHashCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceEqualWeakReference"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public ReferenceEqualWeakReference(object target)
            : base(target)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceEqualWeakReference"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="trackResurrection">if set to <c>true</c> [track resurrection].</param>
        public ReferenceEqualWeakReference(object target, bool trackResurrection)
            : base(target, trackResurrection)
        {
        }

#if !NO_EXCEPTION_SERIALIZATION
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceEqualWeakReference"/> class.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize the current <see cref="T:System.WeakReference"/> object.</param>
        /// <param name="context">(Reserved) Describes the source and destination of the serialized stream specified by <paramref name="info"/>.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="info"/> is null.
        /// </exception>
        protected ReferenceEqualWeakReference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (!this.IsAlive)
            {
                return base.Equals(obj);
            }

            var weakReference = obj as WeakReference;
            if (weakReference != null)
            {
                if (!weakReference.IsAlive)
                {
                    return false;
                }

                obj = weakReference.Target;
            }

            return ReferenceEquals(this.Target, obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (this.IsAlive)
            {
                this.cashedHashCode = this.Target.GetHashCode();
            }

            return this.cashedHashCode;
        }
    }
}