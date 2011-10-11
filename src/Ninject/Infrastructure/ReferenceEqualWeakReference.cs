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
    using System.Security;

    /// <summary>
    /// Weak reference that can be used in collections. It is equal to the
    /// object it references and has the same hash code.
    /// </summary>
    public class ReferenceEqualWeakReference
    {
        private int cashedHashCode;
        private WeakReference weakReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceEqualWeakReference"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public ReferenceEqualWeakReference(object target)
        {
            this.weakReference = new WeakReference(target);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceEqualWeakReference"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="trackResurrection">if set to <c>true</c> [track resurrection].</param>
        public ReferenceEqualWeakReference(object target, bool trackResurrection)
        {
            this.weakReference = new WeakReference(target, trackResurrection);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is alive.
        /// </summary>
        /// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
        public bool IsAlive
        {
            get
            {
                return this.weakReference.IsAlive;
            }
        }

        /// <summary>
        /// Gets or sets the target of this weak reference.
        /// </summary>
        /// <value>The target of this weak reference.</value>
        public object Target
        {
            get
            {
                return this.weakReference.Target;
            }

            set
            {
                this.weakReference.Target = value;
            }
        }
        
        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
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

            var referenceEqualWeakReference = obj as ReferenceEqualWeakReference;
            if (referenceEqualWeakReference != null)
            {
                obj = referenceEqualWeakReference.Target;

                if (obj == null)
                {
                    return false;
                }

            }
            else
            {
                var weakReference = obj as WeakReference;
                if (weakReference != null)
                {
                    obj = weakReference.Target;

                    if (obj == null)
                    {
                        return false;
                    }
                }
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
            var target = this.Target;
            if (target != null)
            {
                this.cashedHashCode = target.GetHashCode();
            }

            return this.cashedHashCode;
        }
    }
}