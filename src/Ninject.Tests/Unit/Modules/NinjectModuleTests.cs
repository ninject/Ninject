namespace Ninject.Tests.Unit.Modules
{
    using Moq;
    using Ninject.Components;
    using Ninject.Modules;
    using Ninject.Planning.Bindings;
    using System;
    using System.Threading;
    using Xunit;

    public class NinjectModuleTests
    {
        protected Mock<IKernel> KernelMock { get; }
        protected Mock<INinjectSettings> SettingsMock { get; }
        protected Mock<IComponentContainer> ComponentsMock { get; }
        protected Mock<IBinding> BindingMock { get; }
        protected MyNinjectModule NinjectModule { get; }

        public NinjectModuleTests()
        {
            KernelMock = new Mock<IKernel>(MockBehavior.Strict);
            SettingsMock = new Mock<INinjectSettings>(MockBehavior.Strict);
            ComponentsMock = new Mock<IComponentContainer>(MockBehavior.Strict);
            BindingMock = new Mock<IBinding>(MockBehavior.Strict);

            NinjectModule = new MyNinjectModule();
        }

        public class WhenAddBindingIsCalledBeforeOnLoad : NinjectModuleTests
        {
            [Fact]
            public void NullReferenceExceptionShouldBeThrown()
            {
                Assert.Throws<NullReferenceException>(() => NinjectModule.AddBinding(BindingMock.Object));
            }

            [Fact]
            public void BindingShouldNotBeAddedToBindings()
            {
                Assert.Throws<NullReferenceException>(() => NinjectModule.AddBinding(BindingMock.Object));

                Assert.Empty(NinjectModule.Bindings);
            }
        }

        public class WhenAddBindingIsCalledAfterOnLoad : NinjectModuleTests
        {
            public WhenAddBindingIsCalledAfterOnLoad()
            {
                KernelMock.Setup(p => p.Components).Returns(ComponentsMock.Object);
                NinjectModule.OnLoad(KernelMock.Object);
            }

            [Fact]
            public void NoArgumentNullCheckShouldBePerformed()
            {
                const IBinding binding = null;

                KernelMock.Setup(p => p.AddBinding(binding));

                NinjectModule.AddBinding(binding);

                KernelMock.Verify(p => p.AddBinding(binding), Times.Once());
            }

            [Fact]
            public void CallShouldBeDelegatedToKernel()
            {
                KernelMock.Setup(p => p.AddBinding(BindingMock.Object));

                NinjectModule.AddBinding(BindingMock.Object);

                KernelMock.Verify(p => p.AddBinding(BindingMock.Object), Times.Once());
            }

            [Fact]
            public void BindingShouldBeAddedToBindings()
            {
                KernelMock.Setup(p => p.AddBinding(BindingMock.Object));

                NinjectModule.AddBinding(BindingMock.Object);

                Assert.Equal(1, NinjectModule.Bindings.Count);
                Assert.True(NinjectModule.Bindings.Contains(BindingMock.Object));
            }
        }

        public class WhenOnLoadIsCalled : NinjectModuleTests
        {
            [Fact]
            public void ArgumentNullExceptionShouldBeThrownWhenKernelIsNull()
            {
                const IKernel kernel = null;

                var actual = Assert.Throws<ArgumentNullException>(() => NinjectModule.OnLoad(kernel));

                Assert.Null(actual.InnerException);
                Assert.Equal(nameof(kernel), actual.ParamName);
            }


            [Fact]
            public void KernelShouldBeAssigned()
            {
                KernelMock.Setup(p => p.Components).Returns(ComponentsMock.Object);

                NinjectModule.OnLoad(KernelMock.Object);

                Assert.NotNull(NinjectModule.Kernel);
                Assert.Same(KernelMock.Object, NinjectModule.Kernel);
            }

            [Fact]
            public void LoadShouldBeCalled()
            {
                KernelMock.Setup(p => p.Components).Returns(ComponentsMock.Object);

                NinjectModule.OnLoad(KernelMock.Object);

                Assert.Equal(1, NinjectModule.LoadCount);
            }
        }

        public class WhenOnUnloadIsCalledAfterOnLoad : NinjectModuleTests
        {
            public WhenOnUnloadIsCalledAfterOnLoad()
            {
                KernelMock.Setup(p => p.Components).Returns(ComponentsMock.Object);
                NinjectModule.OnLoad(KernelMock.Object);
            }

            [Fact]
            public void UnloadShouldBeCalled()
            {
                NinjectModule.OnUnload(KernelMock.Object);

                Assert.Equal(1, NinjectModule.UnloadCount);
            }

            [Fact]
            public void BindingsShouldBeRemovedFromKernel()
            {
                NinjectModule.Bindings.Add(BindingMock.Object);

                KernelMock.Setup(p => p.RemoveBinding(BindingMock.Object));

                NinjectModule.OnUnload(KernelMock.Object);

                KernelMock.Verify(p => p.RemoveBinding(BindingMock.Object), Times.Once());
            }

            [Fact(Skip="https://github.com/ninject/Ninject/issues/311")]
            public void BindingsShouldCleared()
            {
                NinjectModule.Bindings.Add(BindingMock.Object);

                KernelMock.Setup(p => p.RemoveBinding(BindingMock.Object));

                NinjectModule.OnUnload(KernelMock.Object);

                Assert.Empty(NinjectModule.Bindings);
            }

            [Fact]
            public void KernelShouldBeReset()
            {
                NinjectModule.OnUnload(KernelMock.Object);

                Assert.Null(NinjectModule.Kernel);
            }
        }

        public class WhenRemoveBindingIsCalledAfterOnLoad : NinjectModuleTests
        {
            public WhenRemoveBindingIsCalledAfterOnLoad()
            {
                KernelMock.Setup(p => p.Components).Returns(ComponentsMock.Object);
                NinjectModule.OnLoad(KernelMock.Object);
            }

            [Fact]
            public void NoArgumentNullCheckShouldBePerformed()
            {
                const IBinding binding = null;

                KernelMock.Setup(p => p.RemoveBinding(binding));

                NinjectModule.RemoveBinding(binding);

                KernelMock.Verify(p => p.RemoveBinding(binding), Times.Once());
            }

            [Fact]
            public void CallShouldBeDelegatedToKernel()
            {
                KernelMock.Setup(p => p.RemoveBinding(BindingMock.Object));

                NinjectModule.RemoveBinding(BindingMock.Object);

                KernelMock.Verify(p => p.RemoveBinding(BindingMock.Object), Times.Once());
            }

            [Fact]
            public void BindingShouldBeRemovedFromBindings()
            {
                NinjectModule.Bindings.Add(BindingMock.Object);

                KernelMock.Setup(p => p.RemoveBinding(BindingMock.Object));

                NinjectModule.RemoveBinding(BindingMock.Object);

                Assert.Empty(NinjectModule.Bindings);
            }
        }

        public class WhenUnbindIsCalledAfterOnLoad : NinjectModuleTests
        {
            public WhenUnbindIsCalledAfterOnLoad()
            {
                KernelMock.Setup(p => p.Components).Returns(ComponentsMock.Object);
                NinjectModule.OnLoad(KernelMock.Object);
            }

            [Fact]
            public void NoArgumentNullCheckShouldBePerformed()
            {
                const Type service = null;

                KernelMock.Setup(p => p.Unbind(service));

                NinjectModule.Unbind(service);

                KernelMock.Verify(p => p.Unbind(service), Times.Once());
            }

            [Fact]
            public void CallShouldBeDelegatedToKernel()
            {
                var service = typeof(string);

                KernelMock.Setup(p => p.Unbind(service));

                NinjectModule.Unbind(service);

                KernelMock.Verify(p => p.Unbind(service), Times.Once());
            }
        }

        public class MyNinjectModule : NinjectModule
        {
            private int _loadCount;
            private int _unloadCount;

            public int LoadCount
            {
                get { return _loadCount; }
            }

            public int UnloadCount
            {
                get { return _loadCount; }
            }

            public new IKernel Kernel
            {
                get { return base.Kernel; }
            }

            public override void Load()
            {
                Interlocked.Increment(ref _loadCount);
            }

            public override void Unload()
            {
                Interlocked.Increment(ref _unloadCount);
            }
        }
    }
}
