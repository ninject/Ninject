using System.Reflection;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Strategies;
using Ninject.Injection;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;
using Xunit;

namespace Ninject.Tests.Unit.PropertyInjectionStrategyTests
{
    using Ninject.Components;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PropertyInjectionDirectiveContext
    {
        protected Mock<IInjectorFactory> injectorFactoryMock { get; private set; }
        protected Mock<IExceptionFormatter> exceptionFormatterMock { get; private set; }
        protected Mock<IContext> contextMock { get; private set; }
        protected Mock<IPlan> planMock { get; private set; }
        protected Mock<ITargetFactory> targetFactoryMock { get; private set; }
        protected Random random { get; private set; }
        protected readonly PropertyInjectionStrategy strategy;

        public PropertyInjectionDirectiveContext()
        {
            this.injectorFactoryMock = new Mock<IInjectorFactory>(MockBehavior.Strict);
            this.exceptionFormatterMock = new Mock<IExceptionFormatter>(MockBehavior.Strict);
            this.contextMock = new Mock<IContext>(MockBehavior.Strict);
            this.planMock = new Mock<IPlan>(MockBehavior.Strict);
            this.targetFactoryMock = new Mock<ITargetFactory>(MockBehavior.Strict);

            this.random = new Random();

            this.strategy = new PropertyInjectionStrategy(injectorFactoryMock.Object,
                                                          new NinjectSettings(),
                                                          this.exceptionFormatterMock.Object);
        }
    }

    public class WhenActivateIsCalled_WithProjectInjectionDirectivesAndWithoutPropertyValues : PropertyInjectionDirectiveContext
    {
        protected Mock<PropertyInjector> fooInjectorMock;
        protected Mock<PropertyInjector> barInjectorMock;
        protected Dummy instance = new Dummy();
        protected PropertyInfo fooProperty = typeof(Dummy).GetProperty("Foo");
        protected PropertyInfo barProperty = typeof(Dummy).GetProperty("Bar");
        protected int fooResolvedValue;
        protected string barResolvedValue;
        protected Mock<ITarget> fooTargetMock;
        protected Mock<ITarget> barTargetMock;
        protected InstanceReference reference;
        protected FakePropertyInjectionDirective[] directives;

        public WhenActivateIsCalled_WithProjectInjectionDirectivesAndWithoutPropertyValues()
        {
            this.fooInjectorMock = new Mock<PropertyInjector>(MockBehavior.Strict);
            this.barInjectorMock = new Mock<PropertyInjector>(MockBehavior.Strict);
            this.fooTargetMock = new Mock<ITarget>(MockBehavior.Strict);
            this.barTargetMock = new Mock<ITarget>(MockBehavior.Strict);

            FakePropertyInjectionDirective.TargetFactory = this.targetFactoryMock.Object;

            var mockSequence = new MockSequence();

            this.targetFactoryMock.InSequence(mockSequence)
                                  .Setup(p => p.Create(this.fooProperty))
                                  .Returns(fooTargetMock.Object);
            this.targetFactoryMock.InSequence(mockSequence)
                                  .Setup(p => p.Create(this.barProperty))
                                  .Returns(barTargetMock.Object);

            this.directives = new[]
                {
                    new FakePropertyInjectionDirective(this.fooProperty, this.fooInjectorMock.Object),
                    new FakePropertyInjectionDirective(this.barProperty, this.barInjectorMock.Object)
                };
            this.reference = new InstanceReference { Instance = this.instance };
            this.fooResolvedValue = this.random.Next();
            this.barResolvedValue = this.random.Next().ToString();

            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Parameters)
                            .Returns(Array.Empty<IParameter>());
            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.planMock.Object);
            this.planMock.InSequence(mockSequence)
                         .Setup(x => x.GetAll<PropertyInjectionDirective>())
                         .Returns(this.directives);
            this.fooTargetMock.InSequence(mockSequence)
                              .SetupGet(p => p.Name)
                              .Returns(fooProperty.Name);
            this.fooTargetMock.InSequence(mockSequence)
                              .Setup(p => p.ResolveWithin(this.contextMock.Object))
                              .Returns(fooResolvedValue);
            this.fooInjectorMock.InSequence(mockSequence)
                                .Setup(p => p(this.instance, this.fooResolvedValue));
            this.barTargetMock.InSequence(mockSequence)
                              .SetupGet(p => p.Name)
                              .Returns(barProperty.Name);
            this.barTargetMock.InSequence(mockSequence)
                              .Setup(p => p.ResolveWithin(this.contextMock.Object))
                              .Returns(barResolvedValue);
            this.barInjectorMock.InSequence(mockSequence)
                                .Setup(p => p(this.instance, this.barResolvedValue));
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            this.strategy.Activate(this.contextMock.Object, this.reference);

            this.planMock.Verify(x => x.GetAll<PropertyInjectionDirective>());
        }

        [Fact]
        public void ResolvesValuesForEachTargetOfEachDirective()
        {
            this.strategy.Activate(this.contextMock.Object, this.reference);

            this.fooTargetMock.Verify(p => p.ResolveWithin(this.contextMock.Object), Times.Once());
            this.barTargetMock.Verify(p => p.ResolveWithin(this.contextMock.Object), Times.Once());
        }

        [Fact]
        public void InvokesInjectorsForEachDirective()
        {
            this.strategy.Activate(this.contextMock.Object, this.reference);

            this.fooInjectorMock.Verify(x => x(this.instance, this.fooResolvedValue), Times.Once());
            this.barInjectorMock.Verify(x => x(this.instance, this.barResolvedValue), Times.Once());
        }
    }

    public class WhenActivateIsCalled_WithProjectInjectionDirectivesAndWithPropertyValuesFullMatch : PropertyInjectionDirectiveContext
    {
        protected Mock<PropertyInjector> fooInjectorMock;
        protected Mock<PropertyInjector> barInjectorMock;
        protected Dummy instance = new Dummy();
        protected PropertyInfo fooProperty = typeof(Dummy).GetProperty("Foo");
        protected PropertyInfo barProperty = typeof(Dummy).GetProperty("Bar");
        protected int fooPropertyValue;
        protected string barPropertyValue;
        protected Mock<ITarget> fooTargetMock;
        protected Mock<ITarget> barTargetMock;
        protected InstanceReference reference;
        protected FakePropertyInjectionDirective[] directives;
        protected IEnumerable<IParameter> parameters;

        public WhenActivateIsCalled_WithProjectInjectionDirectivesAndWithPropertyValuesFullMatch()
        {
            this.fooInjectorMock = new Mock<PropertyInjector>(MockBehavior.Strict);
            this.barInjectorMock = new Mock<PropertyInjector>(MockBehavior.Strict);
            this.fooTargetMock = new Mock<ITarget>(MockBehavior.Strict);
            this.barTargetMock = new Mock<ITarget>(MockBehavior.Strict);

            FakePropertyInjectionDirective.TargetFactory = this.targetFactoryMock.Object;

            var mockSequence = new MockSequence();

            this.targetFactoryMock.InSequence(mockSequence)
                                  .Setup(p => p.Create(this.fooProperty))
                                  .Returns(fooTargetMock.Object);
            this.targetFactoryMock.InSequence(mockSequence)
                                  .Setup(p => p.Create(this.barProperty))
                                  .Returns(barTargetMock.Object);

            this.fooPropertyValue = this.random.Next();
            this.barPropertyValue = this.random.Next().ToString();
            this.directives = new[]
                {
                    new FakePropertyInjectionDirective(this.fooProperty, this.fooInjectorMock.Object),
                    new FakePropertyInjectionDirective(this.barProperty, this.barInjectorMock.Object)
                };
            this.parameters = new List<IParameter>
                {
                    new ConstructorArgument("A", "B"),
                    new PropertyValue(this.fooProperty.Name, fooPropertyValue),
                    new PropertyValue(this.barProperty.Name, barPropertyValue)
                };
            this.reference = new InstanceReference { Instance = this.instance };

            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Parameters)
                            .Returns(this.parameters);
            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.planMock.Object);
            this.planMock.InSequence(mockSequence)
                         .Setup(x => x.GetAll<PropertyInjectionDirective>())
                         .Returns(this.directives);
            this.fooTargetMock.InSequence(mockSequence)
                              .SetupGet(p => p.Name)
                              .Returns(fooProperty.Name);
            this.fooInjectorMock.InSequence(mockSequence)
                                .Setup(p => p(this.instance, this.fooPropertyValue));
            this.barTargetMock.InSequence(mockSequence)
                              .SetupGet(p => p.Name)
                              .Returns(barProperty.Name);
            this.barInjectorMock.InSequence(mockSequence)
                                .Setup(p => p(this.instance, this.barPropertyValue));
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            this.strategy.Activate(this.contextMock.Object, this.reference);

            this.planMock.Verify(x => x.GetAll<PropertyInjectionDirective>());
        }

        [Fact]
        public void InvokesInjectorsForEachDirective()
        {
            this.strategy.Activate(this.contextMock.Object, this.reference);

            this.fooInjectorMock.Verify(x => x(this.instance, this.fooPropertyValue), Times.Once());
            this.barInjectorMock.Verify(x => x(this.instance, this.barPropertyValue), Times.Once());
        }
    }

    public class WhenActivateIsCalled_WithProjectInjectionDirectivesAndWithPropertyValuesPartialMatch : PropertyInjectionDirectiveContext
    {
        protected Mock<PropertyInjector> fooInjectorMock;
        protected Mock<PropertyInjector> barInjectorMock;
        protected Mock<PropertyInjector> gooInjectorMock;
        protected Dummy instance = new Dummy();
        protected PropertyInfo fooProperty = typeof(Dummy).GetProperty("Foo");
        protected PropertyInfo barProperty = typeof(Dummy).GetProperty("Bar");
        protected PropertyInfo gooProperty = typeof(Dummy).GetProperty("Goo");
        protected int fooPropertyValue;
        protected string gooPropertyValue;
        protected string barResolvedValue;
        protected Mock<ITarget> fooTargetMock;
        protected Mock<ITarget> barTargetMock;
        protected Mock<ITarget> gooTargetMock;
        protected InstanceReference reference;
        protected FakePropertyInjectionDirective[] directives;
        protected IEnumerable<IParameter> parameters;

        public WhenActivateIsCalled_WithProjectInjectionDirectivesAndWithPropertyValuesPartialMatch()
        {
            this.fooInjectorMock = new Mock<PropertyInjector>(MockBehavior.Strict);
            this.barInjectorMock = new Mock<PropertyInjector>(MockBehavior.Strict);
            this.gooInjectorMock = new Mock<PropertyInjector>(MockBehavior.Strict);
            this.fooTargetMock = new Mock<ITarget>(MockBehavior.Strict);
            this.barTargetMock = new Mock<ITarget>(MockBehavior.Strict);

            FakePropertyInjectionDirective.TargetFactory = this.targetFactoryMock.Object;

            var mockSequence = new MockSequence();

            this.targetFactoryMock.InSequence(mockSequence)
                                  .Setup(p => p.Create(this.fooProperty))
                                  .Returns(fooTargetMock.Object);
            this.targetFactoryMock.InSequence(mockSequence)
                                  .Setup(p => p.Create(this.barProperty))
                                  .Returns(barTargetMock.Object);

            this.fooPropertyValue = this.random.Next();
            this.gooPropertyValue = this.random.Next().ToString();
            this.barResolvedValue = this.random.Next().ToString();
            this.directives = new[]
                {
                    new FakePropertyInjectionDirective(this.fooProperty, this.fooInjectorMock.Object),
                    new FakePropertyInjectionDirective(this.barProperty, this.barInjectorMock.Object)
                };
            this.parameters = new List<IParameter>
                {
                    new ConstructorArgument("A", "B"),
                    new PropertyValue(this.fooProperty.Name, fooPropertyValue),
                    new PropertyValue(this.gooProperty.Name, gooPropertyValue)
                };
            this.reference = new InstanceReference { Instance = this.instance };

            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Parameters)
                            .Returns(this.parameters);
            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.planMock.Object);
            this.planMock.InSequence(mockSequence)
                         .Setup(x => x.GetAll<PropertyInjectionDirective>())
                         .Returns(this.directives);
            this.fooTargetMock.InSequence(mockSequence)
                              .SetupGet(p => p.Name)
                              .Returns(fooProperty.Name);
            this.fooInjectorMock.InSequence(mockSequence)
                                .Setup(p => p(this.instance, this.fooPropertyValue));
            this.barTargetMock.InSequence(mockSequence)
                              .SetupGet(p => p.Name)
                              .Returns(barProperty.Name);
            this.barTargetMock.InSequence(mockSequence)
                              .Setup(p => p.ResolveWithin(this.contextMock.Object))
                              .Returns(barResolvedValue);
            this.barInjectorMock.InSequence(mockSequence)
                                .Setup(p => p(this.instance, this.barResolvedValue));
            this.injectorFactoryMock.InSequence(mockSequence)
                                    .Setup(p => p.Create(gooProperty))
                                    .Returns(gooInjectorMock.Object);
            this.gooInjectorMock.InSequence(mockSequence)
                                .Setup(p => p(this.instance, this.gooPropertyValue));
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            this.strategy.Activate(this.contextMock.Object, this.reference);

            this.planMock.Verify(x => x.GetAll<PropertyInjectionDirective>());
        }

        [Fact]
        public void ResolvesValuesForDirectivesWithoutPropertyValueParameter()
        {
            this.strategy.Activate(this.contextMock.Object, this.reference);

            this.barTargetMock.Verify(p => p.ResolveWithin(this.contextMock.Object), Times.Once());
        }

        [Fact]
        public void InvokesInjectorsForEachDirectiveWithoutPropertyValue()
        {
            this.strategy.Activate(this.contextMock.Object, this.reference);

            this.barInjectorMock.Verify(x => x(this.instance, this.barResolvedValue), Times.Once());
        }

        [Fact]
        public void InvokesInjectorsForEachPropertyValue()
        {
            this.strategy.Activate(this.contextMock.Object, this.reference);

            this.fooInjectorMock.Verify(x => x(this.instance, this.fooPropertyValue), Times.Once());
            this.gooInjectorMock.Verify(x => x(this.instance, this.gooPropertyValue), Times.Once());
        }
    }

    public class WhenActivateIsCalled_WithoutProjectInjectionDirectivesAndWithoutPropertyValues : PropertyInjectionDirectiveContext
    {
        private Dummy instance;
        private InstanceReference reference;

        public WhenActivateIsCalled_WithoutProjectInjectionDirectivesAndWithoutPropertyValues()
        {
            IEnumerable<IParameter> parameters = new List<IParameter>
                {
                    new ConstructorArgument("A", "B")
                };
            this.instance = new Dummy();
            this.reference = new InstanceReference { Instance = this.instance };

            var mockSequence = new MockSequence();

            this.contextMock.InSequence(mockSequence)
                            .SetupGet(p => p.Parameters)
                            .Returns(parameters);
            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.planMock.Object);
            this.planMock.InSequence(mockSequence)
                         .Setup(x => x.GetAll<PropertyInjectionDirective>())
                         .Returns(Enumerable.Empty<PropertyInjectionDirective>());
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            this.strategy.Activate(this.contextMock.Object, this.reference);

            this.planMock.Verify(x => x.GetAll<PropertyInjectionDirective>(), Times.Once());
        }
    }

    public class WhenActivateIsCalled_WithoutProjectInjectionDirectivesAndWithPropertyValues : PropertyInjectionDirectiveContext
    {
        protected Dummy instance;
        protected PropertyInfo fooProperty = typeof(Dummy).GetProperty("Foo");
        protected PropertyInfo barProperty = typeof(Dummy).GetProperty("Bar");
        protected int fooPropertyValue;
        protected string barPropertyValue;
        protected Mock<PropertyInjector> fooInjectorMock;
        protected Mock<PropertyInjector> barInjectorMock;
        protected InstanceReference reference;
        protected PropertyInjectionDirective[] directives;
        protected IEnumerable<IParameter> parameters;

        public WhenActivateIsCalled_WithoutProjectInjectionDirectivesAndWithPropertyValues()
        {
            this.fooInjectorMock = new Mock<PropertyInjector>(MockBehavior.Strict);
            this.barInjectorMock = new Mock<PropertyInjector>(MockBehavior.Strict);

            this.fooPropertyValue = new Random().Next();
            this.barPropertyValue = new Random().Next().ToString();
            this.parameters = new List<IParameter>
                {
                    new ConstructorArgument("A", "B"),
                    new PropertyValue(this.fooProperty.Name, fooPropertyValue),
                    new PropertyValue(this.barProperty.Name, barPropertyValue)
                };
            this.instance = new Dummy();
            this.reference = new InstanceReference { Instance = this.instance };

            var mockSequence = new MockSequence();

            this.contextMock.InSequence(mockSequence)
                            .SetupGet(p => p.Parameters)
                            .Returns(parameters);
            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.planMock.Object);
            this.planMock.InSequence(mockSequence)
                         .Setup(x => x.GetAll<PropertyInjectionDirective>())
                         .Returns(Enumerable.Empty<PropertyInjectionDirective>());
            this.injectorFactoryMock.InSequence(mockSequence)
                                    .Setup(p => p.Create(fooProperty))
                                    .Returns(fooInjectorMock.Object);
            this.fooInjectorMock.InSequence(mockSequence)
                                .Setup(p => p(this.instance, fooPropertyValue));
            this.injectorFactoryMock.InSequence(mockSequence)
                                    .Setup(p => p.Create(barProperty))
                                    .Returns(barInjectorMock.Object);
            this.barInjectorMock.InSequence(mockSequence)
                                .Setup(p => p(this.instance, barPropertyValue));
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            this.strategy.Activate(this.contextMock.Object, this.reference);

            this.planMock.Verify(x => x.GetAll<PropertyInjectionDirective>(), Times.Once());
        }

        [Fact]
        public void InjectorIsInvokedForEachPropertyForWhilePropertyValueIsSupplied()
        {
            this.strategy.Activate(this.contextMock.Object, this.reference);

            this.fooInjectorMock.Verify(x => x(this.instance, fooPropertyValue), Times.Once());
            this.barInjectorMock.Verify(x => x(this.instance, barPropertyValue), Times.Once());
        }
    }

    public class WhenActivateIsCalled_PropertyValueCannotBeResolvedToProperty : PropertyInjectionDirectiveContext
    {
        protected Mock<IRequest> requestMock;
        protected Dummy instance;
        protected InstanceReference reference;
        private string exceptionMessage;
        protected IEnumerable<IParameter> parameters;

        public WhenActivateIsCalled_PropertyValueCannotBeResolvedToProperty()
        {
            this.requestMock = new Mock<IRequest>(MockBehavior.Strict);

            this.parameters = new List<IParameter>
                {
                    new ConstructorArgument("A", "B"),
                    new PropertyValue("DoesNotExist", "Boom!"),
                };
            this.instance = new Dummy();
            this.reference = new InstanceReference { Instance = this.instance };
            this.exceptionMessage = this.random.Next().ToString();

            var mockSequence = new MockSequence();

            this.contextMock.InSequence(mockSequence)
                            .SetupGet(p => p.Parameters)
                            .Returns(parameters);
            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.planMock.Object);
            this.planMock.InSequence(mockSequence)
                         .Setup(x => x.GetAll<PropertyInjectionDirective>())
                         .Returns(Enumerable.Empty<PropertyInjectionDirective>());
            this.contextMock.InSequence(mockSequence)
                            .Setup(p => p.Request)
                            .Returns(this.requestMock.Object);
            this.exceptionFormatterMock.InSequence(mockSequence)
                                       .Setup(p => p.CouldNotResolvePropertyForValueInjection(this.requestMock.Object, "DoesNotExist"))
                                       .Returns(this.exceptionMessage);
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            Assert.Throws<ActivationException>(() => this.strategy.Activate(this.contextMock.Object, this.reference));

            this.planMock.Verify(x => x.GetAll<PropertyInjectionDirective>(), Times.Once());
        }

        [Fact]
        public void ThrowsActivationException()
        {
            var actualException = Assert.Throws<ActivationException>(() => this.strategy.Activate(this.contextMock.Object, this.reference));

            Assert.Null(actualException.InnerException);
            Assert.Same(this.exceptionMessage, actualException.Message);
        }
    }


    public interface ITargetFactory
    {
        ITarget Create(PropertyInfo property);
    }

    public class FakePropertyInjectionDirective : PropertyInjectionDirective
    {
        public static ITargetFactory TargetFactory { get; set; }

        public FakePropertyInjectionDirective(PropertyInfo property, PropertyInjector injector)
            : base(property, injector) {
        }

        protected override ITarget CreateTarget(PropertyInfo property)
        {
            return TargetFactory.Create(property);
        }
    }

    public class Dummy
    {
        public int Foo { get; set; }
        public string Bar { get; set; }
        public string Goo { get; set; }
    }
}
