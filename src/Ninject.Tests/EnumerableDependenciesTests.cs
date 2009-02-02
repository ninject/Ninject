using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Ninject.Tests.EnumerableDependenciesTests
{
	public class EnumerableDependenciesContext
	{
		protected readonly StandardKernel kernel;

		public EnumerableDependenciesContext()
		{
			kernel = new StandardKernel();
		}
	}

	public class WhenServiceRequestsUnconstrainedEnumerableOfDependencies : EnumerableDependenciesContext
	{
		[Fact]
		public void ServiceIsInjectedWithAllAvailableDependencies()
		{
			kernel.Bind<IParent>().To<RequestsEnumerable>();
			kernel.Bind<IChild>().To<ChildA>();
			kernel.Bind<IChild>().To<ChildB>();

			var parent = kernel.Get<IParent>();

			Assert.NotNull(parent);
			Assert.NotNull(parent.Children);
			Assert.Equal(2, parent.Children.Length);
			Assert.IsType<ChildA>(parent.Children[0]);
			Assert.IsType<ChildB>(parent.Children[1]);
		}
	}

	public class WhenServiceRequestsConstrainedEnumerableOfDependencies : EnumerableDependenciesContext
	{
		[Fact]
		public void ServiceIsInjectedWithAllDependenciesThatMatchTheConstraint()
		{
			kernel.Bind<IParent>().To<RequestsConstrainedEnumerable>();
			kernel.Bind<IChild>().To<ChildA>().WithName("joe");
			kernel.Bind<IChild>().To<ChildB>().WithName("bob");

			var parent = kernel.Get<IParent>();

			Assert.NotNull(parent);
			Assert.NotNull(parent.Children);
			Assert.Equal(1, parent.Children.Length);
			Assert.IsType<ChildB>(parent.Children[0]);
		}
	}

	public interface IChild { }

	public class ChildA : IChild { }
	public class ChildB : IChild { }

	public interface IParent
	{
		IChild[] Children { get; }
	}

	public class RequestsEnumerable : IParent
	{
		public IChild[] Children { get; private set; }

		public RequestsEnumerable(IEnumerable<IChild> children)
		{
			Children = children.ToArray();
		}
	}

	public class RequestsConstrainedEnumerable : IParent
	{
		public IChild[] Children { get; private set; }

		public RequestsConstrainedEnumerable([Named("bob")] IEnumerable<IChild> children)
		{
			Children = children.ToArray();
		}
	}
}