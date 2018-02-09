namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes;
    using Xunit;

    public class WhenServiceRequestsUnconstrainedEnumerableOfDependencies : UnconstrainedDependenciesContext
    {
        [Fact]
        public void ServiceIsInjectedWithEnumeratorOfAllAvailableDependencies()
        {
            this.Kernel.Bind<IParent>().To<RequestsEnumerable>();
            this.Kernel.Bind<IChild>().To<ChildA>();
            this.Kernel.Bind<IChild>().To<ChildB>();

            var parent = this.Kernel.Get<IParent>();

            VerifyInjection(parent);
        }

        [Fact]
        public void EmptyEnumerableIsInjectedIfElementTypeIsMissingBinding()
        {
            this.Kernel.Bind<IParent>().To<RequestsEnumerable>();

            var parent = this.Kernel.Get<IParent>();

            parent.Should().NotBeNull();
            parent.Children.Count.Should().Be(0);
        }

        [Fact]
        public void EnumerableIsResolvedIfElementTypeIsIsExplicitlyBinded()
        {
            this.Kernel.Bind<IChild>().To<ChildA>();
            this.Kernel.Bind<IChild>().To<ChildB>();

            var children = this.Kernel.Get<IEnumerable<IChild>>();

            children.Should().NotBeEmpty();
        }

        [Fact]
        public void EmptyEnumerableIsResolvedIfElementTypeIsMissingBinding()
        {
            var children = this.Kernel.Get<IEnumerable<IChild>>();

            children.Should().BeEmpty();
        }

        [Fact]
        public void EmptyEnumerableIsResolvedIfElementTypeIsMissingBindingEvenIsWasResoved()
        {
            var child = this.Kernel.Get<ChildA>();
            var children = this.Kernel.Get<IEnumerable<ChildA>>();

            children.Should().BeEmpty();
        }
    }
}
