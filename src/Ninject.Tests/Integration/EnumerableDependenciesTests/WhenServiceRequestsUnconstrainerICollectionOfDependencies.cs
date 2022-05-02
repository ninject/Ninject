namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
    using FluentAssertions;
    using Ninject.Parameters;
    using Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class WhenServiceRequestsUnconstrainedICollectionOfDependencies : UnconstrainedDependenciesContext
    {
        [Fact]
        public void ServiceIsInjectedWithListOfAllAvailableDependencies()
        {
            this.Kernel.Bind<IParent>().To<RequestsICollection>();
            this.Kernel.Bind<IChild>().To<ChildA>();
            this.Kernel.Bind<IChild>().To<ChildB>();

            var parent = this.Kernel.Get<IParent>();

            VerifyInjection(parent);
        }

        [Fact]
        public void ServiceIsInjectedWithEmptyListIfElementTypeIsMissingBinding()
        {
            this.Kernel.Bind<IParent>().To<RequestsICollection>();

            var parent = this.Kernel.Get<IParent>();

            parent.Should().NotBeNull();
            parent.Children.Should().BeEmpty();
        }

        [Fact]
        public void ListIsResolvedIfElementTypeIsExplicitlyBinded()
        {
            this.Kernel.Bind<IChild>().To<ChildA>();

            var children = this.Kernel.Get<ICollection<IChild>>();

            children.Should().NotBeEmpty();
            children.Should().BeOfType<List<IChild>>();

            children = this.Kernel.TryGet<ICollection<IChild>>();

            children.Should().NotBeEmpty();
            children.Should().BeOfType<List<IChild>>();

            var allChildren = this.Kernel.GetAll<IChild>();

            allChildren.Should().NotBeEmpty();
        }

        [Fact]
        public void EmptyListIsResolvedIfElementTypeIsMissingBinding()
        {
            var children = this.Kernel.Get<ICollection<IChild>>();

            children.Should().BeEmpty();
            children.Should().BeOfType<List<IChild>>();

            children = this.Kernel.TryGet<ICollection<IChild>>();

            children.Should().BeEmpty();
            children.Should().BeOfType<List<IChild>>();

            var allChildren = this.Kernel.GetAll<IChild>();

            allChildren.Should().BeEmpty();
        }
    }
}