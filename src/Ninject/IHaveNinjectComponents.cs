using Ninject.Components;

namespace Ninject
{
    /// <summary>
    /// Provides access to Ninject components.
    /// </summary>
    public interface IHaveNinjectComponents
    {
        /// <summary>
        /// Gets the component container, which holds components that contribute to Ninject.
        /// </summary>
        IComponentContainer Components { get; }        
    }
}