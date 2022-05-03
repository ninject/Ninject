namespace Ninject.Tests.Unit.Planning
{
    using Ninject.Planning;
    using Ninject.Planning.Directives;
    using System;
    using Xunit;

    public class PlanTests
    {
        private Plan _plan;
        private ConstructorInjectionDirective _constructor1;
        private ConstructorInjectionDirective _constructor2;
        private PropertyInjectionDirective _property;

        public PlanTests()
        {
            _constructor1 = CreateConstructorInjectionDirective();
            _constructor2 = CreateConstructorInjectionDirective();
            _property = CreatePropertyInjectionDirective();

            _plan = new Plan(this.GetType());
            _plan.Add(_constructor1);
            _plan.Add(_property);
            _plan.Add(_constructor2);
        }

        [Fact]
        public void Ctor_Type_ShouldThrowArgumentNullExceptionWhenTypeIsNull()
        {
            const Type type = null;

            var actualException = Assert.Throws<ArgumentNullException>(() => new Plan(type));
            Assert.Equal(nameof(type), actualException.ParamName);
        }

        [Fact]
        public void Ctor_Type()
        {
            var plan = new Plan(typeof(MyService));

            Assert.Same(typeof(MyService), plan.Type);
            Assert.Empty(plan.Directives);
        }

        [Fact]
        public void Add_Directive_ShouldThrowArgumentNullExceptionWhenDirectiveIsNull()
        {
            const IDirective directive = null;

            var actualException = Assert.Throws<ArgumentNullException>(() => _plan.Add(directive));
            Assert.Equal(nameof(directive), actualException.ParamName);
        }

        [Fact]
        public void Has_ShouldReturnFalseWhenNoDirectiveOfSpecifiedTypeExists()
        {
            Assert.False(_plan.Has<MethodInjectionDirective>());
        }

        [Fact]
        public void Has_ShouldReturnTrueWhenAtLeastOneDirectiveOfSpecifiedTypeExists()
        {
            Assert.True(_plan.Has<ConstructorInjectionDirective>());
            Assert.True(_plan.Has<PropertyInjectionDirective>());
        }

        [Fact]
        public void GetOne_ShouldReturnDirectiveOfSpecifiedTypeWhenOnlyOneDirectiveOfSpecifiedTypeExists()
        {
            var actual = _plan.GetOne<PropertyInjectionDirective>();

            Assert.NotNull(actual);
            Assert.Same(_property, actual);
        }

        [Fact]
        public void GetOne_ShouldReturnNullWhenNoDirectiveOfSpecifiedTypeExists()
        {
            Assert.Null(_plan.GetOne<MethodInjectionDirective>());
        }

        [Fact]
        public void GetOne_ShoulThrowInvalidOperationExceptionWhenMoreThanOneDirectiveOfSpecifiedTypeExists()
        {
            Assert.Throws<InvalidOperationException>(() => _plan.GetOne<ConstructorInjectionDirective>());
        }

        [Fact]
        public void GetOne_ShouldReturnAllDirectivesOfSpecifiedType()
        {
            var actual = _plan.GetAll<ConstructorInjectionDirective>();

            using (var enumerator = actual.GetEnumerator())
            {
                Assert.True(enumerator.MoveNext());
                Assert.Same(_constructor1, enumerator.Current);

                Assert.True(enumerator.MoveNext());
                Assert.Same(_constructor2, enumerator.Current);

                Assert.False(enumerator.MoveNext());
            }
        }

        [Fact]
        public void GetAll_ShouldReturnEmptyEnumeratorWhenNoDirectivesOfSpecifiedTypeExist()
        {
            var actual = _plan.GetAll<MethodInjectionDirective>();

            using (var enumerator = actual.GetEnumerator())
            {
                Assert.False(enumerator.MoveNext());
            }
        }

        private static ConstructorInjectionDirective CreateConstructorInjectionDirective()
        {
            return new ConstructorInjectionDirective(typeof(MyService).GetConstructor(new Type[0]), (_) => null);
        }

        private static PropertyInjectionDirective CreatePropertyInjectionDirective()
        {
            return new PropertyInjectionDirective(typeof(MyService).GetProperty("Name"), (target, value) => { });
        }

        private static MethodInjectionDirective CreateMethodInjectionDirective()
        {
            return new MethodInjectionDirective(typeof(MyService).GetMethod("Run"), (target, arguments) => { });
        }

        public class MyService
        {
            public MyService()
            {
            }

            public string Name { get; }

            public void Run()
            {
            }
        }
    }
}
