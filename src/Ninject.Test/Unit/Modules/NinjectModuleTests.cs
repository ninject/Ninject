using Moq;
using Ninject.Components;
using Ninject.Modules;
using Ninject.Planning.Bindings;
using System;
using System.Threading;
using Xunit;

namespace Ninject.Tests.Unit.Modules
{
    public class NinjectModuleTests
    {
        protected Mock<IKernelConfiguration> KernelConfigurationMock { get; }
        protected Mock<INinjectSettings> SettingsMock { get; }
        protected Mock<IComponentContainer> ComponentsMock { get; }
        protected Mock<IBinding> BindingMock { get; }
        protected MyNinjectModule NinjectModule { get; }

        public NinjectModuleTests()
        {
            KernelConfigurationMock = new Mock<IKernelConfiguration>(MockBehavior.Strict);
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
                KernelConfigurationMock.Setup(p => p.Components).Returns(ComponentsMock.Object);
                NinjectModule.OnLoad(KernelConfigurationMock.Object, SettingsMock.Object);
            }

            [Fact]
            public void NoArgumentNullCheckShouldBePerformed()
            {
                const IBinding binding = null;

                KernelConfigurationMock.Setup(p => p.AddBinding(binding));

                NinjectModule.AddBinding(binding);

                KernelConfigurationMock.Verify(p => p.AddBinding(binding), Times.Once());
            }

            [Fact]
            public void CallShouldBeDelegatedToKernelConfiguration()
            {
                KernelConfigurationMock.Setup(p => p.AddBinding(BindingMock.Object));

                NinjectModule.AddBinding(BindingMock.Object);

                KernelConfigurationMock.Verify(p => p.AddBinding(BindingMock.Object), Times.Once());
            }

            [Fact]
            public void BindingShouldBeAddedToBindings()
            {
                KernelConfigurationMock.Setup(p => p.AddBinding(BindingMock.Object));

                NinjectModule.AddBinding(BindingMock.Object);

                Assert.Equal(1, NinjectModule.Bindings.Count);
                Assert.True(NinjectModule.Bindings.Contains(BindingMock.Object));
            }
        }

        public class WhenOnLoadIsCalled : NinjectModuleTests
        {
            [Fact]
            public void ArgumentNullExceptionShouldBeThrownWhenKernelConfigurationIsNull()
            {
                const IKernelConfiguration kernelConfiguration = null;

                var actual = Assert.Throws<ArgumentNullException>(() => NinjectModule.OnLoad(kernelConfiguration, SettingsMock.Object));

                Assert.Null(actual.InnerException);
                Assert.Equal(nameof(kernelConfiguration), actual.ParamName);
            }


            [Fact]
            public void ArgumentNullExceptionShouldBeThrownWhenSettingsIsNull()
            {
                const INinjectSettings settings = null;

                var actual = Assert.Throws<ArgumentNullException>(() => NinjectModule.OnLoad(KernelConfigurationMock.Object, settings));

                Assert.Null(actual.InnerException);
                Assert.Equal(nameof(settings), actual.ParamName);
            }

            [Fact]
            public void XernelConfigurationShouldBeAssigned()
            {
                KernelConfigurationMock.Setup(p => p.Components).Returns(ComponentsMock.Object);

                NinjectModule.OnLoad(KernelConfigurationMock.Object, SettingsMock.Object);

                Assert.NotNull(NinjectModule.KernelConfiguration);
                Assert.Same(KernelConfigurationMock.Object, NinjectModule.KernelConfiguration);
            }

            [Fact]
            public void SettingsShouldBeAssigned()
            {
                KernelConfigurationMock.Setup(p => p.Components).Returns(ComponentsMock.Object);

                NinjectModule.OnLoad(KernelConfigurationMock.Object, SettingsMock.Object);

                Assert.NotNull(NinjectModule.Settings);
                Assert.Same(SettingsMock.Object, NinjectModule.Settings);
            }

            [Fact]
            public void ComponentsShouldBeAssigned()
            {
                KernelConfigurationMock.Setup(p => p.Components).Returns(ComponentsMock.Object);

                NinjectModule.OnLoad(KernelConfigurationMock.Object, SettingsMock.Object);

                Assert.NotNull(NinjectModule.Components);
                Assert.Same(ComponentsMock.Object, NinjectModule.Components);

                KernelConfigurationMock.Verify(p => p.Components, Times.Once());
            }

            [Fact]
            public void LoadShouldBeCalled()
            {
                KernelConfigurationMock.Setup(p => p.Components).Returns(ComponentsMock.Object);

                NinjectModule.OnLoad(KernelConfigurationMock.Object, SettingsMock.Object);

                Assert.Equal(1, NinjectModule.LoadCount);
            }
        }

        public class WhenOnUnloadIsCalledAfterOnLoad : NinjectModuleTests
        {
            public WhenOnUnloadIsCalledAfterOnLoad()
            {
                KernelConfigurationMock.Setup(p => p.Components).Returns(ComponentsMock.Object);
                NinjectModule.OnLoad(KernelConfigurationMock.Object, SettingsMock.Object);
            }

            [Fact]
            public void UnloadShouldBeCalled()
            {
                NinjectModule.OnUnload();

                Assert.Equal(1, NinjectModule.UnloadCount);
            }

            [Fact]
            public void BindingsShouldBeRemovedFromKernelConfiguration()
            {
                NinjectModule.Bindings.Add(BindingMock.Object);

                KernelConfigurationMock.Setup(p => p.RemoveBinding(BindingMock.Object));

                NinjectModule.OnUnload();

                KernelConfigurationMock.Verify(p => p.RemoveBinding(BindingMock.Object), Times.Once());
            }

            [Fact(Skip="https://github.com/ninject/Ninject/issues/311")]
            public void BindingsShouldCleared()
            {
                NinjectModule.Bindings.Add(BindingMock.Object);

                KernelConfigurationMock.Setup(p => p.RemoveBinding(BindingMock.Object));

                NinjectModule.OnUnload();

                Assert.Empty(NinjectModule.Bindings);
            }

            [Fact]
            public void ComponentsShouldBeReset()
            {
                NinjectModule.OnUnload();

                Assert.Null(NinjectModule.Components);
            }

            [Fact]
            public void KernelConfigurationShouldBeReset()
            {
                NinjectModule.OnUnload();

                Assert.Null(NinjectModule.KernelConfiguration);
            }
        }

        public class WhenRemoveBindingIsCalledAfterOnLoad : NinjectModuleTests
        {
            public WhenRemoveBindingIsCalledAfterOnLoad()
            {
                KernelConfigurationMock.Setup(p => p.Components).Returns(ComponentsMock.Object);
                NinjectModule.OnLoad(KernelConfigurationMock.Object, SettingsMock.Object);
            }

            [Fact]
            public void NoArgumentNullCheckShouldBePerformed()
            {
                const IBinding binding = null;

                KernelConfigurationMock.Setup(p => p.RemoveBinding(binding));

                NinjectModule.RemoveBinding(binding);

                KernelConfigurationMock.Verify(p => p.RemoveBinding(binding), Times.Once());
            }

            [Fact]
            public void CallShouldBeDelegatedToKernelConfiguration()
            {
                KernelConfigurationMock.Setup(p => p.RemoveBinding(BindingMock.Object));

                NinjectModule.RemoveBinding(BindingMock.Object);

                KernelConfigurationMock.Verify(p => p.RemoveBinding(BindingMock.Object), Times.Once());
            }

            [Fact]
            public void BindingShouldBeRemovedFromBindings()
            {
                NinjectModule.Bindings.Add(BindingMock.Object);

                KernelConfigurationMock.Setup(p => p.RemoveBinding(BindingMock.Object));

                NinjectModule.RemoveBinding(BindingMock.Object);

                Assert.Empty(NinjectModule.Bindings);
            }
        }

        public class WhenUnbindIsCalledAfterOnLoad : NinjectModuleTests
        {
            public WhenUnbindIsCalledAfterOnLoad()
            {
                KernelConfigurationMock.Setup(p => p.Components).Returns(ComponentsMock.Object);
                NinjectModule.OnLoad(KernelConfigurationMock.Object, SettingsMock.Object);
            }

            [Fact]
            public void NoArgumentNullCheckShouldBePerformed()
            {
                const Type service = null;

                KernelConfigurationMock.Setup(p => p.Unbind(service));

                NinjectModule.Unbind(service);

                KernelConfigurationMock.Verify(p => p.Unbind(service), Times.Once());
            }

            [Fact]
            public void CallShouldBeDelegatedToKernelConfiguration()
            {
                var service = typeof(string);

                KernelConfigurationMock.Setup(p => p.Unbind(service));

                NinjectModule.Unbind(service);

                KernelConfigurationMock.Verify(p => p.Unbind(service), Times.Once());
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

            public new IKernelConfiguration KernelConfiguration
            {
                get { return base.KernelConfiguration; }
            }

            public new INinjectSettings Settings
            {
                get { return base.Settings; }
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
