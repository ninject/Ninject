namespace Ninject.Tests.Integration
{
    using System;

    using FluentAssertions;

    using Xunit;

    public class WhenReleasingAnObject
    {
        private Foo foo;

        private StandardKernel kernel;

        public WhenReleasingAnObject()
        {
            this.kernel = new StandardKernel();

            this.kernel.Bind<Foo>().ToSelf().InSingletonScope();
            this.kernel.Bind<Bar>().ToSelf().InScope(ctx => this.foo);

            this.foo = this.kernel.Get<Foo>();
        }

        [Fact]
        public void ItIsDisposed()
        {
            this.kernel.Release(this.foo);

            this.foo.Disposed.Should().BeTrue();
        }

        [Fact]
        public void ObjectsThatHaveItAsScopeAreDisposed()
        {
            var bar = kernel.Get<Bar>();

            this.kernel.Release(this.foo);

            bar.Disposed.Should().BeTrue();
        }

        public class Foo : IDisposable
        {
            public void Dispose()
            {
                this.Disposed = true;
            }

            public bool Disposed { get; set; }
        }

        public class Bar : IDisposable
        {
            public void Dispose()
            {
                this.Disposed = true;
            }

            public bool Disposed { get; set; }
        }

    }
}