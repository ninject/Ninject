namespace Ninject.Activation.Caching
{
    /// <summary>
    /// An object that is pruneable.
    /// </summary>
    public interface IPruneable
    {
        /// <summary>
        /// Removes instances from the cache which should no longer be re-used.
        /// </summary>
        void Prune();
    }
}