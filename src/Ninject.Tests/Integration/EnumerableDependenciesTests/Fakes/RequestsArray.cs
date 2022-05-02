namespace Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes
{
    using System.Collections.Generic;

    public class RequestsArray : IParent
    {
        public RequestsArray(IChild[] children)
        {
            this.Children = children;
        }

        public IList<IChild> Children { get; private set; }
    }
}