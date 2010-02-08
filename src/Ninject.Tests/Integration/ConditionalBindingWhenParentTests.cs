using Ninject.Tests.Fakes;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Integration
{
	public class WhenParentContext
	{
		protected readonly StandardKernel kernel;

		public WhenParentContext()
		{
			kernel = new StandardKernel();
			kernel.Bind<Sword>().ToSelf().Named("Broken");
			kernel.Bind<Sword>().ToSelf().WhenParentNamed("Something");
		}
	}

	public class WhenParentNamed : WhenParentContext
	{
		[Fact]
		public void NamedInstanceAvailableEvenWithWhenBinding()
		{
			var instance = kernel.Get<Sword>("Broken");

			instance.ShouldNotBeNull();
			instance.ShouldBeInstanceOf(typeof(Sword));
		}
	}
}