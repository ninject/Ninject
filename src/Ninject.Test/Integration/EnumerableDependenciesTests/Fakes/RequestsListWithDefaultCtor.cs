namespace Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes
{
    using System.Collections.Generic;

    public class RequestsListWithDefaultCtor : RequestsList
    {
        public RequestsListWithDefaultCtor()
            : base(new List<IChild>())
        {
        }

        public RequestsListWithDefaultCtor(List<IChild> children)
            : base(children)
        {
        }
    }
}