using System;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Strategies;
using Ninject.Planning.Bindings;
using Xunit;

namespace Ninject.Tests.Unit.BindingActionStrategyTests
{
	public class BindingActionStrategyContext
	{
		protected readonly BindingActionStrategy strategy;
		protected readonly Mock<IContext> contextMock;
		protected readonly Mock<IBinding> bindingMock;

		public BindingActionStrategyContext()
		{
			contextMock = new Mock<IContext>();
			bindingMock = new Mock<IBinding>();
			strategy = new BindingActionStrategy();
		}
	}

	public class WhenActivateIsCalled : BindingActionStrategyContext
	{
		[Fact]
		public void StrategyInvokesActivationActionsDefinedInBinding()
		{
			bool action1WasCalled = false;
			bool action2WasCalled = false;

			Action<IContext> action1 = c => action1WasCalled = true;
			Action<IContext> action2 = c => action2WasCalled = true;
			var actions = new[] { action1, action2 };

			contextMock.SetupGet(x => x.Binding).Returns(bindingMock.Object);
			bindingMock.SetupGet(x => x.ActivationActions).Returns(actions);
			strategy.Activate(contextMock.Object);

			action1WasCalled.ShouldBeTrue();
			action2WasCalled.ShouldBeTrue();
		}
	}

	public class WhenDeactivateIsCalled : BindingActionStrategyContext
	{
		[Fact]
		public void StrategyInvokesDeactivationActionsDefinedInBinding()
		{
			bool action1WasCalled = false;
			bool action2WasCalled = false;

			Action<IContext> action1 = c => action1WasCalled = true;
			Action<IContext> action2 = c => action2WasCalled = true;
			var actions = new[] { action1, action2 };

			contextMock.SetupGet(x => x.Binding).Returns(bindingMock.Object);
			bindingMock.SetupGet(x => x.DeactivationActions).Returns(actions);
			strategy.Deactivate(contextMock.Object);

			action1WasCalled.ShouldBeTrue();
			action2WasCalled.ShouldBeTrue();
		}
	}
}