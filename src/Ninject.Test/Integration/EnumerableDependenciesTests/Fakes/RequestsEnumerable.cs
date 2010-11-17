namespace Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes
{
    using System.Collections.Generic;
    using System.Linq;

    public class RequestsEnumerable : IParent
    {
        public RequestsEnumerable(IEnumerable<IChild> children)
        {
            this.Children = children.ToList();
        }

        public IList<IChild> Children { get; private set; }
    }
}