namespace Ninject
{
    using System.Collections.Generic;

    using Ninject.Components;
    using Ninject.Planning.Bindings;

    /// <summary>
    /// The binding precedence comparer interface
    /// </summary>
    public interface IBindingPrecedenceComparer : INinjectComponent, IComparer<IBinding>
    {
    }
}