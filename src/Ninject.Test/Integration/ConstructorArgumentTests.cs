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

using System.Diagnostics;

namespace Ninject.Tests.Integration
{
    using System;
    using System.Collections.Generic;
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
        
        public static IEnumerable<object[]> ConstructorArguments
        {
            get
            {
                yield return new Func<bool, IConstructorArgument>[] { inherited => new ConstructorArgument("weapon", new Sword(), inherited) };
                yield return new Func<bool, IConstructorArgument>[] { inherited => new WeakConstructorArgument("weapon", new Sword(), inherited),  };
                yield return new Func<bool, IConstructorArgument>[]
                             {
                                 inherited => new TypeMatchingConstructorArgument(typeof(IWeapon), (context, target) => new Sword(), inherited)
                             };
            }
        }

        public static IEnumerable<object[]> ConstructorArgumentsWithoutShouldInheritArgument
        {
            get
            {
                yield return new Func<IConstructorArgument>[] { () => new ConstructorArgument("weapon", new Sword()) };
                yield return new Func<IConstructorArgument>[] { () => new WeakConstructorArgument("weapon", new Sword()),  };
                yield return new Func<IConstructorArgument>[] { () => new TypeMatchingConstructorArgument(typeof(IWeapon), (context, target) => new Sword()) };
            }
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }


        [Theory]
        [MemberData(nameof(ConstructorArguments))]
        public void ConstructorArgumentsArePassedToFirstLevel(Func<bool, IConstructorArgument> constructorArgument)
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<IWeapon>().To<Dagger>();

            var argument = constructorArgument(false);

            var barracks = this.kernel.Get<Barracks>(argument);

            if (argument is WeakConstructorArgument)
            {
                barracks.Weapon.Should().Match<IWeapon>(s => s == null || s is Sword);
            }
            else
            {
                barracks.Weapon.Should().BeOfType<Sword>();
            }

            barracks.Warrior.Weapon.Should().BeOfType<Dagger>();
        }

        [Theory]
        [MemberData(nameof(ConstructorArgumentsWithoutShouldInheritArgument))]
        public void ConstructorArgumentsAreNotInheritedIfNotSpecified(Func<IConstructorArgument> constructorArgument)
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();

            Action getAction = () => this.kernel.Get<Barracks>(constructorArgument());

            getAction.Should().Throw<ActivationException>();
        }
        
        [Theory]
        [MemberData(nameof(ConstructorArguments))]
        public void ConstructorArgumentsAreInheritedIfSpecified(Func<bool, IConstructorArgument> constructorArgument)
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();

            var argument = constructorArgument(true);

            var barracks = this.kernel.Get<Barracks>(argument);

            if (argument is WeakConstructorArgument)
            {
                barracks.Weapon.Should().Match<IWeapon>(s => s == null || s is Sword);
                barracks.Warrior.Weapon.Should().Match<IWeapon>(s => s == null || s is Sword);
            }
            else
            {
                barracks.Weapon.Should().BeOfType<Sword>();
                barracks.Warrior.Weapon.Should().BeOfType<Sword>();
            }
        }

        [Fact]
        public void WeakConstructorArgument()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<IWeapon>().To<Dagger>();
            this.kernel.Bind<Barracks>().ToSelf().InSingletonScope();

            var weakReference = this.Process();

            var barracks = this.kernel.Get<Barracks>();

            barracks.Weapon.Should().BeOfType<Sword>();
            barracks.Warrior.Weapon.Should().BeOfType<Dagger>();
            barracks.Weapon.Should().BeSameAs(weakReference.Target);
            barracks.Weapon = null;

            GC.Collect();

            weakReference.IsAlive.Should().BeFalse();
        }


        private WeakReference Process()
        {
            var sword = new Sword();
            this.kernel.Get<Barracks>(new WeakConstructorArgument("weapon", sword));
            return new WeakReference(sword);
        }
    }
}