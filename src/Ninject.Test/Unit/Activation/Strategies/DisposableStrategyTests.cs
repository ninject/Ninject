using Moq;
using System;
using Xunit;

using Ninject.Activation;
using Ninject.Activation.Strategies;
using Ninject.Tests.Fakes;
#if !NO_REMOTING
using Ninject.Tests.Infrastructure;
#endif // !NO_REMOTING

namespace Ninject.Tests.Unit.Activation.Strategies
{
    public class DisposableStrategyTests
    {
        private Mock<IContext> _contextMock;
        private DisposableStrategy _strategy;

        public DisposableStrategyTests()
        {
            _contextMock = new Mock<IContext>(MockBehavior.Strict);

            _strategy = new DisposableStrategy();
        }

        [Fact]
        public void Activate_NotTransparantProxy_Disposable()
        {
            var initializableMock = new Mock<IDisposable>(MockBehavior.Strict);
            var reference = new InstanceReference { Instance = initializableMock.Object };

            _strategy.Activate(_contextMock.Object, reference);

            Assert.Same(initializableMock.Object, reference.Instance);
            initializableMock.Verify(p => p.Dispose(), Times.Never);
        }

        [Fact]
        public void Activate_NotTransparantProxy_NotDisposable()
        {
            var nonDisposableMock = new Mock<IWarrior>(MockBehavior.Strict);
            var reference = new InstanceReference { Instance = nonDisposableMock.Object };

            _strategy.Activate(_contextMock.Object, reference);

            Assert.Same(nonDisposableMock.Object, reference.Instance);
        }

        [Fact]
        public void Activate_InstanceIsNull()
        {
            var reference = new InstanceReference();

            _strategy.Activate(_contextMock.Object, reference);

            Assert.Null(reference.Instance);
        }

        [Fact]
        public void Activate_ReferenceIsNull()
        {
            const InstanceReference reference = null;

            _strategy.Activate(_contextMock.Object, reference);
        }

#if !NO_REMOTING
        [Fact]
        public void Activate_TransparentProxy_Disposable()
        {
            using (var server = new RemotingServer())
            using (var client = new RemotingClient())
            {
                server.RegisterActivatedService(typeof(Disposable));

                var initializable = client.GetService<Disposable>();
                var reference = new InstanceReference { Instance = initializable };

                _strategy.Activate(_contextMock.Object, reference);

                Assert.Equal(0, initializable.DisposeCount);
                Assert.Same(initializable, reference.Instance);
            }
        }

        [Fact]
        public void Activate_TransparentProxy_NotDisposable()
        {
            using (var server = new RemotingServer())
            using (var client = new RemotingClient())
            {
                server.RegisterActivatedService(typeof(Monk));

                var notDisposable = client.GetService<Monk>();
                var reference = new InstanceReference { Instance = notDisposable };

                _strategy.Activate(_contextMock.Object, reference);

                Assert.Same(notDisposable, reference.Instance);
            }
        }
#endif // !NO_REMOTING


        [Fact]
        public void Deactivate_NotTransparantProxy_Disposable()
        {
            var initializableMock = new Mock<IDisposable>(MockBehavior.Strict);
            var reference = new InstanceReference { Instance = initializableMock.Object };

            initializableMock.Setup(p => p.Dispose());

            _strategy.Deactivate(_contextMock.Object, reference);

            Assert.Same(initializableMock.Object, reference.Instance);
            initializableMock.Verify(p => p.Dispose(), Times.Once);
        }

        [Fact]
        public void Deactivate_NotTransparantProxy_NotDisposable()
        {
            var nonDisposableMock = new Mock<IWarrior>(MockBehavior.Strict);
            var reference = new InstanceReference { Instance = nonDisposableMock.Object };

            _strategy.Deactivate(_contextMock.Object, reference);

            Assert.Same(nonDisposableMock.Object, reference.Instance);
        }

        [Fact]
        public void Deactivate_InstanceIsNull()
        {
            var reference = new InstanceReference();

            _strategy.Deactivate(_contextMock.Object, reference);

            Assert.Null(reference.Instance);
        }

        [Fact]
        public void Deactivate_ReferenceIsNull()
        {
            const InstanceReference reference = null;

            Assert.Throws<NullReferenceException>(() => _strategy.Deactivate(_contextMock.Object, reference));
        }

#if !NO_REMOTING
        [Fact]
        public void Deactivate_TransparentProxy_Disposable()
        {
            using (var server = new RemotingServer())
            using (var client = new RemotingClient())
            {
                server.RegisterActivatedService(typeof(Disposable));

                var initializable = client.GetService<Disposable>();
                var reference = new InstanceReference { Instance = initializable };

                _strategy.Deactivate(_contextMock.Object, reference);

                Assert.Equal(1, initializable.DisposeCount);
                Assert.Same(initializable, reference.Instance);
            }
        }

        [Fact]
        public void Deactivate_TransparentProxy_NotDisposable()
        {
            using (var server = new RemotingServer())
            using (var client = new RemotingClient())
            {
                server.RegisterActivatedService(typeof(Monk));

                var notDisposable = client.GetService<Monk>();
                var reference = new InstanceReference { Instance = notDisposable };

                _strategy.Deactivate(_contextMock.Object, reference);

                Assert.Same(notDisposable, reference.Instance);
            }
        }
#endif // !NO_REMOTING
    }
}