using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;
using Ninject.Planning.Bindings.Resolvers;
using Ninject.Selection.Heuristics;
using Ninject.Syntax;
using Xunit;

namespace Ninject.Test.Unit
{
    public class ReadOnlyKernelTests
    {
        private Mock<INinjectSettings> _ninjectSettingsMock;
        private Mock<ICache> _cacheMock;
        private Mock<IPlanner> _plannerMock;
        private Mock<IConstructorScorer> _constructorScorerMock;
        private Mock<IPipeline> _pipelineMock;
        private Mock<IBindingPrecedenceComparer> _bindingPrecedenceComparerMock;
        private Mock<IBindingResolver> _bindingResolverMock1;
        private Mock<IBindingResolver> _bindingResolverMock2;
        private Mock<IMissingBindingResolver> _missingBindingResolverMock1;
        private Mock<IMissingBindingResolver> _missingBindingResolverMock2;
        private IEnumerable<IBindingResolver> _bindingResolvers;
        private IEnumerable<IMissingBindingResolver> _missingBindingResolvers;

        public ReadOnlyKernelTests()
        {
            _ninjectSettingsMock = new Mock<INinjectSettings>(MockBehavior.Strict);
            _cacheMock = new Mock<ICache>(MockBehavior.Strict);
            _plannerMock = new Mock<IPlanner>(MockBehavior.Strict);
            _constructorScorerMock = new Mock<IConstructorScorer>(MockBehavior.Strict);
            _pipelineMock = new Mock<IPipeline>(MockBehavior.Strict);
            _bindingPrecedenceComparerMock = new Mock<IBindingPrecedenceComparer>(MockBehavior.Strict);
            _bindingResolverMock1 = new Mock<IBindingResolver>(MockBehavior.Strict);
            _bindingResolverMock2 = new Mock<IBindingResolver>(MockBehavior.Strict);
            _missingBindingResolverMock1 = new Mock<IMissingBindingResolver>(MockBehavior.Strict);
            _missingBindingResolverMock2 = new Mock<IMissingBindingResolver>(MockBehavior.Strict);

            _bindingResolvers = new List<IBindingResolver>
                {
                    _bindingResolverMock1.Object,
                    _bindingResolverMock2.Object
                };
            _missingBindingResolvers = new List<IMissingBindingResolver>
                {
                    _missingBindingResolverMock1.Object,
                    _missingBindingResolverMock2.Object
                };
        }

        [Fact]
        public void CreateContext_ShouldThrowArgumentNullExceptionWhenRequestIsNull()
        {
            var bindings = new Dictionary<Type, ICollection<IBinding>>();
            var readOnlyKernel = CreateReadOnlyKernel(bindings);

            const IRequest request = null;
            var bindingMock = new Mock<IBinding>(MockBehavior.Strict);

            var actual = Assert.Throws<ArgumentNullException>(() => readOnlyKernel.CreateContext(request, bindingMock.Object));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(request), actual.ParamName);
        }

        [Fact]
        public void CreateContext_ShouldThrowArgumentNullExceptionWhenBindingIsNull()
        {
            var bindings = new Dictionary<Type, ICollection<IBinding>>();
            var readOnlyKernel = CreateReadOnlyKernel(bindings);

            var requestMock = new Mock<IRequest>(MockBehavior.Strict);
            const IBinding binding = null;

            var actual = Assert.Throws<ArgumentNullException>(() => readOnlyKernel.CreateContext(requestMock.Object, binding));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(binding), actual.ParamName);
        }

        [Fact]
        public void CreateRequest_ShouldThrowArgumentNullExceptionWhenServiceIsNull()
        {
            var bindings = new Dictionary<Type, ICollection<IBinding>>();
            var readOnlyKernel = CreateReadOnlyKernel(bindings);

            const Type service = null;
            Func<IBindingMetadata, bool> constraint = bindingMetadata => true;
            var parameters = new IParameter[0];

            var actual = Assert.Throws<ArgumentNullException>(() => readOnlyKernel.CreateRequest(service, constraint, parameters, true, false));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(service), actual.ParamName);
        }

        [Fact]
        public void CreateRequest_ShouldThrowArgumentNullExceptionWhenParametersIsNull()
        {
            var bindings = new Dictionary<Type, ICollection<IBinding>>();
            var readOnlyKernel = CreateReadOnlyKernel(bindings);

            var service = typeof(string);
            Func<IBindingMetadata, bool> constraint = bindingMetadata => true;
            IParameter[] parameters = null;

            var actual = Assert.Throws<ArgumentNullException>(() => readOnlyKernel.CreateRequest(service, constraint, parameters, true, false));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(parameters), actual.ParamName);
        }

        [Fact]
        public void Inject_ShouldThrowArgumentNullExceptionWhenInstanceIsNull()
        {
            var bindings = new Dictionary<Type, ICollection<IBinding>>();
            var readOnlyKernel = CreateReadOnlyKernel(bindings);

            const object instance = null;
            var parameters = new IParameter[0];

            var actual = Assert.Throws<ArgumentNullException>(() => readOnlyKernel.Inject(instance, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(instance), actual.ParamName);
        }

        [Fact]
        public void Inject_ShouldThrowArgumentNullExceptionWhenParametersIsNull()
        {
            var bindings = new Dictionary<Type, ICollection<IBinding>>();
            var readOnlyKernel = CreateReadOnlyKernel(bindings);

            var instance = new object();
            IParameter[] parameters = null;

            var actual = Assert.Throws<ArgumentNullException>(() => readOnlyKernel.Inject(instance, parameters));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(parameters), actual.ParamName);
        }

        private MyReadOnlyKernel CreateReadOnlyKernel(IDictionary<Type, ICollection<IBinding>> bindings)
        {
            var sequence = new MockSequence();
            _bindingResolverMock1.InSequence(sequence)
                                 .Setup(p => p.Resolve(It.IsAny<IDictionary<Type, ICollection<IBinding>>>(), typeof(IReadOnlyKernel)))
                                 .Returns(Enumerable.Empty<IBinding>);
            _bindingResolverMock2.InSequence(sequence)
                                 .Setup(p => p.Resolve(It.IsAny<IDictionary<Type, ICollection<IBinding>>>(), typeof(IReadOnlyKernel)))
                                 .Returns(Enumerable.Empty<IBinding>);
            _bindingResolverMock1.InSequence(sequence)
                                 .Setup(p => p.Resolve(It.IsAny<IDictionary<Type, ICollection<IBinding>>>(), typeof(IResolutionRoot)))
                                 .Returns(Enumerable.Empty<IBinding>);
            _bindingResolverMock2.InSequence(sequence)
                                 .Setup(p => p.Resolve(It.IsAny<IDictionary<Type, ICollection<IBinding>>>(), typeof(IResolutionRoot)))
                                 .Returns(Enumerable.Empty<IBinding>);

            return new MyReadOnlyKernel(_ninjectSettingsMock.Object,
                                        bindings,
                                        _cacheMock.Object,
                                        _plannerMock.Object,
                                        _constructorScorerMock.Object,
                                        _pipelineMock.Object,
                                        _bindingPrecedenceComparerMock.Object,
                                        _bindingResolvers,
                                        _missingBindingResolvers);
        }

        public class MyReadOnlyKernel : ReadOnlyKernel
        {
            internal MyReadOnlyKernel(INinjectSettings settings,
                                      IDictionary<Type, ICollection<IBinding>> bindings,
                                      ICache cache,
                                      IPlanner planner,
                                      IConstructorScorer constructorScorer,
                                      IPipeline pipeline,
                                      IBindingPrecedenceComparer bindingPrecedenceComparer,
                                      IEnumerable<IBindingResolver> bindingResolvers,
                                      IEnumerable<IMissingBindingResolver> missingBindingResolvers)
                : base(settings,
                       bindings,
                       cache,
                       planner,
                       constructorScorer,
                       pipeline,
                       bindingPrecedenceComparer,
                       bindingResolvers,
                       missingBindingResolvers)
            {
            }

            public new IContext CreateContext(IRequest request, IBinding binding)
            {
                return base.CreateContext(request, binding);
            }
        }
    }
}
