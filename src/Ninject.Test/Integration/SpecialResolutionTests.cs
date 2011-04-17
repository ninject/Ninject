namespace Ninject.Tests.Integration.SpecialResolutionTests
{
    using Ninject.Syntax;
    using Xunit;
    using Xunit.Should;

    public class SpecialResolutionContext
    {
        protected StandardKernel kernel;

        public SpecialResolutionContext()
        {
            this.kernel = new StandardKernel();
        }
    }

    public class WhenServiceRequestsKernel : SpecialResolutionContext
    {
        [Fact]
        public void InstanceOfKernelIsInjected()
        {
            kernel.Bind<RequestsKernel>().ToSelf();
            var instance = kernel.Get<RequestsKernel>();

            instance.ShouldNotBeNull();
            instance.Kernel.ShouldNotBeNull();
            instance.Kernel.ShouldBeSameAs(kernel);
        }
    }

    public class WhenServiceRequestsResolutionRoot : SpecialResolutionContext
    {
        [Fact]
        public void InstanceOfKernelIsInjected()
        {
            kernel.Bind<RequestsResolutionRoot>().ToSelf();
            var instance = kernel.Get<RequestsResolutionRoot>();

            instance.ShouldNotBeNull();
            instance.ResolutionRoot.ShouldNotBeNull();
            instance.ResolutionRoot.ShouldBeSameAs(kernel);
        }
    }

    public class WhenServiceRequestsString : SpecialResolutionContext
    {
        [Fact]
        public void InstanceOfStringIsInjected()
        {
            kernel.Bind<RequestsString>().ToSelf();
            Assert.Throws<ActivationException>(() => kernel.Get<RequestsString>());
        }
    }

    public class RequestsKernel
    {
        public IKernel Kernel { get; set; }

        public RequestsKernel(IKernel kernel)
        {
            Kernel = kernel;
        }
    }

    public class RequestsResolutionRoot
    {
        public IResolutionRoot ResolutionRoot { get; set; }

        public RequestsResolutionRoot(IResolutionRoot resolutionRoot)
        {
            this.ResolutionRoot = resolutionRoot;
        }
    }

    public class RequestsString
    {
        public string StringValue { get; set; }

        public RequestsString(string stringValue)
        {
            StringValue = stringValue;
        }
    }
}