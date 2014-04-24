namespace Ninject
{
    using System.Collections.Generic;

    using Ninject.Components;
    using Ninject.Planning.Bindings;

    public interface IBindingPrecedenceComparer : INinjectComponent, IComparer<IBinding>
    {
    }
}