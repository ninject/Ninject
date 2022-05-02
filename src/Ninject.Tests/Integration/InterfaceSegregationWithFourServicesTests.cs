//-------------------------------------------------------------------------------
// <copyright file="InterfaceSegregationWithFourServicesTests.cs" company="Ninject Project Contributors">
//   Copyright (c) 2009-2011 Ninject Project Contributors
//   Authors: Remo Gloor (remo.gloor@gmail.com)
//           
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   you may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace Ninject.Tests.Integration
{
    using System;
    using System.Linq;

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

        [Fact]
        public void Rebind()
        {
            this.kernel.Bind<IWarrior, ICleric, IHuman, ILifeform>().To<Monk>().InSingletonScope();
            this.kernel.Bind<IWarrior, ICleric, IHuman, ILifeform>().To<Monk>().InSingletonScope();
            this.kernel.Rebind<IWarrior, ICleric, IHuman, ILifeform>().To<Monk>().InSingletonScope();

            this.kernel.GetBindings(typeof(IWarrior)).Count().Should().Be(1);
            this.kernel.GetBindings(typeof(ICleric)).Count().Should().Be(1);
            this.kernel.GetBindings(typeof(IHuman)).Count().Should().Be(1);
            this.kernel.GetBindings(typeof(ILifeform)).Count().Should().Be(1);
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