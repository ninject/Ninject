namespace Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes
{
    using System.Collections.Generic;

    public class RequestsListWithConcreteClass
    {
        public RequestsListWithConcreteClass(IList<ChildA> children)
        {
            this.Children = children;
        }

        public IList<ChildA> Children { get; private set; }
    }
}