namespace Ninject.Tests.Integration
{
    using Ninject.Tests.Fakes;
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

    [TestClass]
    public class OptionalTest
    {
        private StandardKernel kernel;

        public OptionalTest()
        {
            this.SetUp();
        }

        [TestInitialize]
        public void SetUp()
        {
            this.kernel = new StandardKernel();
        }

        [Fact]
        public void OptionalConstructorArgument()
        {
            var testClass = this.kernel.Get<OptionalConstructorArgumentTestClass>();

            testClass.ShouldNotBeNull();
            testClass.Warrior.ShouldBeNull();
        }

        [Fact]
        public void OptionalMethodArgument()
        {
            var testClass = this.kernel.Get<OptionalMethodArgumentTestClass>();

            testClass.ShouldNotBeNull();
            testClass.Warrior.ShouldBeNull();
        }

        [Fact]
        public void OptionalProperty()
        {
            var testClass = this.kernel.Get<OptionalMethodArgumentTestClass>();

            testClass.ShouldNotBeNull();
            testClass.Warrior.ShouldBeNull();
        }

        public class OptionalConstructorArgumentTestClass
        {
            public OptionalConstructorArgumentTestClass([Optional]IWarrior warrior)
            {
                this.Warrior = warrior;
            }

            public IWarrior Warrior { get; private set; }
        }

        public class OptionalMethodArgumentTestClass
        {
            public IWarrior Warrior { get; private set; }

            [Inject]
            public void SetWarrior([Optional] IWarrior warrior)
            {
                this.Warrior = warrior;
            }
        }
    
        public class OptionalPropertyTestClass
        {
            [Inject]
            [Optional]
            public IWarrior Warrior { get; set; }
        }
    }
}