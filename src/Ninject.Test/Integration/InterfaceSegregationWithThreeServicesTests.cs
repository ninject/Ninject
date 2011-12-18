namespace Ninject.Tests.Integration
{
    using System;
    using FluentAssertions;

    using Ninject.Activation;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class InterfaceSegregationWithFourServicesTests : IDisposable
    {
        private readonly StandardKernel kernel;

        public InterfaceSegregationWithFourServicesTests()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }

        [Fact]
        public void MultipleServicesBoundWithGenericToReturnSameInstance()
        {
            this.kernel.Bind<IWarrior, ICleric, IHuman, ILifeform>().To<Monk>().InSingletonScope();

            this.VerifyAllInterfacesAreSameInstance();
        }

        [Fact]
        public void MultipleServicesBoundWithToReturnSameInstance()
        {
            this.kernel.Bind<IWarrior, ICleric, IHuman, ILifeform>().To(typeof(Monk)).InSingletonScope();

            this.VerifyAllInterfacesAreSameInstance();
        }

        [Fact]
        public void MultipleServicesBoundWithToConstantReturnSameInstance()
        {
            this.kernel.Bind<IWarrior, ICleric, IHuman, ILifeform>().ToConstant(new Monk()).InSingletonScope();

            this.VerifyAllInterfacesAreSameInstance();
        }

        [Fact]
        public void MultipleServicesBoundWithToMethodReturnSameInstance()
        {
            this.kernel.Bind<IWarrior, ICleric, IHuman, ILifeform>().ToMethod(ctx => new Monk()).InSingletonScope();

            this.VerifyAllInterfacesAreSameInstance();
        }

        [Fact]
        public void MultipleServicesBoundWithToProviderReturnSameInstance()
        {
            this.kernel.Bind<IWarrior, ICleric, IHuman, ILifeform>().ToProvider(new MonkProvider()).InSingletonScope();

            this.VerifyAllInterfacesAreSameInstance();
        }

        [Fact]
        public void MultipleServicesBoundWithGenericToProviderReturnSameInstance()
        {
            this.kernel.Bind<IWarrior, ICleric, IHuman, ILifeform>().ToProvider<MonkProvider>().InSingletonScope();

            this.VerifyAllInterfacesAreSameInstance();
        }

        private void VerifyAllInterfacesAreSameInstance()
        {
            var warrior = this.kernel.Get<IWarrior>();
            var cleric = this.kernel.Get<ICleric>();
            var human = this.kernel.Get<IHuman>();
            var lifeform = this.kernel.Get<ILifeform>();

            warrior.Should().BeSameAs(cleric);
            human.Should().BeSameAs(cleric);
            lifeform.Should().BeSameAs(cleric);
        }

        public class MonkProvider : Provider<Monk>
        {
            protected override Monk CreateInstance(IContext context)
            {
                return new Monk();
            }
        }
    }
}