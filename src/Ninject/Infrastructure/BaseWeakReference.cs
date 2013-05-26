namespace Ninject.Infrastructure
{
    using System;

    /// <summary>
    /// Inheritable weak reference base class for Silverlight
    /// </summary>
    public abstract class BaseWeakReference
    {
        private readonly WeakReference innerWeakReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceEqualWeakReference"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        protected BaseWeakReference(object target)
        {
            this.innerWeakReference = new WeakReference(target);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceEqualWeakReference"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="trackResurrection">if set to <c>true</c> [track resurrection].</param>
        protected BaseWeakReference(object target, bool trackResurrection)
        {
            this.innerWeakReference = new WeakReference(target, trackResurrection);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is alive.
        /// </summary>
        /// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
        public bool IsAlive
        {
            get
            {
                return this.innerWeakReference.IsAlive;
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
                return this.innerWeakReference.Target;
            }

            set
            {
                this.innerWeakReference.Target = value;
            }
        }
    }
}