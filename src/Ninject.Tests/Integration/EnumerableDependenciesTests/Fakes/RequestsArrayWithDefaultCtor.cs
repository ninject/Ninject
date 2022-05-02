namespace Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes
{
    public class RequestsArrayWithDefaultCtor : RequestsArray
    {
        public RequestsArrayWithDefaultCtor()
            : base(new IChild[0])
        {
        }

        public RequestsArrayWithDefaultCtor(IChild[] children)
            : base(children)
        {
        }
    }
}