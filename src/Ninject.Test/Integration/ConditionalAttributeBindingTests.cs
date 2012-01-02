namespace Ninject.Tests.Integration
{
    using FluentAssertions;
    using Ninject.Infrastructure.Disposal;
    using Xunit;
    using Attribute = System.Attribute;

    public class WeakAttribute : Attribute
    {
    }

    public class StrongAttribute : Attribute
    {
    }

    public interface IAttackAbility
    {
        int Strength { get; }
    }

    public class UnknownAttack : IAttackAbility
    {
        public int Strength
        {
            get { return -1; }
        }
    }

    public class WeakAttack : IAttackAbility
    {
        public int Strength
        {
            get { return 1; }
        }
    }

    public class StrongAttack : IAttackAbility
    {
        public int Strength
        {
            get { return 10; }
        }
    }

    public interface IVarialbeWeapon
    {
        string Name { get; }
        IAttackAbility StrongAttack { get; set; }
        IAttackAbility WeakAttack { get; set; }
        IAttackAbility WtfAttack { get; set; }
    }

    public class Hammer : IVarialbeWeapon
    {
        [Inject]
        [Weak]
        public IAttackAbility WeakAttack { get; set; }

        [Inject]
        [Strong]
        public IAttackAbility StrongAttack { get; set; }

        [Inject]
        public IAttackAbility WtfAttack { get; set; }

        public string Name
        {
            get { return "hammer"; }
        }
    }

    public class ConditionalAttributeBindingTests : DisposableObject
    {
        protected IKernel kernel;

        public ConditionalAttributeBindingTests()
        {
            this.kernel = new StandardKernel();
            this.kernel.Bind<IVarialbeWeapon>().To<Hammer>();
            this.kernel.Bind<IAttackAbility>().To<UnknownAttack>();
            this.kernel.Bind<IAttackAbility>().To<StrongAttack>().WhenTargetHas<StrongAttribute>();
            this.kernel.Bind<IAttackAbility>().To<WeakAttack>().WhenTargetHas<WeakAttribute>();
        }

        [Fact]
        public void DefaultInstanceIsResolvedWhenNoAttributesMatch()
        {
            var attackAbility = this.kernel.Get<IAttackAbility>();
            attackAbility.Should().BeOfType<UnknownAttack>();
        }

        [Fact]
        public void PropertiesAreInjectMatchingAttributeBindings()
        {
            var hammer = this.kernel.Get<IVarialbeWeapon>();
            hammer.Should().NotBeNull();
            hammer.StrongAttack.Should().BeOfType<StrongAttack>();
            hammer.WeakAttack.Should().BeOfType<WeakAttack>();
            hammer.WtfAttack.Should().BeOfType<UnknownAttack>();
        }

        public override void Dispose( bool disposing )
        {
            if ( disposing && !IsDisposed )
            {
                this.kernel.Dispose();
                this.kernel = null;
            }
            base.Dispose( disposing );
        }
    }
}