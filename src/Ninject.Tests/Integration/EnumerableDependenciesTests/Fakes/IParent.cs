namespace Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes
{
    using System.Collections.Generic;

    public interface IParent
    {
        IList<IChild> Children { get; }
    }
}