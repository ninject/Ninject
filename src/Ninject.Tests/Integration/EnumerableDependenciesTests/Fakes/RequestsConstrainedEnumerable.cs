namespace Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes
{
    using System.Collections.Generic;
    using System.Linq;

    public class RequestsConstrainedEnumerable : IParent
    {
        public RequestsConstrainedEnumerable([Named("bob")] IEnumerable<IChild> children)
        {
            this.Children = children.ToList();
        }

        public IList<IChild> Children { get; private set; }
    }
}