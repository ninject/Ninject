using System;
using Ninject.Infrastructure.Disposal;
using Xunit;

namespace Ninject.Tests.Integration
{
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

	public class ConditionalAttributeBindingTests : DisposableObject
	{
		protected IKernel _kernel;

		public ConditionalAttributeBindingTests()
		{
			_kernel = new StandardKernel();
			_kernel.Bind<IVarialbeWeapon>().To<Hammer>();
			_kernel.Bind<IAttackAbility>().To<UnknownAttack>();
			_kernel.Bind<IAttackAbility>().To<StrongAttack>().WhenTargetHas<StrongAttribute>();
			_kernel.Bind<IAttackAbility>().To<WeakAttack>().WhenTargetHas<WeakAttribute>();
		}

		[Fact]
		public void DefaultInstanceIsResolvedWhenNoAttributesMatch()
		{
			var attackAbility = _kernel.Get<IAttackAbility>();
			Assert.IsType<UnknownAttack>( attackAbility );
		}

		[Fact]
		public void PropertiesAreInjectMatchingAttributeBindings()
		{
			var hammer = _kernel.Get<IVarialbeWeapon>();
			Assert.NotNull( hammer );
			Assert.IsType<StrongAttack>( hammer.StrongAttack );
			Assert.IsType<WeakAttack>( hammer.WeakAttack );
			Assert.IsType<UnknownAttack>( hammer.WtfAttack );
		}

		public override void Dispose( bool disposing )
		{
			if ( disposing && !IsDisposed )
			{
				_kernel.Dispose();
				_kernel = null;
			}
			base.Dispose( disposing );
		}
	}
}