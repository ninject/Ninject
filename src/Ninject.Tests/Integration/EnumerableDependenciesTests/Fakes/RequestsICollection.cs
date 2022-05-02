namespace Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes
{
    using System.Collections.Generic;
    using System.Linq;

    public class RequestsICollection : IParent
    {
        public RequestsICollection(ICollection<IChild> children)
        {
            this.Children = children.ToList();
        }

        public IList<IChild> Children { get; private set; }
    }
}