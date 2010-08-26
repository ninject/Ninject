namespace Ninject.Activation.Caching
{
    using Ninject.Components;

    /// <summary>
    /// Stores the objects that were activated
    /// </summary>
    public interface IActivationCache : INinjectComponent
    {
        /// <summary>
        /// Clears the cache.
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds an activated instance.
        /// </summary>
        /// <param name="instance">The instance to be added.</param>
        void AddActivatedInstance(object instance);

        /// <summary>
        /// Adds an deactivated instance.
        /// </summary>
        /// <param name="instance">The instance to be added.</param>
        void AddDeactivatedInstance(object instance);
        
        /// <summary>
        /// Determines whether the specified instance is activated.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified instance is activated; otherwise, <c>false</c>.
        /// </returns>
        bool IsActivated(object instance);

        /// <summary>
        /// Determines whether the specified instance is deactivated.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified instance is deactivated; otherwise, <c>false</c>.
        /// </returns>
        bool IsDeactivated(object instance);
    }
}