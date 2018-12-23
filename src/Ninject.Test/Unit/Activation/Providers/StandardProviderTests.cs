using Moq;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Activation.Providers;
using Ninject.Infrastructure.Introspection;
using Ninject.Injection;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;
using Ninject.Selection.Heuristics;
using Ninject.Tests.Fakes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Ninject.Test.Unit.Activation.Providers
{
    public class StandardProviderTests
    {
        private Mock<IContext> _contextMock;
        private NinjectSettings _ninjectSettings;
        private Mock<IRequest> _requestMock;
        private Mock<IPlanner> _plannerMock;
        private Mock<IPlan> _planMock;
        private Mock<Func<IContext, IProvider>> _providerCallbackMock;
        private Mock<IProvider> _providerMock;
        private Mock<IConstructorScorer> _constructorScorerMock;
        private Mock<ITarget> _targetMock;
        private StandardProvider _standardProvider;

        public StandardProviderTests()
        {
            _ninjectSettings = new NinjectSettings
                {
                    // Disable to reduce memory pressure
                    ActivationCacheDisabled = true,
                    LoadExtensions = false
                };

            _requestMock = new Mock<IRequest>(MockBehavior.Strict);
            _contextMock = new Mock<IContext>(MockBehavior.Strict);
            _plannerMock = new Mock<IPlanner>(MockBehavior.Strict);
            _planMock = new Mock<IPlan>(MockBehavior.Strict);
            _providerCallbackMock = new Mock<Func<IContext, IProvider>>(MockBehavior.Strict);
            _providerMock = new Mock<IProvider>(MockBehavior.Strict);
            _constructorScorerMock = new Mock<IConstructorScorer>(MockBehavior.Strict);
            _targetMock = new Mock<ITarget>(MockBehavior.Strict);

            _standardProvider = new StandardProvider(typeof(Monk), _plannerMock.Object, _constructorScorerMock.Object);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullExceptionWhenTypeIsNull()
        {
            const Type type = null;
            var planner = _plannerMock.Object;
            var constructorScorer = _constructorScorerMock.Object;

            var actual = Assert.Throws<ArgumentNullException>(() => new StandardProvider(type, planner, constructorScorer));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(type), actual.ParamName);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullExceptionWhenPlannerIsNull()
        {
            var type = typeof(Monk);
            const IPlanner planner = null;
            var constructorScorer = _constructorScorerMock.Object;

            var actual = Assert.Throws<ArgumentNullException>(() => new StandardProvider(type, planner, constructorScorer));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(planner), actual.ParamName);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullExceptionWhenConstructorScorerIsNull()
        {
            var type = typeof(Monk);
            var planner = _plannerMock.Object;
            IConstructorScorer constructorScorer = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new StandardProvider(type, planner, constructorScorer));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(constructorScorer), actual.ParamName);
        }

        [Fact]
        public void Constructor()
        {
            var type = typeof(Ninja);

            var actual = new StandardProvider(type, _plannerMock.Object, _constructorScorerMock.Object);

            Assert.Same(type, actual.Type);
            Assert.Same(_plannerMock.Object, actual.Planner);
            Assert.Same(_constructorScorerMock.Object, actual.ConstructorScorer);
        }

        [Fact]
        public void Create_ShouldUseConstructorArgumentsForBestConstructor()
        {
            var seq = new MockSequence();

            var injectorOneMock = new Mock<ConstructorInjector>(MockBehavior.Strict);
            var injectorTwoMock = new Mock<ConstructorInjector>(MockBehavior.Strict);
            var directiveOne = new ConstructorInjectionDirective(GetMyServiceWeaponAndWarriorConstructor(), injectorOneMock.Object);
            var directiveTwo = new ConstructorInjectionDirective(GetMyServiceClericAndName(), injectorTwoMock.Object);
            var name = new Random().Next().ToString();
            var cleric = new Monk();
            var instance = new object();
            var parameters = new List<IParameter>
                {
                    new ConstructorArgument("name", name),
                    new ConstructorArgument("weapon", new Dagger()),
                    new ConstructorArgument("cleric", cleric),
                };

            var kernelConfiguration = new KernelConfiguration(_ninjectSettings);
            var context = CreateContext(kernelConfiguration, kernelConfiguration.BuildReadOnlyKernel(), parameters, typeof(NinjaBarracks), _providerCallbackMock.Object, _ninjectSettings);
            context.Plan = new Plan(typeof(MyService));
            context.Plan.Add(directiveOne);
            context.Plan.Add(directiveTwo);

            _constructorScorerMock.InSequence(seq).Setup(p => p.Score(context, directiveOne)).Returns(2);
            _constructorScorerMock.InSequence(seq).Setup(p => p.Score(context, directiveTwo)).Returns(3);
            injectorTwoMock.InSequence(seq).Setup(p => p(cleric, name)).Returns(instance);

            var actual = _standardProvider.Create(context);

            Assert.Same(instance, actual);
        }

        [Fact]
        public void Create_ShouldThrowInvalidOperationExceptionWhenNoConstructorArgumentIsAvailableForGivenParameterAndResolveReturnsMultipleResults()
        {
            var seq = new MockSequence();

            var injectorOneMock = new Mock<ConstructorInjector>(MockBehavior.Strict);
            var injectorTwoMock = new Mock<ConstructorInjector>(MockBehavior.Strict);
            var directiveOne = new ConstructorInjectionDirective(GetMyServiceWeaponAndWarriorConstructor(), injectorOneMock.Object);
            var directiveTwo = new ConstructorInjectionDirective(GetMyServiceClericAndName(), injectorTwoMock.Object);
            var readOnlyKernelMock = new Mock<IReadOnlyKernel>(MockBehavior.Strict);
            var childRequestMock = new Mock<IRequest>(MockBehavior.Strict);
            var planMock = new Mock<IPlan>(MockBehavior.Strict);
            var instance = new object();
            var weaponMock = new Mock<IWeapon>(MockBehavior.Strict);
            var warriorMock = new Mock<IWarrior>(MockBehavior.Strict);
            var directives = new[]

                {
                    directiveOne,
                    directiveTwo
                };

            var parameters = new List<IParameter>
                {
                    new ConstructorArgument("name", "Foo"),
                    new ConstructorArgument("weapon", weaponMock.Object),
                    new ConstructorArgument("cleric", new Monk()),
                };

            _contextMock.InSequence(seq).Setup(p => p.Plan).Returns(_planMock.Object);
            _contextMock.InSequence(seq).Setup(p => p.Plan).Returns(_planMock.Object);
            _planMock.InSequence(seq).Setup(p => p.GetAll< ConstructorInjectionDirective>()).Returns(directives);
            _constructorScorerMock.InSequence(seq).Setup(p => p.Score(_contextMock.Object, directiveOne)).Returns(3);
            _constructorScorerMock.InSequence(seq).Setup(p => p.Score(_contextMock.Object, directiveTwo)).Returns(2);
            _contextMock.InSequence(seq).Setup(p => p.Parameters).Returns(parameters);
            _contextMock.InSequence(seq).Setup(p => p.Parameters).Returns(parameters);
            _contextMock.InSequence(seq).Setup(p => p.Request).Returns(_requestMock.Object);
            _requestMock.InSequence(seq).Setup(p => p.CreateChild(typeof(IWarrior), _contextMock.Object, It.IsAny<ParameterTarget>())).Returns(childRequestMock.Object);
            childRequestMock.InSequence(seq).SetupSet(p => p.IsUnique = true);
            _contextMock.InSequence(seq).Setup(p => p.Kernel).Returns(readOnlyKernelMock.Object);
            readOnlyKernelMock.InSequence(seq).Setup(p => p.Resolve(childRequestMock.Object)).Returns(new[] { warriorMock.Object, new FootSoldier() });

            var actualException = Assert.Throws<InvalidOperationException>(() => _standardProvider.Create(_contextMock.Object));

            Assert.Null(actualException.InnerException);
            Assert.Equal("Sequence contains more than one element", actualException.Message);
        }

        [Fact]
        public void Create_ShouldPassResolvedObjectForGivenParameterWhenNoConstructorArgumentIsAvailableForParameterAndResolveReturnsSingleResult()
        {
            var seq = new MockSequence();

            var injectorOneMock = new Mock<ConstructorInjector>(MockBehavior.Strict);
            var injectorTwoMock = new Mock<ConstructorInjector>(MockBehavior.Strict);
            var directiveOne = new ConstructorInjectionDirective(GetMyServiceWeaponAndWarriorConstructor(), injectorOneMock.Object);
            var directiveTwo = new ConstructorInjectionDirective(GetMyServiceClericAndName(), injectorTwoMock.Object);
            var weaponMock = new Mock<IWeapon>(MockBehavior.Strict);
            var warriorMock = new Mock<IWarrior>(MockBehavior.Strict);
            var readOnlyKernelMock = new Mock<IReadOnlyKernel>(MockBehavior.Strict);
            var childRequestMock = new Mock<IRequest>(MockBehavior.Strict);
            var planMock = new Mock<IPlan>(MockBehavior.Strict);
            var expected = new object();
            var directives = new[]
                {
                    directiveOne,
                    directiveTwo
                };

            var parameters = new List<IParameter>
                {
                    new ConstructorArgument("name", "Foo"),
                    new ConstructorArgument("weapon", weaponMock.Object),
                    new ConstructorArgument("cleric", new Monk()),
                };

            _contextMock.InSequence(seq).Setup(p => p.Plan).Returns(_planMock.Object);
            _contextMock.InSequence(seq).Setup(p => p.Plan).Returns(_planMock.Object);
            _planMock.InSequence(seq).Setup(p => p.GetAll<ConstructorInjectionDirective>()).Returns(directives);
            _constructorScorerMock.InSequence(seq).Setup(p => p.Score(_contextMock.Object, directiveOne)).Returns(3);
            _constructorScorerMock.InSequence(seq).Setup(p => p.Score(_contextMock.Object, directiveTwo)).Returns(2);
            _contextMock.InSequence(seq).Setup(p => p.Parameters).Returns(parameters);
            _contextMock.InSequence(seq).Setup(p => p.Parameters).Returns(parameters);
            _contextMock.InSequence(seq).Setup(p => p.Request).Returns(_requestMock.Object);
            _requestMock.InSequence(seq).Setup(p => p.CreateChild(typeof(IWarrior), _contextMock.Object, It.IsAny<ParameterTarget>())).Returns(childRequestMock.Object);
            childRequestMock.InSequence(seq).SetupSet(p => p.IsUnique = true);
            _contextMock.InSequence(seq).Setup(p => p.Kernel).Returns(readOnlyKernelMock.Object);
            readOnlyKernelMock.InSequence(seq).Setup(p => p.Resolve(childRequestMock.Object)).Returns(new[] { warriorMock.Object });
            injectorOneMock.InSequence(seq).Setup(p => p(weaponMock.Object, warriorMock.Object)).Returns(expected);

            var actual = _standardProvider.Create(_contextMock.Object);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Create_ShouldPassNullAsValueForGivenParameterWhenNoConstructorArgumentIsAvailableForGivenParameterAndResolveReturnsEmptyResult()
        {
            var seq = new MockSequence();

            var injectorOneMock = new Mock<ConstructorInjector>(MockBehavior.Strict);
            var injectorTwoMock = new Mock<ConstructorInjector>(MockBehavior.Strict);
            var directiveOne = new ConstructorInjectionDirective(GetMyServiceWeaponAndWarriorConstructor(), injectorOneMock.Object);
            var directiveTwo = new ConstructorInjectionDirective(GetMyServiceClericAndName(), injectorTwoMock.Object);
            var weaponMock = new Mock<IWeapon>(MockBehavior.Strict);
            var warriorMock = new Mock<IWarrior>(MockBehavior.Strict);
            var readOnlyKernelMock = new Mock<IReadOnlyKernel>(MockBehavior.Strict);
            var childRequestMock = new Mock<IRequest>(MockBehavior.Strict);
            var planMock = new Mock<IPlan>(MockBehavior.Strict);
            var directives = new[]
                {
                    directiveOne,
                    directiveTwo
                };
            var expected = new object();
            var parameters = new List<IParameter>
                {
                    new ConstructorArgument("name", "Foo"),
                    new ConstructorArgument("weapon", weaponMock.Object),
                    new ConstructorArgument("cleric", new Monk()),
                };

            _contextMock.InSequence(seq).Setup(p => p.Plan).Returns(_planMock.Object);
            _contextMock.InSequence(seq).Setup(p => p.Plan).Returns(_planMock.Object);
            _planMock.InSequence(seq).Setup(p => p.GetAll<ConstructorInjectionDirective>()).Returns(directives);
            _constructorScorerMock.InSequence(seq).Setup(p => p.Score(_contextMock.Object, directiveOne)).Returns(3);
            _constructorScorerMock.InSequence(seq).Setup(p => p.Score(_contextMock.Object, directiveTwo)).Returns(2);
            _contextMock.InSequence(seq).Setup(p => p.Parameters).Returns(parameters);
            _contextMock.InSequence(seq).Setup(p => p.Parameters).Returns(parameters);
            _contextMock.InSequence(seq).Setup(p => p.Request).Returns(_requestMock.Object);
            _requestMock.InSequence(seq).Setup(p => p.CreateChild(typeof(IWarrior), _contextMock.Object, It.IsAny<ParameterTarget>())).Returns(childRequestMock.Object);
            childRequestMock.InSequence(seq).SetupSet(p => p.IsUnique = true);
            _contextMock.InSequence(seq).Setup(p => p.Kernel).Returns(readOnlyKernelMock.Object);
            readOnlyKernelMock.InSequence(seq).Setup(p => p.Resolve(childRequestMock.Object)).Returns(Enumerable.Empty<object>());
            injectorOneMock.InSequence(seq).Setup(p => p(weaponMock.Object, null)).Returns(expected);

            var actual = _standardProvider.Create(_contextMock.Object);

            Assert.Same(expected, actual);
        }


        [Fact]
        public void Create_ShouldThrowArgumentNullExceptionWhenContextIsNull()
        {
            const IContext context = null;

            var actualException = Assert.Throws<ArgumentNullException>(() => _standardProvider.Create(context));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(context), actualException.ParamName);
        }

        [Fact]
        public void Create_ShouldCreatePlanWhenNull()
        {
            var seq = new MockSequence();
            var constructorInjectorMock = new Mock<ConstructorInjector>(MockBehavior.Strict);
            var createdObject = new object();
            var directives = new[]
                {
                    new ConstructorInjectionDirective(GetMyServiceDefaultConstructor(), constructorInjectorMock.Object)
                };

            _contextMock.InSequence(seq).Setup(p => p.Plan).Returns((IPlan) null);
            _contextMock.InSequence(seq).Setup(p => p.Request).Returns(_requestMock.Object);
            _requestMock.InSequence(seq).Setup(p => p.Service).Returns(typeof(Dagger));
            _plannerMock.InSequence(seq).Setup(p => p.GetPlan(typeof(Monk))).Returns(_planMock.Object);
            _contextMock.InSequence(seq).SetupSet(p => p.Plan = _planMock.Object);
            _contextMock.InSequence(seq).Setup(p => p.Plan).Returns(_planMock.Object);
            _planMock.InSequence(seq).Setup(p => p.GetAll<ConstructorInjectionDirective>()).Returns(directives);
            constructorInjectorMock.InSequence(seq).Setup(p => p(new object[0])).Returns(createdObject);

            var actual = _standardProvider.Create(_contextMock.Object);

            Assert.Same(createdObject, actual);
        }

        [Fact]
        public void Create_ShouldThrowActivationExceptionWhenThereAreZeroConstructorInjectionDirectives()
        {
            var directives = new ConstructorInjectionDirective[0];
            var parameters = new List<IParameter>
                {
                    new ConstructorArgument("location", "Biutiful"),
                    new ConstructorArgument("warrior", new Monk()),
                    new ConstructorArgument("weapon", new Dagger())
                };

            var kernelConfiguration = new KernelConfiguration(_ninjectSettings);
            var context = CreateContext(kernelConfiguration, kernelConfiguration.BuildReadOnlyKernel(), parameters, typeof(NinjaBarracks), _providerCallbackMock.Object, _ninjectSettings);
            context.Plan = new Plan(typeof(NinjaBarracks));

            _providerCallbackMock.Setup(p => p(context)).Returns(_providerMock.Object);

            var actualException = Assert.Throws<ActivationException>(() => _standardProvider.Create(context));

            Assert.Null(actualException.InnerException);
            Assert.Equal(CreateNoConstructorAvailableExceptionMessage(), actualException.Message);

            _providerCallbackMock.Verify(p => p(context), Times.Once());
        }

        [Fact]
        public void Create_ShouldThrowActivationExceptionWhenThereIsMoreThanOneConstructorInjectionDirectiveWithTheBestScore()
        {
            var seq = new MockSequence();
            var constructorInjectorMock = new Mock<ConstructorInjector>(MockBehavior.Strict);

            var directiveOne = new ConstructorInjectionDirective(GetMyServiceWeaponAndWarriorConstructor(), constructorInjectorMock.Object);
            var directiveTwo = new ConstructorInjectionDirective(GetMyServiceDefaultConstructor(), constructorInjectorMock.Object);
            var directiveThree = new ConstructorInjectionDirective(GetMyServiceClericAndName(), constructorInjectorMock.Object);
            var directives = new ConstructorInjectionDirective[]
                {
                    directiveOne,
                    directiveTwo,
                    directiveThree
                };
            var parameters = new List<IParameter>
                {
                    new ConstructorArgument("location", "Biutiful"),
                    new ConstructorArgument("warrior", new Monk()),
                    new ConstructorArgument("weapon", new Dagger())
                };

            var kernelConfiguration = new KernelConfiguration(_ninjectSettings);
            var context = CreateContext(kernelConfiguration, kernelConfiguration.BuildReadOnlyKernel(), parameters, typeof(NinjaBarracks), _providerCallbackMock.Object, _ninjectSettings);
            context.Plan = new Plan(typeof(MyService));
            context.Plan.Add(directiveOne);
            context.Plan.Add(directiveTwo);
            context.Plan.Add(directiveThree);

            _constructorScorerMock.InSequence(seq).Setup(p => p.Score(context, directiveOne)).Returns(3);
            _constructorScorerMock.InSequence(seq).Setup(p => p.Score(context, directiveTwo)).Returns(2);
            _constructorScorerMock.InSequence(seq).Setup(p => p.Score(context, directiveThree)).Returns(3);
            _providerCallbackMock.InSequence(seq).Setup(p => p(context)).Returns(_providerMock.Object);

            var actualException = Assert.Throws<ActivationException>(() => _standardProvider.Create(context));

            Assert.Null(actualException.InnerException);
            Assert.Equal(CreateAmbiguousConstructorExceptionMessage(actualException.GetType()), actualException.Message);
        }

        [Fact]
        public void Create_ShouldThrowInvalidOperationExceptionWhenMoreThanOneConstructorArgumentIsAvailableForParameter()
        {
            var parameters = new List<IParameter>
                {
                    new ConstructorArgument("location", "Biutiful"),
                    new ConstructorArgument("warrior", new Monk()),
                    new ConstructorArgument("warrior", new Monk()),
                    new ConstructorArgument("weapon", new Dagger())
                };
            var constructorInjectorMock = new Mock<ConstructorInjector>(MockBehavior.Strict);

            var kernelConfiguration = new KernelConfiguration(_ninjectSettings);
            var context = CreateContext(kernelConfiguration, kernelConfiguration.BuildReadOnlyKernel(), parameters, typeof(NinjaBarracks), _providerCallbackMock.Object, _ninjectSettings);
            context.Plan = new Plan(typeof(NinjaBarracks));
            context.Plan.Add(new ConstructorInjectionDirective(GetWeaponAndWarriorConstructor(), constructorInjectorMock.Object));

            var actualException = Assert.Throws<InvalidOperationException>(() => _standardProvider.Create(context));

            Assert.Null(actualException.InnerException);
            Assert.Equal("Sequence contains more than one matching element", actualException.Message);
        }

        [Fact]
        public void GetImplementationType_ShouldThrowArgumentNullExceptionWhenServiceIsNull()
        {
            const Type service = null;

            var actualException = Assert.Throws<ArgumentNullException>(() => _standardProvider.GetImplementationType(service));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(service), actualException.ParamName);
        }

        [Fact]
        public void GetValue_ShouldThrowArgumentNullExceptionWhenContextIsNull()
        {
            const IContext context = null;
            var target = _targetMock.Object;

            var actualException = Assert.Throws<ArgumentNullException>(() => _standardProvider.GetValue(context, target));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(context), actualException.ParamName);
        }

        [Fact]
        public void GetValue_ShouldThrowArgumentNullExceptionWhenTargetIsNull()
        {
            var context = _contextMock.Object;
            const ITarget target = null;

            var actualException = Assert.Throws<ArgumentNullException>(() => _standardProvider.GetValue(context, target));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(target), actualException.ParamName);
        }

        private static ConstructorInfo GetWeaponAndWarriorConstructor()
        {
            return typeof(NinjaBarracks).GetConstructor(new[] { typeof(IWarrior), typeof(IWeapon) });
        }

        private static ConstructorInfo GetMyServiceDefaultConstructor()
        {
            return typeof(MyService).GetConstructor(new Type[0]);
        }

        private static ConstructorInfo GetMyServiceWeaponAndWarriorConstructor()
        {
            return typeof(MyService).GetConstructor(new[] { typeof(IWeapon), typeof(IWarrior) });
        }

        private static ConstructorInfo GetMyServiceClericAndName()
        {
            return typeof(MyService).GetConstructor(new[] { typeof(ICleric), typeof(string) });
        }

        private static Context CreateContext(IKernelConfiguration kernelConfiguration,
                                             IReadOnlyKernel readonlyKernel,
                                             IEnumerable<IParameter> parameters,
                                             Type serviceType,
                                             Func<IContext, IProvider> providerCallback,
                                             INinjectSettings ninjectSettings)
        {
            var request = new Request(serviceType,
                                      null,
                                      Enumerable.Empty<IParameter>(),
                                      null,
                                      false,
                                      true);

            var binding = new Binding(serviceType);
            binding.BindingConfiguration.ProviderCallback = providerCallback;

            var context = new Context(readonlyKernel,
                                      ninjectSettings,
                                      request,
                                      binding,
                                      kernelConfiguration.Components.Get<ICache>(),
                                      kernelConfiguration.Components.Get<IPlanner>(),
                                      kernelConfiguration.Components.Get<IPipeline>(),
                                      kernelConfiguration.Components.Get<IExceptionFormatter>());
            context.Parameters = parameters.ToArray();
            return context;
        }

        private static string CreateAmbiguousConstructorExceptionMessage(Type exceptionType)
        {
            var newLine = Environment.NewLine;

            return "Error activating NinjaBarracks using self-binding of NinjaBarracks" + newLine +
                   "Several constructors have the same priority." + newLine +
                   newLine +
                   "Constructors:" + newLine +
                   "MyService(IWeapon weaponIWarrior warrior)" + newLine +
                   "MyService(ICleric clericstring name)" + newLine +
                   newLine +
                   "Activation path:" + newLine +
                   "  1) Request for NinjaBarracks" + newLine +
                   newLine +
                   "Suggestions:" + newLine +
                   "  1) Specify the constructor using ToConstructor syntax." + newLine +
                   "  2) Add an Inject attribute to the constructor." + newLine;
        }

        private static string CreateNoConstructorAvailableExceptionMessage()
        {
            var newLine = Environment.NewLine;

            return "Error activating NinjaBarracks using self-binding of NinjaBarracks" + newLine +
                   "No constructor was available to create an instance of the implementation type." + newLine +
                   newLine +
                   "Activation path:" + newLine +
                   "  1) Request for NinjaBarracks" + newLine +
                   newLine +
                   "Suggestions:" + newLine +
                   "  1) Ensure that the implementation type has a public constructor." + newLine +
                   "  2) If you have implemented the Singleton pattern, use a binding with InSingletonScope() instead." + newLine;
        }

        public class MyService
        {
            public MyService()
            {
            }

            public MyService(IWeapon weapon, IWarrior warrior)
            {
            }

            public MyService(ICleric cleric, string name)
            {
            }
        }
    }
}
