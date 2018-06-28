namespace Ninject.Tests.Integration.SpecialResolutionTests
{
    using System;

    using FluentAssertions;
    using Ninject.Syntax;
    using Xunit;

    public class SpecialResolutionContext : IDisposable
    {
        protected IKernelConfiguration kernelConfiguration;

        public SpecialResolutionContext()
        {
            this.kernelConfiguration = new KernelConfiguration();
        }

        public void Dispose()
        {
            this.kernelConfiguration.Dispose();
        }
    }

    public class WhenServiceRequestsKernel : SpecialResolutionContext
    {
        [Fact]
        public void InstanceOfKernelIsInjected()
        {
            this.kernelConfiguration.Bind<RequestsKernel>().ToSelf();
            var kernel = this.kernelConfiguration.BuildReadOnlyKernel();

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
            this.kernelConfiguration.Bind<RequestsResolutionRoot>().ToSelf();
            var kernel = this.kernelConfiguration.BuildReadOnlyKernel();

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
            this.kernelConfiguration.Bind<RequestsString>().ToSelf();
            var kernel = this.kernelConfiguration.BuildReadOnlyKernel();

            Assert.Throws<ActivationException>(() => kernel.Get<RequestsString>());
        }
    }

    public class RequestsKernel
    {
        public IReadOnlyKernel Kernel { get; set; }

        public RequestsKernel(IReadOnlyKernel kernel)
        {
            this.Kernel = kernel;
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
            this.StringValue = stringValue;
        }
    }
}