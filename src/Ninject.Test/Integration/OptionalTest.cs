namespace Ninject.Tests.Integration
{
    using FluentAssertions;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class OptionalTest
    {
        private readonly StandardKernel kernel;

        public OptionalTest()
        {
            this.kernel = new StandardKernel();
        }

        [Fact]
        public void OptionalConstructorArgument()
        {
            var testClass = this.kernel.Get<OptionalConstructorArgumentTestClass>();

            testClass.Should().NotBeNull();
            testClass.Warrior.Should().BeNull();
        }

        [Fact]
        public void OptionalMethodArgument()
        {
            var testClass = this.kernel.Get<OptionalMethodArgumentTestClass>();

            testClass.Should().NotBeNull();
            testClass.Warrior.Should().BeNull();
        }

        [Fact]
        public void OptionalProperty()
        {
            var testClass = this.kernel.Get<OptionalMethodArgumentTestClass>();

            testClass.Should().NotBeNull();
            testClass.Warrior.Should().BeNull();
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