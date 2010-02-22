using System;
using Ninject.Tests.Fakes;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Integration
{
	public class AutoBoundContext
	{
		protected readonly StandardKernel kernel;

		public AutoBoundContext()
		{
			kernel = new StandardKernel();
		}
	}

	public class WhenGetIsCalledForInterfaceImplicitlyBound : AutoBoundContext
	{
		[Fact(Skip = "Requires implicit resolution, still debating this")]
		public void SingleInstanceIsReturnedWhenOneBindingIsRegistered()
		{
			kernel.BindTo<Sword>();

			var weapon = kernel.Get<IWeapon>();

			weapon.ShouldNotBeNull();
			weapon.ShouldBeInstanceOf<Sword>();
		}
  }
}
