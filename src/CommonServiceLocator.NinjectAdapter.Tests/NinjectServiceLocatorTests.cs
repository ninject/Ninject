using FluentAssertions;
using Microsoft.Practices.ServiceLocation;
using Ninject;
using Ninject.Infrastructure.Disposal;
using Xunit;

namespace CommonServiceLocator.NinjectAdapter.Tests
{
    public interface IFoo { }
    public class Foo : IFoo { }

    public class NinjectServiceLocatorTests : DisposableObject
    {
        protected IKernel kernel;

        [Fact]
        public void DefaultInstanceIsResolvedWhenNoKeySpecified()
        {
            this.kernel = new StandardKernel();
            ServiceLocator.SetLocatorProvider(() => new NinjectServiceLocator(kernel));

            this.kernel.Bind<IFoo>().To<Foo>().Named("SomeFoo");

            // CommonServiceLocator requires that the named binding still be used.
            // http://commonservicelocator.codeplex.com/wikipage?title=API%20Reference&referringTitle=Home
            var instance = ServiceLocator.Current.GetInstance<IFoo>();

            instance.Should().NotBeNull();
            instance.Should().BeOfType<Foo>();
        }

        public override void Dispose( bool disposing )
        {
            if ( disposing && !IsDisposed )
            {
                this.kernel.Dispose();
                this.kernel = null;
            }
            base.Dispose( disposing );
        }
    }
}
