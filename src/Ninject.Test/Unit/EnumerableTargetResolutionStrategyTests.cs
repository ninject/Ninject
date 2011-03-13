namespace Ninject.Tests.Unit.EnumerableTargetResolutionStrategyTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Moq;
    using Ninject.Activation;
    using Ninject.Infrastructure;
    using Ninject.Parameters;
    using Ninject.Planning.Targets;
    using Ninject.Planning.Targets.Strategies;
    using Ninject.Tests.Fakes;
    using Xunit;
    using Xunit.Should;

    public class EnumerableTargetResolutionStrategyContext
    {
        protected EnumerableTargetResolutionStrategy Strategy { get; private set; }
        protected Mock<IContext> ContextMock { get; private set; }
        protected Mock<ITarget> TargetMock { get; private set; }
        protected Mock<IKernel> KernelMock { get; private set; }
        protected IList<IWeapon> RegisteredTypes { get; private set; }
        protected Type RequestedType { get; set; }
        protected Maybe<object> Result { get; private set; }

        public EnumerableTargetResolutionStrategyContext()
        {
            Strategy = new EnumerableTargetResolutionStrategy();
            this.ContextMock = new Mock<IContext>();
            this.KernelMock = new Mock<IKernel>();
            this.TargetMock = new Mock<ITarget>();
            this.TargetMock.SetupGet(t => t.Type).Returns(() => this.RequestedType);
            this.RegisteredTypes = new List<IWeapon>();

            this.ContextMock.SetupGet(c => c.Request).Returns(new Request(typeof(Ninja), null, new IParameter[0], null, false, false));
            this.ContextMock.SetupGet(c => c.Kernel).Returns(KernelMock.Object);
            this.ContextMock.SetupGet(c => c.Parameters).Returns(new List<IParameter>());
            this.KernelMock.Setup(k => k.Resolve(It.IsAny<IRequest>())).Returns(this.RegisteredTypes.Cast<object>());
        }

        protected void Resolve()
        {
            this.Result = this.Strategy.Resolve(this.TargetMock.Object, this.ContextMock.Object);
        }
    }

    public abstract class WhenAGenericCollectionTypeIsRequested<T> : EnumerableTargetResolutionStrategyContext
    {
        protected WhenAGenericCollectionTypeIsRequested()
        {
            this.RequestedType = typeof (T);
            Resolve();
        }

        protected void PropertTypeShouldBeReturned()
        {
            this.RequestedType.IsAssignableFrom(this.Result.Value.GetType()).ShouldBeTrue();
        }
    }

    public class WhenAnEnumerableIsRequested : WhenAGenericCollectionTypeIsRequested<IEnumerable<IWeapon>>
    {
        [Fact]
        public void AnEnumerableShouldBeReturned()
        {
            this.PropertTypeShouldBeReturned();
        }
    }

    public class WhenAnArrayIsRequested : WhenAGenericCollectionTypeIsRequested<IWeapon[]>
    {
        [Fact]
        public void AnArrayShouldBeReturned()
        {
            this.PropertTypeShouldBeReturned();
        }
    }

    public class WhenAnIListIsRequested : WhenAGenericCollectionTypeIsRequested<IList<IWeapon>>
    {
        [Fact]
        public void AnIListShouldBeReturned()
        {
            this.PropertTypeShouldBeReturned();
        }
    }

    public class WhenAListIsRequested : WhenAGenericCollectionTypeIsRequested<List<IWeapon>>
    {
        [Fact]
        public void AListShouldBeReturned()
        {
            this.PropertTypeShouldBeReturned();
        }
    }

    public class WhenAnICollectionIsRequested : WhenAGenericCollectionTypeIsRequested<ICollection<IWeapon>>
    {
        [Fact]
        public void AnICollectionShouldBeReturned()
        {
            this.PropertTypeShouldBeReturned();
        }
    }

    public class WhenRequestTypeIsNotAGenericCollectionType : EnumerableTargetResolutionStrategyContext
    {
        public WhenRequestTypeIsNotAGenericCollectionType()
        {
            this.RequestedType = typeof (IWeapon);
            this.Resolve();
        }

        [Fact]
        public void TheResultShouldBeNone()
        {
            this.Result.ShouldBe(Maybe<object>.None);
        }
    }

    public class WhenNoMatchingTypesAreRegistered : EnumerableTargetResolutionStrategyContext
    {
        public WhenNoMatchingTypesAreRegistered()
        {
            this.RegisteredTypes.Clear();
            this.RequestedType = typeof(IEnumerable<IWeapon>);
            this.Resolve();
        }

        [Fact]
        public void AnEmptyEnumerableShouldBeReturned()
        {
            ((IEnumerable<IWeapon>)Result.Value).ShouldBeEmpty();
        }
    }

    public class WhenOneMatchingTypeIsRegistered : EnumerableTargetResolutionStrategyContext
    {
        public WhenOneMatchingTypeIsRegistered()
        {
            this.RegisteredTypes.Add(new Shuriken());
            this.RequestedType = typeof(IEnumerable<IWeapon>);
            this.Resolve();
        }

        [Fact]
        public void ASingleValueShouldBeReturned()
        {
            ((IEnumerable<IWeapon>)Result.Value).ShouldContainSingle();
        }
    }

    public class WhenMultipleMatchingTypesAreRegistered : EnumerableTargetResolutionStrategyContext
    {
        private readonly IEnumerable<IWeapon> collection;

        public WhenMultipleMatchingTypesAreRegistered()
        {
            this.RegisteredTypes.Add(new Shuriken());
            this.RegisteredTypes.Add(new Sword());
            this.RegisteredTypes.Add(new ShortSword());
            this.RequestedType = typeof(IEnumerable<IWeapon>);
            this.Resolve();
            this.collection = ((IEnumerable<IWeapon>) this.Result.Value);
        }

        [Fact]
        public void AllValuesShouldBeReturned()
        {
            foreach (var weapon in RegisteredTypes)
            {
                this.collection.ShouldContain(weapon);
            }
        }

        [Fact]
        public void OnlyRegisteredValuesShouldBeReturned()
        {
            this.collection.Count().ShouldBe(this.RegisteredTypes.Count);
        }
    }
}