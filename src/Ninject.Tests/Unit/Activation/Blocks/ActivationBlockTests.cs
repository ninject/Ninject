namespace Ninject.Tests.Unit.Activation.Blocks
{
    using Moq;
    using Ninject.Activation;
    using Ninject.Activation.Blocks;
    using Ninject.Parameters;
    using Ninject.Planning.Bindings;
    using Ninject.Syntax;
    using Ninject.Tests.Fakes;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class ActivationBlockTests
    {
        private Mock<IResolutionRoot> _parentMock;
        private Mock<IRequest> _requestMock;
        private ActivationBlock _activationBlock;

        public ActivationBlockTests()
        {
            _parentMock = new Mock<IResolutionRoot>(MockBehavior.Strict);
            _requestMock = new Mock<IRequest>(MockBehavior.Strict);

            _activationBlock = new ActivationBlock(_parentMock.Object);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullExceptionWhenParentIsNull()
        {
            const IResolutionRoot parent = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new ActivationBlock(parent));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(parent), actual.ParamName);
        }

        [Fact]
        public void CanResolve_Request_DelegatesCallToParent()
        {
            _parentMock.Setup(p => p.CanResolve(_requestMock.Object)).Returns(true);

            Assert.True(_activationBlock.CanResolve(_requestMock.Object));
            _parentMock.Verify(p => p.CanResolve(_requestMock.Object));

            _parentMock.Reset();

            _parentMock.Setup(p => p.CanResolve(_requestMock.Object)).Returns(false);

            Assert.False(_activationBlock.CanResolve(_requestMock.Object));
            _parentMock.Verify(p => p.CanResolve(_requestMock.Object));
        }

        [Fact]
        public void CanResolve_Request_ShouldNotPerformArgumentNullCheck()
        {
            const IRequest request = null;

            _parentMock.Setup(p => p.CanResolve(request)).Returns(true);

            Assert.True(_activationBlock.CanResolve(request));
            _parentMock.Verify(p => p.CanResolve(request));
        }

        [Fact]
        public void CanResolve_RequestAndIgnoreImplicitBindings_DelegatesCallToParent()
        {
            _parentMock.Setup(p => p.CanResolve(_requestMock.Object, true)).Returns(false);

            Assert.False(_activationBlock.CanResolve(_requestMock.Object, true));
            _parentMock.Verify(p => p.CanResolve(_requestMock.Object, true));

            _parentMock.Reset();

            _parentMock.Setup(p => p.CanResolve(_requestMock.Object, false)).Returns(true);

            Assert.True(_activationBlock.CanResolve(_requestMock.Object, false));
            _parentMock.Verify(p => p.CanResolve(_requestMock.Object, false));
        }

        [Fact]
        public void CanResolve_RequestAndIgnoreImplicitBindings_ShouldNotPerformArgumentNullCheck()
        {
            const IRequest request = null;

            _parentMock.Setup(p => p.CanResolve(request, false)).Returns(false);

            Assert.False(_activationBlock.CanResolve(request, false));
            _parentMock.Verify(p => p.CanResolve(request, false));
        }

        [Fact]
        public void CreateRequest_ShouldReturnCreatedRequest()
        {
            var service = typeof(Ninja);
            Func<IBindingMetadata, bool> constraint = (bindingMetadata) => false;
            IParameter[] parameters = new[] {
                                                new ConstructorArgument("a", 1),
                                                new ConstructorArgument("b", 2)
                                            };
            var isOptional = true;
            var isUnique = false;

            var request = _activationBlock.CreateRequest(service, constraint, parameters, isOptional, isUnique);

            Assert.NotNull(request);
            Assert.NotNull(request.ActiveBindings);
            Assert.Empty(request.ActiveBindings);
            Assert.Same(constraint, request.Constraint);
            Assert.Equal(0, request.Depth);
            Assert.False(request.ForceUnique);
            Assert.Equal(isOptional, request.IsOptional);
            Assert.Equal(isUnique, request.IsUnique);
            Assert.Equal(parameters, request.Parameters);
            Assert.Null(request.ParentRequest);
            Assert.Null(request.ParentContext);
            Assert.Same(service, request.Service);
            Assert.Null(request.Target);

            var scope = request.GetScope();
            Assert.NotNull(scope);
            Assert.Same(_activationBlock, scope);
        }

        [Fact]
        public void CreateRequest_ShouldThrowArgumentNullExceptionWhenServiceIsNull()
        {
            const Type service = null;
            Func<IBindingMetadata, bool> constraint = null;
            var parameters = Array.Empty<IParameter>();
            var isOptional = true;
            var isUnique = false;

            var actual = Assert.Throws<ArgumentNullException>(() => _activationBlock.CreateRequest(service, constraint, parameters, isOptional, isUnique));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(service), actual.ParamName);
        }

        [Fact]
        public void CreateRequest_ShouldThrowArgumentNullExceptionWhenParametersIsNull()
        {
            var service = GetType();
            Func<IBindingMetadata, bool> constraint = null;
            const IParameter[] parameters = null;
            var isOptional = true;
            var isUnique = false;

            var actual = Assert.Throws<ArgumentNullException>(() => _activationBlock.CreateRequest(service, constraint, parameters, isOptional, isUnique));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(parameters), actual.ParamName);
        }

        [Fact]
        public void Inject_DelegatesCallToParent()
        {
            var instance = new object();
            var parameters = new IParameter[0];

            _parentMock.Setup(p => p.Inject(instance, parameters));

            _activationBlock.Inject(instance, parameters);

            _parentMock.Verify(p => p.Inject(instance, parameters));
        }

        [Fact]
        public void Inject_ShouldNotPerformArgumentNullCheck()
        {
            const object instance = null;
            const IParameter[] parameters = null;

            _parentMock.Setup(p => p.Inject(instance, parameters));

            _activationBlock.Inject(instance, parameters);

            _parentMock.Verify(p => p.Inject(instance, parameters));
        }

        [Fact]
        public void Resolve_DelegatesCallToParent()
        {
            var instancesMock = new Mock<IEnumerable<object>>(MockBehavior.Strict);

            _parentMock.Setup(p => p.Resolve(_requestMock.Object)).Returns(instancesMock.Object);

            var actual = _activationBlock.Resolve(_requestMock.Object);

            Assert.NotNull(actual);
            Assert.Same(instancesMock.Object, actual);

            _parentMock.Verify(p => p.Resolve(_requestMock.Object), Times.Once());
        }

        [Fact]
        public void Resolve_ShouldNotPerformArgumentNullCheck()
        {
            const IRequest request = null;
            var instancesMock = new Mock<IEnumerable<object>>(MockBehavior.Strict);

            _parentMock.Setup(p => p.Resolve(request)).Returns(instancesMock.Object);

            var actual = _activationBlock.Resolve(request);

            Assert.NotNull(actual);
            Assert.Same(instancesMock.Object, actual);

            _parentMock.Verify(p => p.Resolve(request), Times.Once());
        }
    }
}
