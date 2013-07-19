namespace Ninject.Tests.Integration
{
    using System.Linq;

    using FluentAssertions;

    using Xunit;

    public class GenericBindings
    {
        [Fact]
        public void ShouldReturnASingleBindingOnly()
        {
            var kernel = new StandardKernel();
            kernel.Bind(typeof(GenericService<>)).ToSelf().InTransientScope();

            var bindings = kernel.GetBindings(typeof(GenericService<>)).ToList();

            bindings.Distinct().Count().Should().Be(1);

            bindings.Count.Should().Be(1);
        }

        public class GenericService<T>
        {
        }
    }
}