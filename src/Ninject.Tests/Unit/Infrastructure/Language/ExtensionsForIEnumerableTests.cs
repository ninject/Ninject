namespace Ninject.Tests.Unit.Infrastructure.Language
{
    using System.Collections;
    using Xunit;
    using Ninject.Tests.Fakes;
    using System;
    using System.Collections.Generic;

    public class ExtensionsForIEnumerableTests
    {
        [Fact]
        public void ToListSlow_AssigmentCompatibility()
        {
            IEnumerable enumerable = new IWeapon[] { new Dagger(), new ShortSword() };

            var actual = Ninject.Infrastructure.Language.ExtensionsForIEnumerable.ToListSlow(enumerable, typeof(IWeapon));

            Assert.NotNull(actual);
            Assert.Equal(enumerable, actual);

            var typedList = actual as List<IWeapon>;

            Assert.NotNull(typedList);
            Assert.Equal(enumerable, typedList);
        }

        [Fact]
        public void ToListSlow_Covariance()
        {
            IEnumerable enumerable = new Sword[] { new Sword(), new ShortSword() };

            var actual = Ninject.Infrastructure.Language.ExtensionsForIEnumerable.ToListSlow(enumerable, typeof(IWeapon));

            Assert.NotNull(actual);
            Assert.Equal(enumerable, actual);

            var typedList = actual as List<IWeapon>;

            Assert.NotNull(typedList);
            Assert.Equal(enumerable, typedList);
        }

        [Fact]
        public void ToListSlow_Contravariance()
        {
            IEnumerable enumerable = new object[] { new Sword(), new ShortSword() };

            var actualException = Assert.Throws<ArgumentException>(() => Ninject.Infrastructure.Language.ExtensionsForIEnumerable.ToListSlow(enumerable, typeof(IWeapon)));

            Assert.Null(actualException.InnerException);
        }

        [Fact]
        public void CastSlow_Covariance()
        {
            IEnumerable enumerable = new Sword[] { new Sword(), new ShortSword() };

            var actual = Ninject.Infrastructure.Language.ExtensionsForIEnumerable.CastSlow(enumerable, typeof(IWeapon));

            Assert.NotNull(actual);
            Assert.Equal(enumerable, actual);

            var typedEnumerable = actual as IEnumerable<IWeapon>;

            Assert.NotNull(typedEnumerable);
            Assert.Equal(enumerable, typedEnumerable);
        }

        [Fact]
        public void CastSlow_Contravariance()
        {
            IEnumerable enumerable = new object[] { new FootSoldier(), new Samurai(new Dagger()) };

            var actual = Ninject.Infrastructure.Language.ExtensionsForIEnumerable.CastSlow(enumerable, typeof(IWarrior));

            Assert.NotNull(actual);
            Assert.Equal(enumerable, actual);

            var typedEnumerable = actual as IEnumerable<IWarrior>;

            Assert.NotNull(typedEnumerable);
            Assert.Equal(enumerable, typedEnumerable);
        }

        [Fact]
        public void CastSlow_Contravariance_ElementCannotBeCastToElementType()
        {
            IEnumerable enumerable = new object[] { new FootSoldier(), new ShortSword() };

            var actual = Ninject.Infrastructure.Language.ExtensionsForIEnumerable.CastSlow(enumerable, typeof(IWarrior));

            Assert.NotNull(actual);
            var enumerator = actual.GetEnumerator();
            Assert.True(enumerator.MoveNext());
            Assert.NotNull(enumerator.Current);
            Assert.IsType<FootSoldier>(enumerator.Current);
            Assert.Throws<InvalidCastException>(() => enumerator.MoveNext());

            var typedEnumerable = actual as IEnumerable<IWarrior>;

            Assert.NotNull(typedEnumerable);
            var typedEnumerator = typedEnumerable.GetEnumerator();
            Assert.True(typedEnumerator.MoveNext());
            Assert.NotNull(typedEnumerator.Current);
            Assert.IsType<FootSoldier>(typedEnumerator.Current);
            Assert.Throws<InvalidCastException>(() => typedEnumerator.MoveNext());
        }
    }
}
