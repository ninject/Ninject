//-------------------------------------------------------------------------------
// <copyright file="ConstructorArgumentTests.cs" company="Ninject Project Contributors">
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

    using FluentAssertions;

    using Ninject.Parameters;
    using Ninject.Tests.Fakes;

    using Xunit;

    public class ConstructorArgumentTests : IDisposable
    {
        private StandardKernel kernel;

        public ConstructorArgumentTests()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }

        [Fact]
        public void ConstructorArgumentsArePassedToFirstLevel()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<IWeapon>().To<Dagger>();

            var baracks = this.kernel.Get<Barracks>(new ConstructorArgument("weapon", new Sword()));

            baracks.Weapon.Should().BeOfType<Sword>();
            baracks.Warrior.Weapon.Should().BeOfType<Dagger>();
        }
        
        [Fact]
        public void ConstructorArgumentsAreNotInheritedIfNotSpecified()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();

            Action getAction = () => this.kernel.Get<Barracks>(new ConstructorArgument("weapon", new Sword()));

            getAction.ShouldThrow<ActivationException>();
        }
        
        [Fact]
        public void ConstructorArgumentsAreInheritedIfSpecified()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();

            var baracks = this.kernel.Get<Barracks>(new ConstructorArgument("weapon", new Sword(), true));

            baracks.Weapon.Should().BeOfType<Sword>();
            baracks.Warrior.Weapon.Should().BeOfType<Sword>();
        }
    }
}