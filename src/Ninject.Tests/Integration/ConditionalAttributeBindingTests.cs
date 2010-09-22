namespace Ninject.Tests.Integration
{
    using System;
    using Ninject.Infrastructure.Disposal;
#if SILVERLIGHT
#if SILVERLIGHT_MSTEST
    using MsTest.Should;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Fact = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
#else
    using UnitDriven;
    using UnitDriven.Should;
    using Fact = UnitDriven.TestMethodAttribute;
#endif
#else
    using Ninject.Tests.MSTestAttributes;
    using Xunit;
    using Xunit.Should;
#endif

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
        #region IAttackAbility Members

        public int Strength
        {
            get { return -1; }
        }

        #endregion
    }

    public class WeakAttack : IAttackAbility
    {
        #region IAttackAbility Members

        public int Strength
        {
            get { return 1; }
        }

        #endregion
    }

    public class StrongAttack : IAttackAbility
    {
        #region IAttackAbility Members

        public int Strength
        {
            get { return 10; }
        }

        #endregion
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
        #region IWeapon Members

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

        #endregion
    }

    [TestClass]
    public class ConditionalAttributeBindingTests : DisposableObject
    {
        protected IKernel kernel;

        public ConditionalAttributeBindingTests()
        {
            this.SetUp();
        }

        [TestInitialize]
        public void SetUp()
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
            attackAbility.ShouldBeInstanceOf<UnknownAttack>();
        }

        [Fact]
        public void PropertiesAreInjectMatchingAttributeBindings()
        {
            var hammer = this.kernel.Get<IVarialbeWeapon>();
            hammer.ShouldNotBeNull();
            hammer.StrongAttack.ShouldBeInstanceOf<StrongAttack>();
            hammer.WeakAttack.ShouldBeInstanceOf<WeakAttack>();
            hammer.WtfAttack.ShouldBeInstanceOf<UnknownAttack>();
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