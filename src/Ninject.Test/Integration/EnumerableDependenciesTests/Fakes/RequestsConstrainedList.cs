namespace Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes
{
    using System.Collections.Generic;

    public class RequestsConstrainedList : IParent
    {
        public RequestsConstrainedList([Named("bob")] List<IChild> children)
        {
            this.Children = children;
        }

        public IList<IChild> Children { get; private set; }
    }
}