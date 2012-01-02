namespace Ninject.Tests.Integration.SpecialResolutionTests
{
    using System;

    using FluentAssertions;
    using Ninject.Syntax;
    using Xunit;

    public class SpecialResolutionContext : IDisposable
    {
        protected StandardKernel kernel;

        public SpecialResolutionContext()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }
    }

    public class WhenServiceRequestsKernel : SpecialResolutionContext
    {
        [Fact]
        public void InstanceOfKernelIsInjected()
        {
            kernel.Bind<RequestsKernel>().ToSelf();
            var instance = kernel.Get<RequestsKernel>();

            instance.Should().NotBeNull();
            instance.Kernel.Should().NotBeNull();
            instance.Kernel.Should().BeSameAs(kernel);
        }
    }

    public class WhenServiceRequestsResolutionRoot : SpecialResolutionContext
    {
        [Fact]
        public void InstanceOfKernelIsInjected()
        {
            kernel.Bind<RequestsResolutionRoot>().ToSelf();
            var instance = kernel.Get<RequestsResolutionRoot>();

            instance.Should().NotBeNull();
            instance.ResolutionRoot.Should().NotBeNull();
            instance.ResolutionRoot.Should().BeSameAs(kernel);
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