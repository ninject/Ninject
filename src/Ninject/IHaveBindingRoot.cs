using Ninject.Syntax;

namespace Ninject
{
    /// <summary>
    /// Indicates the object has a reference to a <see cref="IBindingRoot"/>.
    /// </summary>
    public interface IHaveBindingRoot
    {
        /// <summary>
        /// Gets the associated binding root.
        /// </summary>
        IBindingRoot BindingRoot { get; }
    }
}