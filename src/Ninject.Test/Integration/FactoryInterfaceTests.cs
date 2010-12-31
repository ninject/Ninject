namespace Ninject.Tests.Integration.FactoryInterfaceTests
{
	using System;
	using System.Linq;
	using System.Reflection;
	using Ninject.Parameters;
	using Ninject.Activation;
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

	[TestClass]
	public class FactoryUsageExamples
	{
		private StandardKernel kernel;

		public FactoryUsageExamples()
		{
			this.SetUp();
		}

		[TestInitialize]
		public void SetUp()
		{
			this.kernel = new StandardKernel();
		}

		[Fact]
		public void RequestAFactoryObject()
		{
			// Bind ToSelf is implicit, so the following two lines are optional
			// kernel.Bind (typeof (Dagger)).ToSelf ();
			// kernel.Bind (typeof (Factory<>)).ToSelf ();

			var factoryInstance1 = kernel.Get<Factory<Dagger>> ();
			var factoryInstance2 = kernel.Get<Factory<Dagger>> ();

			factoryInstance1.ShouldNotBeNull ();
			factoryInstance1.ShouldNotBeSameAs (factoryInstance2);

			var instance1 = factoryInstance1.Get ();
			var instance2 = factoryInstance1.Get ();
			var instance3 = factoryInstance2.Get ();

			instance1.ShouldNotBeNull();
			instance1.ShouldBeInstanceOf<Dagger> ();
			instance1.ShouldNotBeSameAs (instance2);
			instance1.ShouldNotBeSameAs (instance3);
		}

		[Fact]
		public void RequestASingletonFactoryObject ()
		{
			kernel.Bind<Dagger> ().ToSelf ().InSingletonScope ();

			var factoryInstance1 = kernel.Get<Factory<Dagger>> ();
			var factoryInstance2 = kernel.Get<Factory<Dagger>> ();

			factoryInstance1.ShouldNotBeNull ();
			// the factory itself does not need to be a singleton, and it is not in this case
			factoryInstance1.ShouldNotBeSameAs (factoryInstance2);

			var instance1 = factoryInstance1.Get ();
			var instance2 = factoryInstance1.Get ();
			var instance3 = factoryInstance2.Get ();

			instance1.ShouldNotBeNull ();
			instance1.ShouldBeInstanceOf<Dagger> ();
			// the instances should be always the same
			instance1.ShouldBeSameAs (instance2);
			instance1.ShouldBeSameAs (instance3);
		}

		[Fact]
		public void RequestAFactoryWithConstructorObject()
		{
			// define a custom scope to bind Samurai
			kernel.Bind<Samurai> ().ToSelf ().InScope (ctx => {
				var parameter = ctx.Parameters.OfType<ConstructorArgument> ().Where (p => p.Name == "weapon").SingleOrDefault ();
				return parameter != null ? parameter.GetValue (ctx, null) : "";
			});

			var factoryInstance1 = kernel.Get<Factory<Samurai, IWeapon>> ();
			var factoryInstance2 = kernel.Get<Factory<Samurai, IWeapon>> ();

			factoryInstance1.ShouldNotBeNull ();
			factoryInstance1.ShouldNotBeSameAs (factoryInstance2);

			IWeapon dagger = new Dagger ();
			var instance1 = factoryInstance1.Get (dagger);
			var instance2 = factoryInstance1.Get (dagger);
			var instance3 = factoryInstance2.Get (dagger);
			var instance4 = factoryInstance2.Get (new Dagger ());

			instance1.ShouldNotBeNull ();
			instance1.ShouldBeInstanceOf<Samurai> ();
			instance1._weapon.ShouldBeSameAs (dagger);
			// the instances should be always the same if the Constructor Argument is the same
			instance1.ShouldBeSameAs (instance2);
			instance1.ShouldBeSameAs (instance3);
			instance1.ShouldNotBeSameAs (instance4);
		}

	}

	public class Factory<T> 
	{
		private readonly IKernel Kernel;

		public Factory (IKernel Kernel)
		{
			this.Kernel = Kernel;
		}

		public T Get ()
		{
			return Kernel.Get<T> ();
		}
	}

	public class Factory<T, Argument1Type> where T : class
	{
		private readonly IKernel Kernel;
		private readonly string Argument1Name;

		public Factory (IKernel Kernel)
		{
			this.Kernel = Kernel;

			// Find the name of the constructor argument. Asume the same name if T has multiple constructors with the same type
			foreach (var Constructor in typeof (T).GetConstructors ()) {
				foreach (var Parameter in Constructor.GetParameters ()) {
					if (typeof (Argument1Type) == Parameter.ParameterType.UnderlyingSystemType) {
						Argument1Name = Parameter.Name;
					}
				}
			}
		}

		public T Get (Argument1Type Argument1)
		{
			return Kernel.Get<T> (new ConstructorArgument (Argument1Name, Argument1));
		}
	}

}
