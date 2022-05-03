namespace Ninject.Tests.Unit.Syntax
{
    using Ninject.Planning.Bindings;
    using Ninject.Syntax;
    using System;
    using Xunit;

    public class BindingRootTests
    {
        private BindingRoot _bindingRoot;

        public BindingRootTests()
        {
            _bindingRoot = new MyBindingRoot();
        }

        [Fact]
        public void Bind_ShouldThrowArgumentNullExceptionWhenServicesIsNull()
        {
            const Type[] services = null;

            var actual = Assert.Throws<ArgumentNullException>(() => _bindingRoot.Bind(services));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(services), actual.ParamName);
        }

        [Fact]
        public void Bind_ShouldThrowArgumentExceptionWhenServicesIsEmpty()
        {
            var services = Array.Empty<Type>();

            var actual = Assert.Throws<ArgumentException>(() => _bindingRoot.Bind(services));

            Assert.Null(actual.InnerException);
            Assert.StartsWith("Specify at least one type to bind.", actual.Message);
            Assert.Equal(nameof(services), actual.ParamName);
        }

        [Fact]
        public void Rebind_ShouldThrowArgumentNullExceptionWhenServicesIsNull()
        {
            const Type[] services = null;

            var actual = Assert.Throws<ArgumentNullException>(() => _bindingRoot.Rebind(services));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(services), actual.ParamName);
        }

        [Fact]
        public void Rebind_ShouldThrowArgumentExceptionWhenServicesIsEmpty()
        {
            var services = Array.Empty<Type>();

            var actual = Assert.Throws<ArgumentException>(() => _bindingRoot.Rebind(services));

            Assert.Null(actual.InnerException);
            Assert.StartsWith("Specify at least one type to bind.", actual.Message);
            Assert.Equal(nameof(services), actual.ParamName);
        }

        public class MyBindingRoot : BindingRoot
        {
            protected override IKernel KernelInstance
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override void AddBinding(IBinding binding)
            {
                throw new NotImplementedException();
            }

            public override void RemoveBinding(IBinding binding)
            {
                throw new NotImplementedException();
            }

            public override void Unbind(Type service)
            {
                throw new NotImplementedException();
            }
        }

    }
}
