namespace Ninject.Tests.Integration
{
    using System;
    using FluentAssertions;

    using Ninject.Activation;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class InterfaceSegregationWithThreerServicesTests : IDisposable
    {
        private readonly StandardKernel kernel;

        public InterfaceSegregationWithThreerServicesTests()
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
            this.kernel.Bind<IWarrior, ICleric, IHuman>().To<Monk>().InSingletonScope();

            this.VerifyAllInterfacesAreSameInstance();
        }

        [Fact]
        public void MultipleServicesBoundWithToReturnSameInstance()
        {
            this.kernel.Bind<IWarrior, ICleric, IHuman>().To(typeof(Monk)).InSingletonScope();

            this.VerifyAllInterfacesAreSameInstance();
        }

        [Fact]
        public void MultipleServicesBoundWithToConstantReturnSameInstance()
        {
            this.kernel.Bind<IWarrior, ICleric, IHuman>().ToConstant(new Monk()).InSingletonScope();

            this.VerifyAllInterfacesAreSameInstance();
        }

        [Fact]
        public void MultipleServicesBoundWithToMethodReturnSameInstance()
        {
            this.kernel.Bind<IWarrior, ICleric, IHuman>().ToMethod(ctx => new Monk()).InSingletonScope();

            this.VerifyAllInterfacesAreSameInstance();
        }

        [Fact]
        public void MultipleServicesBoundWithToProviderReturnSameInstance()
        {
            this.kernel.Bind<IWarrior, ICleric, IHuman>().ToProvider(new MonkProvider()).InSingletonScope();

            this.VerifyAllInterfacesAreSameInstance();
        }

        [Fact]
        public void MultipleServicesBoundWithGenericToProviderReturnSameInstance()
        {
            this.kernel.Bind<IWarrior, ICleric, IHuman>().ToProvider<MonkProvider>().InSingletonScope();

            this.VerifyAllInterfacesAreSameInstance();
        }

        private void VerifyAllInterfacesAreSameInstance()
        {
            var warrior = this.kernel.Get<IWarrior>();
            var cleric = this.kernel.Get<ICleric>();
            var human = this.kernel.Get<IHuman>();

            warrior.Should().BeSameAs(cleric);
            human.Should().BeSameAs(cleric);
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