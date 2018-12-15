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
    public class StartableStrategyTests
    {
        private Mock<IContext> _contextMock;
        private StartableStrategy _strategy;

        public StartableStrategyTests()
        {
            _contextMock = new Mock<IContext>(MockBehavior.Strict);

            _strategy = new StartableStrategy();
        }

        [Fact]
        public void Activate_NotTransparantProxy_Startable()
        {
            var startableMock = new Mock<IStartable>(MockBehavior.Strict);
            var reference = new InstanceReference { Instance = startableMock.Object };

            startableMock.Setup(p => p.Start());

            _strategy.Activate(_contextMock.Object, reference);

            Assert.Same(startableMock.Object, reference.Instance);
            startableMock.Verify(p => p.Start(), Times.Once);
        }

        [Fact]
        public void Activate_NotTransparantProxy_NotStartable()
        {
            var nonStartableMock = new Mock<IWarrior>(MockBehavior.Strict);
            var reference = new InstanceReference { Instance = nonStartableMock.Object };

            _strategy.Activate(_contextMock.Object, reference);

            Assert.Same(nonStartableMock.Object, reference.Instance);
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

            Assert.Throws<NullReferenceException>(() => _strategy.Activate(_contextMock.Object, reference));
        }

#if !NO_REMOTING
        [Fact]
        public void Activate_TransparentProxy_Startable()
        {
            using (var server = new RemotingServer())
            using (var client = new RemotingClient())
            {
                server.RegisterActivatedService(typeof(Startable));

                var startable = client.GetService<Startable>();
                var reference = new InstanceReference { Instance = startable };

                _strategy.Activate(_contextMock.Object, reference);

                Assert.Equal(1, startable.StartCount);
                Assert.Equal(0, startable.StopCount);
                Assert.Same(startable, reference.Instance);
            }
        }

        [Fact]
        public void Activate_TransparentProxy_NotStartable()
        {
            using (var server = new RemotingServer())
            using (var client = new RemotingClient())
            {
                server.RegisterActivatedService(typeof(Monk));

                var notStartable = client.GetService<Monk>();
                var reference = new InstanceReference { Instance = notStartable };

                _strategy.Activate(_contextMock.Object, reference);

                Assert.Same(notStartable, reference.Instance);
            }
        }
#endif // !NO_REMOTING

        [Fact]
        public void Deactivate_NotTransparantProxy_Startable()
        {
            var startableMock = new Mock<IStartable>(MockBehavior.Strict);
            var reference = new InstanceReference { Instance = startableMock.Object };

            startableMock.Setup(p => p.Stop());

            _strategy.Deactivate(_contextMock.Object, reference);

            Assert.Same(startableMock.Object, reference.Instance);
            startableMock.Verify(p => p.Stop(), Times.Once);
        }

        [Fact]
        public void Deactivate_NotTransparantProxy_NotStartable()
        {
            var nonStartableMock = new Mock<IWarrior>(MockBehavior.Strict);
            var reference = new InstanceReference { Instance = nonStartableMock.Object };

            _strategy.Deactivate(_contextMock.Object, reference);

            Assert.Same(nonStartableMock.Object, reference.Instance);
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
        public void Deactivate_TransparentProxy_Startable()
        {
            using (var server = new RemotingServer())
            using (var client = new RemotingClient())
            {
                server.RegisterActivatedService(typeof(Startable));

                var startable = client.GetService<Startable>();
                var reference = new InstanceReference { Instance = startable };

                _strategy.Deactivate(_contextMock.Object, reference);

                Assert.Equal(0, startable.StartCount);
                Assert.Equal(1, startable.StopCount);
                Assert.Same(startable, reference.Instance);
            }
        }

        [Fact]
        public void Deactivate_TransparentProxy_NotStartable()
        {
            using (var server = new RemotingServer())
            using (var client = new RemotingClient())
            {
                server.RegisterActivatedService(typeof(Monk));

                var notStartable = client.GetService<Monk>();
                var reference = new InstanceReference { Instance = notStartable };

                _strategy.Deactivate(_contextMock.Object, reference);

                Assert.Same(notStartable, reference.Instance);
            }
        }
#endif // !NO_REMOTING
    }
}