using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Integration.EnumerableDependenciesTests
{
	public abstract class EnumerableDependenciesContext
	{
		protected readonly StandardKernel kernel;

		protected EnumerableDependenciesContext()
		{
			kernel = new StandardKernel();
		}

		protected abstract void VerifyInjection(IParent parent);
	}

	public class UnconstrainedDependenciesContext : EnumerableDependenciesContext
	{
		protected override void VerifyInjection(IParent parent)
		{
			parent.ShouldNotBeNull();
			parent.Children.ShouldNotBeNull();
			parent.Children.Count.ShouldBe(2);
			parent.Children[0].ShouldBeInstanceOf<ChildA>();
			parent.Children[1].ShouldBeInstanceOf<ChildB>();
		}
	}

	public class ConstrainedDependenciesContext : EnumerableDependenciesContext
	{
		protected override void VerifyInjection(IParent parent)
		{
			parent.ShouldNotBeNull();
			parent.Children.ShouldNotBeNull();
			parent.Children.Count.ShouldBe(1);
			parent.Children[0].ShouldBeInstanceOf<ChildB>();
		}
	}

	public class WhenServiceRequestsUnconstrainedEnumerableOfDependencies : UnconstrainedDependenciesContext
	{
		[Fact]
		public void ServiceIsInjectedWithEnumeratorOfAllAvailableDependencies()
		{
			kernel.Bind<IParent>().To<RequestsEnumerable>();
			kernel.Bind<IChild>().To<ChildA>();
			kernel.Bind<IChild>().To<ChildB>();

			var parent = kernel.Get<IParent>();

			VerifyInjection(parent);
		}
	}

	public class WhenServiceRequestsUnconstrainedListOfDependencies : UnconstrainedDependenciesContext
	{
		[Fact]
		public void ServiceIsInjectedWithListOfAllAvailableDependencies()
		{
			kernel.Bind<IParent>().To<RequestsList>();
			kernel.Bind<IChild>().To<ChildA>();
			kernel.Bind<IChild>().To<ChildB>();

			var parent = kernel.Get<IParent>();

			VerifyInjection(parent);
		}

		[Fact]
		public void ServiceIsInjectedWithListOfAllAvailableDependenciesWhenDefaultCtorIsAvailable()
		{
			kernel.Bind<IParent>().To<RequestsListWithDefaultCtor>();
			kernel.Bind<IChild>().To<ChildA>();
			kernel.Bind<IChild>().To<ChildB>();

			var parent = kernel.Get<IParent>();

			VerifyInjection(parent);
		}
	}

	public class WhenServiceRequestsUnconstrainedArrayOfDependencies : UnconstrainedDependenciesContext
	{
		[Fact]
		public void ServiceIsInjectedWithArrayOfAllAvailableDependencies()
		{
			kernel.Bind<IParent>().To<RequestsArray>();
			kernel.Bind<IChild>().To<ChildA>();
			kernel.Bind<IChild>().To<ChildB>();

			var parent = kernel.Get<IParent>();

			VerifyInjection(parent);
		}

		[Fact]
		public void ServiceIsInjectedWithArrayOfAllAvailableDependenciesWhenDefaultCtorIsAvailable()
		{
			kernel.Bind<IParent>().To<RequestsArrayWithDefaultCtor>();
			kernel.Bind<IChild>().To<ChildA>();
			kernel.Bind<IChild>().To<ChildB>();

			var parent = kernel.Get<IParent>();

			VerifyInjection(parent);
		}
	}

	public class WhenServiceRequestsConstrainedEnumerableOfDependencies : ConstrainedDependenciesContext
	{
		[Fact]
		public void ServiceIsInjectedWithAllDependenciesThatMatchTheConstraint()
		{
			kernel.Bind<IParent>().To<RequestsConstrainedEnumerable>();
			kernel.Bind<IChild>().To<ChildA>().Named("joe");
			kernel.Bind<IChild>().To<ChildB>().Named("bob");

			var parent = kernel.Get<IParent>();

			VerifyInjection(parent);
		}
	}

	public class WhenServiceRequestsConstrainedListOfDependencies : ConstrainedDependenciesContext
	{
		[Fact]
		public void ServiceIsInjectedWithAllDependenciesThatMatchTheConstraint()
		{
			kernel.Bind<IParent>().To<RequestsConstrainedList>();
			kernel.Bind<IChild>().To<ChildA>().Named("joe");
			kernel.Bind<IChild>().To<ChildB>().Named("bob");

			var parent = kernel.Get<IParent>();

			VerifyInjection(parent);
		}
	}

	public class WhenServiceRequestsConstrainedArrayOfDependencies : ConstrainedDependenciesContext
	{
		[Fact]
		public void ServiceIsInjectedWithAllDependenciesThatMatchTheConstraint()
		{
			kernel.Bind<IParent>().To<RequestsConstrainedArray>();
			kernel.Bind<IChild>().To<ChildA>().Named("joe");
			kernel.Bind<IChild>().To<ChildB>().Named("bob");

			var parent = kernel.Get<IParent>();

			VerifyInjection(parent);
		}
	}

	public interface IChild { }

	public class ChildA : IChild { }
	public class ChildB : IChild { }

	public interface IParent
	{
		IList<IChild> Children { get; }
	}

	public class RequestsEnumerable : IParent
	{
		public IList<IChild> Children { get; private set; }

		public RequestsEnumerable(IEnumerable<IChild> children)
		{
			Children = children.ToList();
		}
	}

	public class RequestsList : IParent
	{
		public IList<IChild> Children { get; private set; }

		public RequestsList(List<IChild> children)
		{
			Children = children;
		}
	}

	public class RequestsListWithDefaultCtor : RequestsList
	{
		public RequestsListWithDefaultCtor()
			: base(new List<IChild>())
		{
		}

		public RequestsListWithDefaultCtor(List<IChild> children)
			: base(children)
		{
		}
	}

	public class RequestsArray : IParent
	{
		public IList<IChild> Children { get; private set; }

		public RequestsArray(IChild[] children)
		{
			Children = children;
		}
	}

	public class RequestsArrayWithDefaultCtor : RequestsArray
	{
		public RequestsArrayWithDefaultCtor()
			:base(new IChild[0])
		{
		}

		public RequestsArrayWithDefaultCtor(IChild[] children)
			:base(children)
		{
		}
	}

	public class RequestsConstrainedEnumerable : IParent
	{
		public IList<IChild> Children { get; private set; }

		public RequestsConstrainedEnumerable([Named("bob")] IEnumerable<IChild> children)
		{
			Children = children.ToList();
		}
	}

	public class RequestsConstrainedList : IParent
	{
		public IList<IChild> Children { get; private set; }

		public RequestsConstrainedList([Named("bob")] List<IChild> children)
		{
			Children = children;
		}
	}

	public class RequestsConstrainedArray : IParent
	{
		public IList<IChild> Children { get; private set; }

		public RequestsConstrainedArray([Named("bob")] IChild[] children)
		{
			Children = children;
		}
	}
}