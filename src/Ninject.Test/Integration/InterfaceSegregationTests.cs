namespace Ninject.Tests.Integration
{
    using System;
    using FluentAssertions;
    using Ninject.Planning.Bindings;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class InterfaceSegregationTests : IDisposable
    {
        private StandardKernel kernel;

        public InterfaceSegregationTests()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }

        [Fact]
        public void ServiceWithMultipleInterfacesCanReturnSameInstance()
        {
            var config = this.kernel.Bind<IWarrior>().To<Monk>().InSingletonScope().Binding.BindingConfiguration;
            this.kernel.AddBinding(new Binding(typeof(ICleric), config));

            var warrior = this.kernel.Get<IWarrior>();
            var cleric = this.kernel.Get<ICleric>();

            warrior.Should().BeSameAs(cleric);
        }
    }
}