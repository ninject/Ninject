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
    using System.Collections.Generic;
    using FluentAssertions;

    using Ninject.Parameters;
    using Ninject.Tests.Fakes;

    using Xunit;
    using Xunit.Extensions;

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
                // ReSharper disable CoVariantArrayConversion
                yield return new Func<bool, IConstructorArgument>[] { inherited => new ConstructorArgument("weapon", new Sword(), inherited) };
                yield return new Func<bool, IConstructorArgument>[] { inherited => new WeakConstructorArgument("weapon", new Sword(), inherited),  };
                yield return new Func<bool, IConstructorArgument>[] { inherited => new TypeMatchingConstructorArgument(typeof(IWeapon), new Sword(), inherited) };
                // ReSharper restore CoVariantArrayConversion
            }
        }

        public static IEnumerable<object[]> ConstructorArgumentsWithoutShouldInheritArgument
        {
            get
            {
                // ReSharper disable CoVariantArrayConversion
                yield return new Func<IConstructorArgument>[] { () => new ConstructorArgument("weapon", new Sword()) };
                yield return new Func<IConstructorArgument>[] { () => new WeakConstructorArgument("weapon", new Sword()),  };
                yield return new Func<IConstructorArgument>[] { () => new TypeMatchingConstructorArgument(typeof(IWeapon), new Sword()) };
                // ReSharper restore CoVariantArrayConversion
            }
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }

        [Theory]
        [PropertyData("ConstructorArguments")]
        public void ConstructorArgumentsArePassedToFirstLevel(Func<bool, IConstructorArgument> constructorArgument)
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<IWeapon>().To<Dagger>();

            var baracks = this.kernel.Get<Barracks>(constructorArgument(false));

            baracks.Weapon.Should().BeOfType<Sword>();
            baracks.Warrior.Weapon.Should().BeOfType<Dagger>();
        }

        [Theory]
        [PropertyData("ConstructorArgumentsWithoutShouldInheritArgument")]
        public void ConstructorArgumentsAreNotInheritedIfNotSpecified(Func<IConstructorArgument> constructorArgument)
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();

            Action getAction = () => this.kernel.Get<Barracks>(constructorArgument());

            getAction.ShouldThrow<ActivationException>();
        }
        
        [Theory]
        [PropertyData("ConstructorArguments")]
        public void ConstructorArgumentsAreInheritedIfSpecified(Func<bool, IConstructorArgument> constructorArgument)
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();
            var baracks = this.kernel.Get<Barracks>(constructorArgument(true));

            baracks.Weapon.Should().BeOfType<Sword>();
            baracks.Warrior.Weapon.Should().BeOfType<Sword>();
        }

#if !MONO
        [Fact]
        public void WeakConstructorArgument()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<IWeapon>().To<Dagger>();
            this.kernel.Bind<Barracks>().ToSelf().InSingletonScope();

            var weakReference = this.Process();

            var baracks = this.kernel.Get<Barracks>();

            baracks.Weapon.Should().BeOfType<Sword>();
            baracks.Warrior.Weapon.Should().BeOfType<Dagger>();
            baracks.Weapon.Should().BeSameAs(weakReference.Target);
            baracks.Weapon = null;

            GC.Collect();

            weakReference.IsAlive.Should().BeFalse();
        }
#endif

        private WeakReference Process()
        {
            var sword = new Sword();
            this.kernel.Get<Barracks>(new WeakConstructorArgument("weapon", sword));
            return new WeakReference(sword);
        }
    }
}