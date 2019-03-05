namespace Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes
{
    using System.Collections.Generic;
    using System.Linq;

    public class RequestsConstrainedICollection : IParent
    {
        public RequestsConstrainedICollection([Named("bob")] ICollection<IChild> children)
        {
            this.Children = children.ToList();
        }

        public IList<IChild> Children { get; private set; }
    }
}