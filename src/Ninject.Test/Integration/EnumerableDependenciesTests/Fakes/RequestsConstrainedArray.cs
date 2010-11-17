namespace Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes
{
    using System.Collections.Generic;

    public class RequestsConstrainedArray : IParent
    {
        public RequestsConstrainedArray([Named("bob")] IChild[] children)
        {
            this.Children = children;
        }

        public IList<IChild> Children { get; private set; }
    }
}