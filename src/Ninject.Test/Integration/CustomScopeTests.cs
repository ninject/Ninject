namespace Ninject.Tests.Integration.CustomScopeTests
{
	using System;
	using System.Linq;
	using Ninject.Parameters;
	using Ninject.Tests.Fakes;
#if SILVERLIGHT
#if SILVERLIGHT_MSTEST
    using MsTest.Should;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Assert = AssertWithThrows;
    using Fact = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
#else
    using UnitDriven;
    using UnitDriven.Should;
    using Assert = AssertWithThrows;
    using Fact = UnitDriven.TestMethodAttribute;
#endif
#else
	using Ninject.Tests.MSTestAttributes;
	using Xunit;
	using Xunit.Should;
#endif

	public class CustomScopeContext
	{
		protected StandardKernel kernel;

		public CustomScopeContext()
		{
			this.SetUp();
		}

		[TestInitialize]
		public void SetUp()
		{
			this.kernel = new StandardKernel();
		}
	}

	[TestClass]
	public class CustomScopeUsedAsSingleton : CustomScopeContext
	{
		[Fact]
		public void FirstActivatedInstanceIsReused()
		{
			kernel.Bind<IWarrior>().To<Samurai>().InScope(ctx => {
				return "";
			});

			// we expect the same object, even if the constructor arguments are different in the requests
			// ie. the constructor arguments of the second request are discarded
			var instance1 = kernel.Get<IWarrior>(new ConstructorArgument("weapon", new Dagger()));
			var instance2 = kernel.Get<IWarrior>(new ConstructorArgument("weapon", new Shuriken()));

			instance1.ShouldBeSameAs(instance2);
			instance2.Weapon.ShouldBeInstanceOf(typeof(Dagger));
		}
	}

	[TestClass]
	public class CustomScopeUsedAsTransient : CustomScopeContext
	{
		[Fact]
		public void NoInstanceIsReused()
		{
			kernel.Bind<IWarrior>().To<Samurai>().InScope(ctx => {
				return new Object();
			});

			// we expect different objects, even if the constructor arguments are the same
			IWeapon weapon = new Dagger();
			var instance1 = kernel.Get<IWarrior>(new ConstructorArgument("weapon", weapon));
			var instance2 = kernel.Get<IWarrior>(new ConstructorArgument("weapon", weapon));

			instance1.ShouldNotBeSameAs(instance2);
			instance1.Weapon.ShouldBeSameAs(instance2.Weapon);
		}
	}

	[TestClass]
	public class CustomScopeBasedOnConstructorArgument : CustomScopeContext
	{
		[Fact]
		public void MatchedConstructorInstanceIsReused()
		{
			kernel.Bind<IWarrior>().To<Samurai>().InScope(ctx => {
				var parameter = ctx.Parameters.OfType<ConstructorArgument>().Where(p => p.Name == "weapon").SingleOrDefault();
				return parameter != null ? parameter.GetValue(ctx, null) : "";
			});

			// instance will only be the same when constructor argument is exactly the same!
			IWeapon dagger = new Dagger();
			var instance1 = kernel.Get<IWarrior>(new ConstructorArgument("weapon", dagger));
			var instance2 = kernel.Get<IWarrior>(new ConstructorArgument("weapon", new Shuriken()));
			var instance3 = kernel.Get<IWarrior>(new ConstructorArgument("weapon", new Dagger()));
			var instance4 = kernel.Get<IWarrior> (new ConstructorArgument("weapon", dagger));

			instance1.ShouldNotBeSameAs(instance2);
			instance1.ShouldNotBeSameAs(instance3);
			instance1.ShouldBeSameAs(instance4);
		}
	}

}
