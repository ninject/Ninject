namespace Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes
{
    using System.Collections.Generic;

    public class RequestsIList : IParent
    {
        public RequestsIList(IList<IChild> children)
        {
            this.Children = children;
        }

        public IList<IChild> Children { get; private set; }
    }
}