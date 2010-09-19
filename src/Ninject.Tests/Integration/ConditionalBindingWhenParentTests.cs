namespace Ninject.Tests.Integration
{
    using Ninject.Tests.Fakes;
#if SILVERLIGHT
    using UnitDriven;
    using UnitDriven.Should;
    using Fact = UnitDriven.TestMethodAttribute;
#else
    using Ninject.Tests.MSTestAttributes;
    using Xunit;
    using Xunit.Should;
#endif

    public class WhenParentContext
    {
        protected StandardKernel kernel;

        public WhenParentContext()
        {
            this.SetUp();
        }

        [TestInitialize]
        public void SetUp()
        {
            this.kernel = new StandardKernel();
            this.kernel.Bind<Sword>().ToSelf().Named("Broken");
            this.kernel.Bind<Sword>().ToSelf().WhenParentNamed("Something");
        }
    }

    [TestClass]
    public class WhenParentNamed : WhenParentContext
    {
        [Fact]
        public void NamedInstanceAvailableEvenWithWhenBinding()
        {
            var instance = kernel.Get<Sword>("Broken");

            instance.ShouldNotBeNull();
            instance.ShouldBeInstanceOf(typeof(Sword));
        }
    }
}