﻿using Moq;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Components;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;
using Ninject.Tests.Fakes;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Ninject.Tests.Unit.Activation
{
    public class ContextTests
    {
        private Mock<IReadOnlyKernel> _kernelMock;
        private Mock<INinjectSettings> _settingsMock;
        private Mock<IRequest> _requestMock;
        private Mock<IBinding> _bindingMock;
        private Mock<ICache> _cacheMock;
        private Mock<IPlanner> _plannerMock;
        private Mock<IPipeline> _pipelineMock;
        private Mock<IExceptionFormatter> _exceptionFormatterMock;
        private Mock<IProvider> _providerMock;
        private Mock<IPlan> _planMock;
        private MockSequence _mockSequence;

        private ConstructorArgument _constructorArgName;
        private ConstructorArgument _constructorArgId;
        private ConstructorArgument _constructorArgDate;
        private PropertyValue _propertyFirstName;

        public ContextTests()
        {
            _kernelMock = new Mock<IReadOnlyKernel>(MockBehavior.Strict);
            _settingsMock = new Mock<INinjectSettings>(MockBehavior.Strict);
            _requestMock = new Mock<IRequest>(MockBehavior.Strict);
            _bindingMock = new Mock<IBinding>(MockBehavior.Strict);
            _cacheMock = new Mock<ICache>(MockBehavior.Strict);
            _plannerMock = new Mock<IPlanner>(MockBehavior.Strict);
            _pipelineMock = new Mock<IPipeline>(MockBehavior.Strict);
            _exceptionFormatterMock = new Mock<IExceptionFormatter>(MockBehavior.Strict);
            _providerMock = new Mock<IProvider>(MockBehavior.Strict);
            _planMock = new Mock<IPlan>(MockBehavior.Strict);

            _mockSequence = new MockSequence();

            _constructorArgName = new ConstructorArgument("name", "foo");
            _constructorArgId = new ConstructorArgument("id", 7);
            _constructorArgDate = new ConstructorArgument("date", DateTime.Now);
            _propertyFirstName = new PropertyValue("FirstName", "Michael");
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullExceptionWhenKernelIsNull()
        {
            const IReadOnlyKernel kernel = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new Context(kernel,
                                                                                _settingsMock.Object,
                                                                                _requestMock.Object,
                                                                                _bindingMock.Object,
                                                                                _cacheMock.Object,
                                                                                _plannerMock.Object,
                                                                                _pipelineMock.Object,
                                                                                _exceptionFormatterMock.Object));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(kernel), actual.ParamName);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullExceptionWhenSettingsIsNull()
        {
            const INinjectSettings settings = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new Context(_kernelMock.Object,
                                                                                settings,
                                                                                _requestMock.Object,
                                                                                _bindingMock.Object,
                                                                                _cacheMock.Object,
                                                                                _plannerMock.Object,
                                                                                _pipelineMock.Object,
                                                                                _exceptionFormatterMock.Object));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(settings), actual.ParamName);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullExceptionWhenRequestIsNull()
        {
            const IRequest request = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new Context(_kernelMock.Object,
                                                                                _settingsMock.Object,
                                                                                request,
                                                                                _bindingMock.Object,
                                                                                _cacheMock.Object,
                                                                                _plannerMock.Object,
                                                                                _pipelineMock.Object,
                                                                                _exceptionFormatterMock.Object));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(request), actual.ParamName);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullExceptionWhenBindingIsNull()
        {
            const IBinding binding = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new Context(_kernelMock.Object,
                                                                                _settingsMock.Object,
                                                                                _requestMock.Object,
                                                                                binding,
                                                                                _cacheMock.Object,
                                                                                _plannerMock.Object,
                                                                                _pipelineMock.Object,
                                                                                _exceptionFormatterMock.Object));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(binding), actual.ParamName);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullExceptionWhenCacheIsNull()
        {
            const ICache cache = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new Context(_kernelMock.Object,
                                                                                _settingsMock.Object,
                                                                                _requestMock.Object,
                                                                                _bindingMock.Object,
                                                                                cache,
                                                                                _plannerMock.Object,
                                                                                _pipelineMock.Object,
                                                                                _exceptionFormatterMock.Object));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(cache), actual.ParamName);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullExceptionWhenPlannerIsNull()
        {
            const IPlanner planner = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new Context(_kernelMock.Object,
                                                                                _settingsMock.Object,
                                                                                _requestMock.Object,
                                                                                _bindingMock.Object,
                                                                                _cacheMock.Object,
                                                                                planner,
                                                                                _pipelineMock.Object,
                                                                                _exceptionFormatterMock.Object));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(planner), actual.ParamName);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullExceptionWhenPipelineIsNull()
        {
            const IPipeline pipeline = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new Context(_kernelMock.Object,
                                                                                _settingsMock.Object,
                                                                                _requestMock.Object,
                                                                                _bindingMock.Object,
                                                                                _cacheMock.Object,
                                                                                _plannerMock.Object,
                                                                                pipeline,
                                                                                _exceptionFormatterMock.Object));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(pipeline), actual.ParamName);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullExceptionWhenExceptionFormatterIsNull()
        {
            const IExceptionFormatter exceptionFormatter = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new Context(_kernelMock.Object,
                                                                                _settingsMock.Object,
                                                                                _requestMock.Object,
                                                                                _bindingMock.Object,
                                                                                _cacheMock.Object,
                                                                                _plannerMock.Object,
                                                                                _pipelineMock.Object,
                                                                                exceptionFormatter));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(exceptionFormatter), actual.ParamName);
        }

        [Fact]
        public void Constructor_BindingAndRequestDefineParameters()
        {
            IEnumerable<IParameter> requestParameters = new IParameter[] { _constructorArgDate, _constructorArgName};
            ICollection<IParameter> bindingParameters = new IParameter[] { _propertyFirstName, _constructorArgDate, _constructorArgId };
            Type service = typeof(Dagger);

            _requestMock.Setup(p => p.Parameters).Returns(requestParameters);
            _bindingMock.Setup(p => p.Parameters).Returns(bindingParameters);
            _bindingMock.Setup(p => p.Service).Returns(service);

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);

            var parameters = context.Parameters;

            Assert.NotNull(parameters);
            Assert.Equal(new IParameter[] { _constructorArgDate, _constructorArgName, _propertyFirstName, _constructorArgId }, context.Parameters);
            Assert.Same(parameters, context.Parameters);
        }

        [Fact]
        public void Constructor_RequestDefinesParameters()
        {
            IEnumerable<IParameter> requestParameters = new IParameter[] { _constructorArgDate, _constructorArgName };
            ICollection<IParameter> bindingParameters = Array.Empty<IParameter>();
            Type service = typeof(Dagger);

            _requestMock.Setup(p => p.Parameters).Returns(requestParameters);
            _bindingMock.Setup(p => p.Parameters).Returns(bindingParameters);
            _bindingMock.Setup(p => p.Service).Returns(service);

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);

            var parameters = context.Parameters;

            Assert.NotNull(parameters);
            Assert.Equal(new IParameter[] { _constructorArgDate, _constructorArgName }, context.Parameters);
            Assert.Same(parameters, context.Parameters);
        }

        [Fact]
        public void Constructor_BindingDefinesParameters()
        {
            IEnumerable<IParameter> requestParameters = Enumerable.Empty<IParameter>();
            ICollection<IParameter> bindingParameters = new IParameter[] { _propertyFirstName, _constructorArgId };
            Type service = typeof(Dagger);

            _requestMock.Setup(p => p.Parameters).Returns(requestParameters);
            _bindingMock.Setup(p => p.Parameters).Returns(bindingParameters);
            _bindingMock.Setup(p => p.Service).Returns(service);

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);

            var parameters = context.Parameters;

            Assert.NotNull(parameters);
            Assert.Equal(new IParameter[] { _propertyFirstName, _constructorArgId }, context.Parameters);
            Assert.Same(parameters, context.Parameters);
        }

        [Fact]
        public void Constructor_NoParameters()
        {
            IEnumerable<IParameter> requestParameters = Enumerable.Empty<IParameter>();
            ICollection<IParameter> bindingParameters = Array.Empty<IParameter>();
            Type bindingService = typeof(Dagger);

            _requestMock.Setup(p => p.Parameters).Returns(requestParameters);
            _bindingMock.Setup(p => p.Parameters).Returns(bindingParameters);
            _bindingMock.Setup(p => p.Service).Returns(bindingService);

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);

            var parameters = context.Parameters;

            Assert.NotNull(parameters);
            Assert.Empty(context.Parameters);
            Assert.Same(parameters, context.Parameters);
        }

        [Fact]
        public void Constructor_BindingServiceIsGenericTypeDefinition_GenericArguments()
        {
            Type bindingService = typeof(IDictionary<,>);
            Type requestService = typeof(Dictionary<int,string>);

            _requestMock.Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.Setup(p => p.Service).Returns(bindingService);
            _requestMock.Setup(p => p.Service).Returns(requestService);

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);

            Assert.True(context.HasInferredGenericArguments);
            Assert.Equal(new[] { typeof(int), typeof(string) }, context.GenericArguments);
        }

        [Fact]
        public void Constructor_BindingServiceIsGenericTypeDefinition_NoGenericArguments()
        {
            Type bindingService = typeof(IDictionary<,>);
            Type requestService = typeof(string);

            _requestMock.Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.Setup(p => p.Service).Returns(bindingService);
            _requestMock.Setup(p => p.Service).Returns(requestService);

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);

            Assert.True(context.HasInferredGenericArguments);
            Assert.Empty(context.GenericArguments);
        }

        [Fact]
        public void Constructor_BindingServiceIsNoGenericTypeDefinition()
        {
            Type bindingService = typeof(Dagger);

            _requestMock.Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.Setup(p => p.Service).Returns(bindingService);

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);

            Assert.False(context.HasInferredGenericArguments);
        }

        [Fact]
        public void GetScope_WithoutRequestScopeButWithBindingScope_BeforeResolve()
        {
            var bindingScope = new object();

            _requestMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(typeof(Dagger));

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);

            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(null);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetScope(context)).Returns(bindingScope);

            var scope = context.GetScope();

            Assert.NotNull(scope);
            Assert.Same(bindingScope, scope);

            _requestMock.Verify(p => p.GetScope(), Times.Once());
            _bindingMock.Verify(p => p.GetScope(context), Times.Once());

            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(null);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetScope(context)).Returns(bindingScope);

            Assert.Same(bindingScope, context.GetScope());

            _requestMock.Verify(p => p.GetScope(), Times.Exactly(2));
            _bindingMock.Verify(p => p.GetScope(context), Times.Exactly(2));
        }

        [Fact]
        public void GetScope_WithoutRequestScopeButWithBindingScope_DuringResolve()
        {
            var bindingScope = new object();
            object bindingScopeDuringResolve = null;

            _requestMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(typeof(Dagger));

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);

            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(new Stack<IBinding>());
            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(null);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetScope(context)).Returns(bindingScope);
            _cacheMock.InSequence(_mockSequence)
                      .Setup(p => p.TryGet(context))
                      .Returns(new object())
                      .Callback(() => bindingScopeDuringResolve = context.GetScope());

            context.Resolve();

            Assert.NotNull(bindingScopeDuringResolve);
            Assert.Same(bindingScope, bindingScopeDuringResolve);

            _requestMock.Verify(p => p.GetScope(), Times.Once());
            _bindingMock.Verify(p => p.GetScope(context), Times.Once());

            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(null);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetScope(context)).Returns(bindingScope);

            Assert.Same(bindingScope, context.GetScope());

            _requestMock.Verify(p => p.GetScope(), Times.Exactly(2));
            _bindingMock.Verify(p => p.GetScope(context), Times.Exactly(2));
        }

        [Fact]
        public void GetScope_WithoutRequestScopeButWithBindingScope_AfterResolve()
        {
            var bindingScope = new object();

            _requestMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(typeof(Dagger));

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);

            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(new Stack<IBinding>());
            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(null);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetScope(context)).Returns(bindingScope);
            _cacheMock.InSequence(_mockSequence).Setup(p => p.TryGet(context)).Returns(new object());

            context.Resolve();

            _requestMock.Verify(p => p.GetScope(), Times.Once());
            _bindingMock.Verify(p => p.GetScope(context), Times.Once());

            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(null);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetScope(context)).Returns(bindingScope);

            Assert.Same(bindingScope, context.GetScope());

            _requestMock.Verify(p => p.GetScope(), Times.Exactly(2));
            _bindingMock.Verify(p => p.GetScope(context), Times.Exactly(2));
        }

        [Fact]
        public void Resolve_CachedInstance_ScopeIsNull()
        {
            const object bindingScope = null;
            var activeBindings = new Stack<IBinding>();
            var cachedInstance = new object();

            _requestMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(typeof(Dagger));

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);

            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(bindingScope);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetScope(context)).Returns(bindingScope);
            _cacheMock.InSequence(_mockSequence).Setup(p => p.TryGet(context)).Returns(cachedInstance);

            var instance = context.Resolve();

            Assert.NotNull(instance);
            Assert.Same(cachedInstance, instance);
            Assert.Empty(activeBindings);

            Assert.Null(context.Plan);

            _requestMock.Verify(p => p.ActiveBindings, Times.Once());
            _requestMock.Verify(p => p.GetScope(), Times.Once());
            _bindingMock.Verify(p => p.GetScope(context), Times.Once());
            _cacheMock.Verify(p => p.TryGet(context), Times.Once());
        }

        [Fact]
        public void Resolve_CachedInstance_ScopeIsNotNull()
        {
            var bindingScope = new object();
            var activeBindings = new Stack<IBinding>();
            var cachedInstance = new object();

            _requestMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(typeof(Dagger));

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);

            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(bindingScope);
            _cacheMock.InSequence(_mockSequence).Setup(p => p.TryGet(context)).Returns(cachedInstance);

            var instance = context.Resolve();

            Assert.NotNull(instance);
            Assert.Same(cachedInstance, instance);
            Assert.Empty(activeBindings);

            Assert.Null(context.Plan);

            _requestMock.Verify(p => p.ActiveBindings, Times.Once());
            _requestMock.Verify(p => p.GetScope(), Times.Once());
            _cacheMock.Verify(p => p.TryGet(context), Times.Once());
        }

        [Fact]
        public void Resolve_NewInstanceIsNotNull_ScopeIsNull_PlanIsNull()
        {
            const object bindingScope = null;
            var activeBindings = new Stack<IBinding>();
            const object cachedInstance = null;
            var newInstance = new Monk();
            InstanceReference activatedReference = null;

            _requestMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(typeof(Dagger));

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);

            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(bindingScope);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetScope(context)).Returns(bindingScope);
            _cacheMock.InSequence(_mockSequence).Setup(p => p.TryGet(context)).Returns(cachedInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetProvider(context)).Returns(_providerMock.Object);
            _providerMock.InSequence(_mockSequence).Setup(p => p.Create(context)).Returns(newInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _plannerMock.InSequence(_mockSequence).Setup(p => p.GetPlan(newInstance.GetType())).Returns(_planMock.Object);
            _pipelineMock.InSequence(_mockSequence)
                         .Setup(p => p.Activate(context, It.IsNotNull<InstanceReference>()))
                         .Callback<IContext, InstanceReference>((ctx, reference) => activatedReference = reference);

            var instance = context.Resolve();

            Assert.NotNull(instance);
            Assert.Same(newInstance, instance);
            Assert.Empty(activeBindings);
            Assert.NotNull(activatedReference);
            Assert.Same(newInstance, activatedReference.Instance);

            Assert.NotNull(context.Plan);
            Assert.Same(_planMock.Object, context.Plan);

            _requestMock.Verify(p => p.ActiveBindings, Times.Exactly(3));
            _requestMock.Verify(p => p.GetScope(), Times.Once());
            _cacheMock.Verify(p => p.TryGet(context), Times.Once());
            _providerMock.Verify(p => p.Create(context), Times.Once());
            _plannerMock.Verify(p => p.GetPlan(newInstance.GetType()), Times.Once());
            _pipelineMock.Verify(p => p.Activate(context, It.IsNotNull<InstanceReference>()), Times.Once());
        }

        [Fact]
        public void Resolve_NewInstanceIsNotNull_ScopeIsNull_PlanIsNotNull()
        {
            const object bindingScope = null;
            var activeBindings = new Stack<IBinding>();
            const object cachedInstance = null;
            var newInstance = new Monk();
            InstanceReference activatedReference = null;

            _requestMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(typeof(Dagger));

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);
            context.Plan = _planMock.Object;

            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(bindingScope);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetScope(context)).Returns(bindingScope);
            _cacheMock.InSequence(_mockSequence).Setup(p => p.TryGet(context)).Returns(cachedInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetProvider(context)).Returns(_providerMock.Object);
            _providerMock.InSequence(_mockSequence).Setup(p => p.Create(context)).Returns(newInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _pipelineMock.InSequence(_mockSequence)
                         .Setup(p => p.Activate(context, It.IsNotNull<InstanceReference>()))
                         .Callback<IContext, InstanceReference>((ctx, reference) => activatedReference = reference);

            var instance = context.Resolve();

            Assert.NotNull(instance);
            Assert.Same(newInstance, instance);
            Assert.Empty(activeBindings);
            Assert.NotNull(activatedReference);
            Assert.Same(newInstance, activatedReference.Instance);

            Assert.NotNull(context.Plan);
            Assert.Same(_planMock.Object, context.Plan);

            _requestMock.Verify(p => p.ActiveBindings, Times.Exactly(3));
            _requestMock.Verify(p => p.GetScope(), Times.Once());
            _cacheMock.Verify(p => p.TryGet(context), Times.Once());
            _providerMock.Verify(p => p.Create(context), Times.Once());
            _pipelineMock.Verify(p => p.Activate(context, It.IsNotNull<InstanceReference>()), Times.Once());
        }

        [Fact]
        public void Resolve_NewInstanceIsNotNull_ScopeIsNotNull_PlanIsNull()
        {
            var bindingScope = new object();
            var activeBindings = new Stack<IBinding>();
            const object cachedInstance = null;
            var newInstance = new Monk();
            InstanceReference cachedReference = null;
            InstanceReference activatedReference = null;

            _requestMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(typeof(Dagger));

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);

            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(bindingScope);
            _cacheMock.InSequence(_mockSequence).Setup(p => p.TryGet(context)).Returns(cachedInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetProvider(context)).Returns(_providerMock.Object);
            _providerMock.InSequence(_mockSequence).Setup(p => p.Create(context)).Returns(newInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _cacheMock.InSequence(_mockSequence)
                         .Setup(p => p.Remember(context, It.IsNotNull<InstanceReference>()))
                         .Callback<IContext, InstanceReference>((ctx, reference) => cachedReference = reference);
            _plannerMock.InSequence(_mockSequence).Setup(p => p.GetPlan(newInstance.GetType())).Returns(_planMock.Object);
            _pipelineMock.InSequence(_mockSequence)
                         .Setup(p => p.Activate(context, It.IsNotNull<InstanceReference>()))
                         .Callback<IContext, InstanceReference>((ctx, reference) => activatedReference = reference);

            var instance = context.Resolve();

            Assert.NotNull(instance);
            Assert.Same(newInstance, instance);
            Assert.Empty(activeBindings);
            Assert.NotNull(cachedReference);
            Assert.Same(newInstance, cachedReference.Instance);
            Assert.Same(cachedReference, activatedReference);

            Assert.NotNull(context.Plan);
            Assert.Same(_planMock.Object, context.Plan);

            _requestMock.Verify(p => p.ActiveBindings, Times.Exactly(3));
            _requestMock.Verify(p => p.GetScope(), Times.Once());
            _cacheMock.Verify(p => p.TryGet(context), Times.Once());
            _providerMock.Verify(p => p.Create(context), Times.Once());
            _cacheMock.Verify(p => p.Remember(context, It.IsNotNull<InstanceReference>()), Times.Once());
            _plannerMock.Verify(p => p.GetPlan(newInstance.GetType()), Times.Once());
            _pipelineMock.Verify(p => p.Activate(context, It.IsNotNull<InstanceReference>()), Times.Once());
        }

        [Fact]
        public void Resolve_NewInstanceIsNotNull_ScopeIsNull_PipelineActivateThrowsActivationException()
        {
            const object bindingScope = null;
            var activeBindings = new Stack<IBinding>();
            const object cachedInstance = null;
            var newInstance = new Monk();
            InstanceReference activatedReference = null;
            var activationException = new ActivationException();

            _requestMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(typeof(Dagger));

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);
            context.Plan = _planMock.Object;

            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(bindingScope);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetScope(context)).Returns(bindingScope);
            _cacheMock.InSequence(_mockSequence).Setup(p => p.TryGet(context)).Returns(cachedInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetProvider(context)).Returns(_providerMock.Object);
            _providerMock.InSequence(_mockSequence).Setup(p => p.Create(context)).Returns(newInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _pipelineMock.InSequence(_mockSequence)
                         .Setup(p => p.Activate(context, It.IsNotNull<InstanceReference>()))
                         .Callback<IContext, InstanceReference>((ctx, reference) => activatedReference = reference)
                         .Throws(activationException);

            var actualException = Assert.Throws<ActivationException>(() => context.Resolve());

            Assert.Empty(activeBindings);
            Assert.NotNull(activatedReference);
            Assert.Same(newInstance, activatedReference.Instance);
            Assert.NotNull(context.Plan);
            Assert.Same(_planMock.Object, context.Plan);

            _requestMock.Verify(p => p.ActiveBindings, Times.Exactly(3));
            _requestMock.Verify(p => p.GetScope(), Times.Once());
            _cacheMock.Verify(p => p.TryGet(context), Times.Once());
            _providerMock.Verify(p => p.Create(context), Times.Once());
            _pipelineMock.Verify(p => p.Activate(context, It.IsNotNull<InstanceReference>()), Times.Once());
        }

        [Fact]
        public void Resolve_NewInstanceIsNotNull_ScopeIsNotNull_PipelineActivateThrowsActivationException()
        {
            var bindingScope = new object();
            var activeBindings = new Stack<IBinding>();
            const object cachedInstance = null;
            var newInstance = new Monk();
            InstanceReference cachedReference = null;
            InstanceReference activatedReference = null;
            var activationException = new ActivationException();

            _requestMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(typeof(Dagger));

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);
            context.Plan = _planMock.Object;

            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(bindingScope);
            _cacheMock.InSequence(_mockSequence).Setup(p => p.TryGet(context)).Returns(cachedInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetProvider(context)).Returns(_providerMock.Object);
            _providerMock.InSequence(_mockSequence).Setup(p => p.Create(context)).Returns(newInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _cacheMock.InSequence(_mockSequence)
                         .Setup(p => p.Remember(context, It.IsNotNull<InstanceReference>()))
                         .Callback<IContext, InstanceReference>((ctx, reference) => cachedReference = reference);
            _pipelineMock.InSequence(_mockSequence)
                         .Setup(p => p.Activate(context, It.IsNotNull<InstanceReference>()))
                         .Callback<IContext, InstanceReference>((ctx, reference) => activatedReference = reference)
                         .Throws(activationException);
            _cacheMock.InSequence(_mockSequence).Setup(p => p.Release(newInstance)).Returns(true);

            var actualException = Assert.Throws<ActivationException>(() => context.Resolve());

            Assert.Empty(activeBindings);
            Assert.NotNull(cachedReference);
            Assert.Same(newInstance, cachedReference.Instance);
            Assert.NotNull(activatedReference);
            Assert.Same(cachedReference, activatedReference);
            Assert.NotNull(context.Plan);
            Assert.Same(_planMock.Object, context.Plan);

            _requestMock.Verify(p => p.ActiveBindings, Times.Exactly(3));
            _requestMock.Verify(p => p.GetScope(), Times.Once());
            _cacheMock.Verify(p => p.TryGet(context), Times.Once());
            _providerMock.Verify(p => p.Create(context), Times.Once());
            _cacheMock.Verify(p => p.Remember(context, It.IsNotNull<InstanceReference>()), Times.Once());
            _pipelineMock.Verify(p => p.Activate(context, It.IsNotNull<InstanceReference>()), Times.Once());
            _cacheMock.Verify(p => p.Release(newInstance), Times.Once());
        }

        [Fact]
        public void Resolve_NewInstanceIsNotNull_ScopeIsNotNull_PlanIsNotNull()
        {
            var bindingScope = new object();
            var activeBindings = new Stack<IBinding>();
            const object cachedInstance = null;
            var newInstance = new Monk();
            InstanceReference cachedReference = null;
            InstanceReference activatedReference = null;

            _requestMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(typeof(Dagger));

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);
            context.Plan = _planMock.Object;

            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(bindingScope);
            _cacheMock.InSequence(_mockSequence).Setup(p => p.TryGet(context)).Returns(cachedInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetProvider(context)).Returns(_providerMock.Object);
            _providerMock.InSequence(_mockSequence).Setup(p => p.Create(context)).Returns(newInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _cacheMock.InSequence(_mockSequence)
                         .Setup(p => p.Remember(context, It.IsNotNull<InstanceReference>()))
                         .Callback<IContext, InstanceReference>((ctx, reference) => cachedReference = reference);
            _pipelineMock.InSequence(_mockSequence)
                         .Setup(p => p.Activate(context, It.IsNotNull<InstanceReference>()))
                         .Callback<IContext, InstanceReference>((ctx, reference) => activatedReference = reference);

            var instance = context.Resolve();

            Assert.NotNull(instance);
            Assert.Same(newInstance, instance);
            Assert.Empty(activeBindings);
            Assert.NotNull(cachedReference);
            Assert.Same(newInstance, cachedReference.Instance);
            Assert.NotNull(activatedReference);
            Assert.Same(newInstance, activatedReference.Instance);

            Assert.NotNull(context.Plan);
            Assert.Same(_planMock.Object, context.Plan);

            _requestMock.Verify(p => p.ActiveBindings, Times.Exactly(3));
            _requestMock.Verify(p => p.GetScope(), Times.Once());
            _cacheMock.Verify(p => p.TryGet(context), Times.Once());
            _providerMock.Verify(p => p.Create(context), Times.Once());
            _cacheMock.Verify(p => p.Remember(context, It.IsNotNull<InstanceReference>()), Times.Once());
            _pipelineMock.Verify(p => p.Activate(context, It.IsNotNull<InstanceReference>()), Times.Once());
        }

        [Fact]
        public void Resolve_NewInstanceIsNull_ScopeIsNull_AllowNullInjectionIsFalse_PlanIsNull()
        {
            const object bindingScope = null;
            var activeBindings = new Stack<IBinding>();
            const object cachedInstance = null;
            const object newInstance = null;
            var providerReturnedNullExceptionMessage = new string('a', 1);

            _requestMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(typeof(Dagger));

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);

            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(bindingScope);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetScope(context)).Returns(bindingScope);
            _cacheMock.InSequence(_mockSequence).Setup(p => p.TryGet(context)).Returns(cachedInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetProvider(context)).Returns(_providerMock.Object);
            _providerMock.InSequence(_mockSequence).Setup(p => p.Create(context)).Returns(newInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _settingsMock.InSequence(_mockSequence).Setup(p => p.AllowNullInjection).Returns(false);
            _exceptionFormatterMock.InSequence(_mockSequence)
                                   .Setup(p => p.ProviderReturnedNull(context))
                                   .Returns(providerReturnedNullExceptionMessage);

            var actualException = Assert.Throws<ActivationException>(() => context.Resolve());

            Assert.Null(actualException.InnerException);
            Assert.Same(providerReturnedNullExceptionMessage, actualException.Message);
            Assert.Empty(activeBindings);
            Assert.Null(context.Plan);

            _requestMock.Verify(p => p.ActiveBindings, Times.Exactly(3));
            _requestMock.Verify(p => p.GetScope(), Times.Once());
            _cacheMock.Verify(p => p.TryGet(context), Times.Once());
            _providerMock.Verify(p => p.Create(context), Times.Once());
            _settingsMock.Verify(p => p.AllowNullInjection, Times.Once());
            _exceptionFormatterMock.Verify(p => p.ProviderReturnedNull(context), Times.Once());
        }

        [Fact]
        public void Resolve_NewInstanceIsNull_ScopeIsNull_AllowNullInjectionIsFalse_PlanIsNotNull()
        {
            const object bindingScope = null;
            var activeBindings = new Stack<IBinding>();
            const object cachedInstance = null;
            const object newInstance = null;
            var providerReturnedNullExceptionMessage = new string('a', 1);

            _requestMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(typeof(Dagger));

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);
            context.Plan = _planMock.Object;

            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(bindingScope);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetScope(context)).Returns(bindingScope);
            _cacheMock.InSequence(_mockSequence).Setup(p => p.TryGet(context)).Returns(cachedInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetProvider(context)).Returns(_providerMock.Object);
            _providerMock.InSequence(_mockSequence).Setup(p => p.Create(context)).Returns(newInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _settingsMock.InSequence(_mockSequence).Setup(p => p.AllowNullInjection).Returns(false);
            _exceptionFormatterMock.InSequence(_mockSequence)
                                   .Setup(p => p.ProviderReturnedNull(context))
                                   .Returns(providerReturnedNullExceptionMessage);

            var actualException = Assert.Throws<ActivationException>(() => context.Resolve());

            Assert.Null(actualException.InnerException);
            Assert.Same(providerReturnedNullExceptionMessage, actualException.Message);
            Assert.Empty(activeBindings);
            Assert.NotNull(context.Plan);
            Assert.Same(_planMock.Object, context.Plan);

            _requestMock.Verify(p => p.ActiveBindings, Times.Exactly(3));
            _requestMock.Verify(p => p.GetScope(), Times.Once());
            _cacheMock.Verify(p => p.TryGet(context), Times.Once());
            _providerMock.Verify(p => p.Create(context), Times.Once());
            _settingsMock.Verify(p => p.AllowNullInjection, Times.Once());
            _exceptionFormatterMock.Verify(p => p.ProviderReturnedNull(context), Times.Once());
        }

        [Fact]
        public void Resolve_NewInstanceIsNull_ScopeIsNull_AllowNullInjectionIsTrue_PlanIsNull()
        {
            const object bindingScope = null;
            var activeBindings = new Stack<IBinding>();
            const object cachedInstance = null;
            const object newInstance = null;
            var requestService = typeof(Monk);

            _requestMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(typeof(Dagger));

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);

            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(bindingScope);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetScope(context)).Returns(bindingScope);
            _cacheMock.InSequence(_mockSequence).Setup(p => p.TryGet(context)).Returns(cachedInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetProvider(context)).Returns(_providerMock.Object);
            _providerMock.InSequence(_mockSequence).Setup(p => p.Create(context)).Returns(newInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _settingsMock.InSequence(_mockSequence).Setup(p => p.AllowNullInjection).Returns(true);
            _requestMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(requestService);
            _plannerMock.InSequence(_mockSequence).Setup(p => p.GetPlan(requestService)).Returns(_planMock.Object);

            var instance = context.Resolve();

            Assert.Null(instance);
            Assert.Empty(activeBindings);
            Assert.NotNull(context.Plan);
            Assert.Same(_planMock.Object, context.Plan);

            _requestMock.Verify(p => p.ActiveBindings, Times.Exactly(3));
            _requestMock.Verify(p => p.GetScope(), Times.Once());
            _cacheMock.Verify(p => p.TryGet(context), Times.Once());
            _providerMock.Verify(p => p.Create(context), Times.Once());
            _settingsMock.Verify(p => p.AllowNullInjection, Times.Once());
            _requestMock.Verify(p => p.Service, Times.Once());
            _plannerMock.Verify(p => p.GetPlan(requestService), Times.Once());
        }

        [Fact]
        public void Resolve_NewInstanceIsNull_ScopeIsNull_AllowNullInjectionIsTrue_PlanIsNotNull()
        {
            const object bindingScope = null;
            var activeBindings = new Stack<IBinding>();
            const object cachedInstance = null;
            const object newInstance = null;
            var requestService = typeof(Monk);

            _requestMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(typeof(Dagger));

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);

            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(bindingScope);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetScope(context)).Returns(bindingScope);
            _cacheMock.InSequence(_mockSequence).Setup(p => p.TryGet(context)).Returns(cachedInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetProvider(context)).Returns(_providerMock.Object);
            _providerMock.InSequence(_mockSequence).Setup(p => p.Create(context)).Returns(newInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _settingsMock.InSequence(_mockSequence).Setup(p => p.AllowNullInjection).Returns(true);
            _requestMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(requestService);
            _plannerMock.InSequence(_mockSequence).Setup(p => p.GetPlan(requestService)).Returns(_planMock.Object);

            var instance = context.Resolve();

            Assert.Null(instance);
            Assert.Empty(activeBindings);
            Assert.NotNull(context.Plan);
            Assert.Same(_planMock.Object, context.Plan);

            _requestMock.Verify(p => p.ActiveBindings, Times.Exactly(3));
            _requestMock.Verify(p => p.GetScope(), Times.Once());
            _cacheMock.Verify(p => p.TryGet(context), Times.Once());
            _providerMock.Verify(p => p.Create(context), Times.Once());
            _settingsMock.Verify(p => p.AllowNullInjection, Times.Once());
            _requestMock.Verify(p => p.Service, Times.Once());
            _plannerMock.Verify(p => p.GetPlan(requestService), Times.Once());
        }

        [Fact]
        public void Resolve_NewInstanceIsNull_ScopeIsNotNull_AllowNullInjectionIsFalse_PlanIsNull()
        {
            var bindingScope = new object();
            var activeBindings = new Stack<IBinding>();
            const object cachedInstance = null;
            const object newInstance = null;
            var providerReturnedNullExceptionMessage = new string('a', 1);

            _requestMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(typeof(Dagger));

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);

            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(bindingScope);
            _cacheMock.InSequence(_mockSequence).Setup(p => p.TryGet(context)).Returns(cachedInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetProvider(context)).Returns(_providerMock.Object);
            _providerMock.InSequence(_mockSequence).Setup(p => p.Create(context)).Returns(newInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _settingsMock.InSequence(_mockSequence).Setup(p => p.AllowNullInjection).Returns(false);
            _exceptionFormatterMock.InSequence(_mockSequence)
                                   .Setup(p => p.ProviderReturnedNull(context))
                                   .Returns(providerReturnedNullExceptionMessage);

            var actualException = Assert.Throws<ActivationException>(() => context.Resolve());

            Assert.Null(actualException.InnerException);
            Assert.Same(providerReturnedNullExceptionMessage, actualException.Message);
            Assert.Empty(activeBindings);
            Assert.Null(context.Plan);

            _requestMock.Verify(p => p.ActiveBindings, Times.Exactly(3));
            _requestMock.Verify(p => p.GetScope(), Times.Once());
            _cacheMock.Verify(p => p.TryGet(context), Times.Once());
            _providerMock.Verify(p => p.Create(context), Times.Once());
            _settingsMock.Verify(p => p.AllowNullInjection, Times.Once());
            _exceptionFormatterMock.Verify(p => p.ProviderReturnedNull(context), Times.Once());
        }

        [Fact]
        public void Resolve_NewInstanceIsNull_ScopeIsNotNull_AllowNullInjectionIsFalse_PlanIsNotNull()
        {
            var bindingScope = new object();
            var activeBindings = new Stack<IBinding>();
            const object cachedInstance = null;
            const object newInstance = null;
            var providerReturnedNullExceptionMessage = new string('a', 1);

            _requestMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(typeof(Dagger));

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);
            context.Plan = _planMock.Object;

            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(bindingScope);
            _cacheMock.InSequence(_mockSequence).Setup(p => p.TryGet(context)).Returns(cachedInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetProvider(context)).Returns(_providerMock.Object);
            _providerMock.InSequence(_mockSequence).Setup(p => p.Create(context)).Returns(newInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _settingsMock.InSequence(_mockSequence).Setup(p => p.AllowNullInjection).Returns(false);
            _exceptionFormatterMock.InSequence(_mockSequence)
                                   .Setup(p => p.ProviderReturnedNull(context))
                                   .Returns(providerReturnedNullExceptionMessage);

            var actualException = Assert.Throws<ActivationException>(() => context.Resolve());

            Assert.Null(actualException.InnerException);
            Assert.Same(providerReturnedNullExceptionMessage, actualException.Message);
            Assert.Empty(activeBindings);
            Assert.NotNull(context.Plan);
            Assert.Same(_planMock.Object, context.Plan);

            _requestMock.Verify(p => p.ActiveBindings, Times.Exactly(3));
            _requestMock.Verify(p => p.GetScope(), Times.Once());
            _cacheMock.Verify(p => p.TryGet(context), Times.Once());
            _providerMock.Verify(p => p.Create(context), Times.Once());
            _settingsMock.Verify(p => p.AllowNullInjection, Times.Once());
            _exceptionFormatterMock.Verify(p => p.ProviderReturnedNull(context), Times.Once());
        }

        [Fact]
        public void Resolve_NewInstanceIsNull_ScopeIsNotNull_AllowNullInjectionIsTrue_PlanIsNull()
        {
            var bindingScope = new object();
            var activeBindings = new Stack<IBinding>();
            const object cachedInstance = null;
            const object newInstance = null;
            var requestService = typeof(Monk);

            _requestMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(typeof(Dagger));

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);

            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(bindingScope);
            _cacheMock.InSequence(_mockSequence).Setup(p => p.TryGet(context)).Returns(cachedInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetProvider(context)).Returns(_providerMock.Object);
            _providerMock.InSequence(_mockSequence).Setup(p => p.Create(context)).Returns(newInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _settingsMock.InSequence(_mockSequence).Setup(p => p.AllowNullInjection).Returns(true);
            _requestMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(requestService);
            _plannerMock.InSequence(_mockSequence).Setup(p => p.GetPlan(requestService)).Returns(_planMock.Object);

            var instance = context.Resolve();

            Assert.Null(instance);
            Assert.Empty(activeBindings);
            Assert.NotNull(context.Plan);
            Assert.Same(_planMock.Object, context.Plan);

            _requestMock.Verify(p => p.ActiveBindings, Times.Exactly(3));
            _requestMock.Verify(p => p.GetScope(), Times.Once());
            _cacheMock.Verify(p => p.TryGet(context), Times.Once());
            _providerMock.Verify(p => p.Create(context), Times.Once());
            _settingsMock.Verify(p => p.AllowNullInjection, Times.Once());
            _requestMock.Verify(p => p.Service, Times.Once());
            _plannerMock.Verify(p => p.GetPlan(requestService), Times.Once());
        }

        [Fact]
        public void Resolve_NewInstanceIsNull_ScopeIsNotNull_AllowNullInjectionIsTrue_PlanIsNotNull()
        {
            var bindingScope = new object();
            var activeBindings = new Stack<IBinding>();
            const object cachedInstance = null;
            const object newInstance = null;

            _requestMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Parameters).Returns(Array.Empty<IParameter>());
            _bindingMock.InSequence(_mockSequence).Setup(p => p.Service).Returns(typeof(Dagger));

            var context = new Context(_kernelMock.Object,
                                      _settingsMock.Object,
                                      _requestMock.Object,
                                      _bindingMock.Object,
                                      _cacheMock.Object,
                                      _plannerMock.Object,
                                      _pipelineMock.Object,
                                      _exceptionFormatterMock.Object);
            context.Plan = _planMock.Object;

            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _requestMock.InSequence(_mockSequence).Setup(p => p.GetScope()).Returns(bindingScope);
            _cacheMock.InSequence(_mockSequence).Setup(p => p.TryGet(context)).Returns(cachedInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _bindingMock.InSequence(_mockSequence).Setup(p => p.GetProvider(context)).Returns(_providerMock.Object);
            _providerMock.InSequence(_mockSequence).Setup(p => p.Create(context)).Returns(newInstance);
            _requestMock.InSequence(_mockSequence).Setup(p => p.ActiveBindings).Returns(activeBindings);
            _settingsMock.InSequence(_mockSequence).Setup(p => p.AllowNullInjection).Returns(true);

            var instance = context.Resolve();

            Assert.Null(instance);
            Assert.Empty(activeBindings);
            Assert.NotNull(context.Plan);
            Assert.Same(_planMock.Object, context.Plan);

            _requestMock.Verify(p => p.ActiveBindings, Times.Exactly(3));
            _requestMock.Verify(p => p.GetScope(), Times.Once());
            _cacheMock.Verify(p => p.TryGet(context), Times.Once());
            _providerMock.Verify(p => p.Create(context), Times.Once());
            _settingsMock.Verify(p => p.AllowNullInjection, Times.Once());
        }
    }
}
