namespace Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes
{
    using System.Collections.Generic;

    public class RequestsConstrainedIList : IParent
    {
        public RequestsConstrainedIList([Named("bob")] IList<IChild> children)
        {
            this.Children = children;
        }

        public IList<IChild> Children { get; private set; }
    }
}