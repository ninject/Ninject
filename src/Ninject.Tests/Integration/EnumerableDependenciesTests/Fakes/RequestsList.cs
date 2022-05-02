namespace Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes
{
    using System.Collections.Generic;

    public class RequestsList : IParent
    {
        public RequestsList(List<IChild> children)
        {
            this.Children = children;
        }

        public IList<IChild> Children { get; private set; }
    }
}