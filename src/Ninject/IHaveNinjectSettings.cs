namespace Ninject
{
    /// <summary>
    /// Provides access to Ninject settings.
    /// </summary>
    public interface IHaveNinjectSettings
    {
        /// <summary>
        /// Gets the kernel settings.
        /// </summary>
        INinjectSettings Settings { get; }    
    }
}