namespace Ninject.Tests.Integration.SpecialResolutionTests
{
    using Ninject.Syntax;
#if SILVERLIGHT
#if SILVERLIGHT_MSTEST
    using MsTest.Should;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Assert = AssertWithThrows;
    using Fact = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
#else
    using UnitDriven;
    using UnitDriven.Should;
    using Assert = AssertWithThrows;
    using Fact = UnitDriven.TestMethodAttribute;
#endif
#else
    using Ninject.Tests.MSTestAttributes;
    using Xunit;
    using Xunit.Should;
#endif

    public class SpecialResolutionContext
    {
        protected StandardKernel kernel;

        public SpecialResolutionContext()
        {
            this.SetUp();
        }

        [TestInitialize]
        public void SetUp()
        {
            this.kernel = new StandardKernel();
        }
    }

    [TestClass]
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

    [TestClass]
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

    [TestClass]
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