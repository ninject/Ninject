//-------------------------------------------------------------------------------
// <copyright file="ConstructorArgumentInBindingConfigurationBuilderTest.cs" company="Ninject Project Contributors">
//   Copyright (c) 2009-2013 Ninject Project Contributors
//   Authors: Ivan Appert (iappert@gmail.com)
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
    using FluentAssertions;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class ConstructorArgumentInBindingConfigurationBuilderTest : IDisposable
    {
        private readonly StandardKernel kernel;

        public ConstructorArgumentInBindingConfigurationBuilderTest()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }

        [Fact]
        public void ConstructorArgumentWithMatchingTypeShouldBeUsed()
        {
            var expectedWeapon = new Dagger();
            this.kernel.Bind<Samurai>().ToSelf().WithConstructorArgument<IWeapon>(expectedWeapon);

            var samurai = this.kernel.Get<Samurai>();

            samurai.Weapon.Should().Be(expectedWeapon);
        }

        [Fact]
        public void ConstructorArgumentWithMatchingTypeShouldBeUsedIfUsingExplicitTypeArgumentSyntax()
        {
            var expectedWeapon = new Dagger();
            this.kernel.Bind<Samurai>().ToSelf().WithConstructorArgument(typeof(IWeapon), expectedWeapon);

            var samurai = this.kernel.Get<Samurai>();

            samurai.Weapon.Should().Be(expectedWeapon);
        }

        [Fact]
        public void ConstructorArgumentWithMatchingTypeShouldBeUsedIfUsingCallbackWithContext()
        {
            var expectedWeapon = new Shuriken();
            this.kernel.Bind<Samurai>().ToSelf().WithConstructorArgument(typeof(IWeapon), context => expectedWeapon);

            var samurai = this.kernel.Get<Samurai>();

            samurai.Weapon.Should().Be(expectedWeapon);
        }

        [Fact]
        public void ConstructorArgumentWithMatchingTypeShouldBeUsedIfUsingCallbackWithContextAndTarget()
        {
            var expectedWeapon = new Shuriken();
            this.kernel.Bind<Samurai>().ToSelf().WithConstructorArgument(typeof(IWeapon), (context, target) => expectedWeapon);

            var samurai = this.kernel.Get<Samurai>();

            samurai.Weapon.Should().Be(expectedWeapon);
        }
    }
}