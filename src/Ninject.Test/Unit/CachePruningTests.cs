#if !NO_MOQ
namespace Ninject.Tests.Unit.CacheTests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using Ninject.Activation;
    using Ninject.Activation.Caching;
    using Ninject.Activation.Strategies;
    using Ninject.Parameters;
    using Ninject.Planning;
    using Ninject.Planning.Bindings;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class WhenPruneIsCalled
    {
        private Mock<ICachePruner> cachePrunerMock;
        private Mock<IBindingConfiguration> bindingConfigurationMock;
        private Cache cache;

        public WhenPruneIsCalled()
        {
            this.SetUp();
        }

        public void SetUp()
        {
            this.cachePrunerMock = new Mock<ICachePruner>();
            this.bindingConfigurationMock = new Mock<IBindingConfiguration>();
            this.cache = new Cache(new PipelineMock(), this.cachePrunerMock.Object);
        }

#if !MONO
        [Fact]
        public void CollectedScopeInstancesAreRemoved()
        {
            var sword = new Sword();
            var swordWeakReference = new WeakReference(sword);
            var context = CreateContextMock(new TestObject(42), this.bindingConfigurationMock.Object);
            this.Remember(sword, context);

            sword = null;
            context = null;
            GC.Collect();
            cache.Prune();
            
            GC.Collect();
            bool swordCollected = !swordWeakReference.IsAlive;
            swordCollected.Should().BeTrue();
        }
#endif

        [Fact]
        public void UncollectedScopeInstancesAreNotRemoved()
        {
            var sword = new Sword();
            var swordWeakReference = new WeakReference(sword);
            var context = CreateContextMock(new TestObject(42), this.bindingConfigurationMock.Object);

            this.Remember(sword, context);
            GC.Collect();
            bool swordCollected = !swordWeakReference.IsAlive;

            swordCollected.Should().BeFalse();
        }

        private static IContext CreateContextMock(object scope, IBindingConfiguration bindingConfiguration, params Type[] genericArguments)
        {
            var bindingMock = new Mock<IBinding>();
            bindingMock.Setup(b => b.BindingConfiguration).Returns(bindingConfiguration);
            return new ContextMock(scope, bindingMock.Object, genericArguments);
        }

        private void Remember(Sword sword, IContext context)
        {
            this.cache.Remember(context, new InstanceReference { Instance = sword });
        }
    }

    public class PipelineMock : IPipeline
    {
        public void Dispose()
        {
        }

        public INinjectSettings Settings
        {
            get;
            set;
        }

        public void Activate(IContext context, InstanceReference reference)
        {
        }

        public void Deactivate(IContext context, InstanceReference reference)
        {
        }

        public IList<IActivationStrategy> Strategies
        {
            get;
            set;
        }
    }

    public class ContextMock : IContext
    {
        private WeakReference scope;
        public ContextMock(object scope, IBinding binding, Type[] genericArguments)
        {
            this.scope = new WeakReference(scope);
            this.Binding = binding;
            this.GenericArguments = genericArguments;
        }

        public IProvider GetProvider()
        {
            throw new NotImplementedException();
        }

        public object GetScope()
        {
            return this.scope.Target;
        }

        public object Resolve()
        {
            throw new NotImplementedException();
        }

        public IKernel Kernel { get; set; }

        public IRequest Request { get; set; }

        public IBinding Binding { get; private set; }

        public IPlan Plan { get; set; }

        public ICollection<IParameter> Parameters { get; set; }

        public Type[] GenericArguments
        {
            get;
            private set;
        }

        public bool HasInferredGenericArguments
        {
            get
            {
                return this.GenericArguments != null;
            }
        }
    }
}
#endif