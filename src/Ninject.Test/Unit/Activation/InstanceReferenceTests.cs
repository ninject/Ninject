using Ninject.Activation;
using Ninject.Tests.Fakes;
#if !NO_REMOTING
using Ninject.Tests.Infrastructure;
#endif // !NO_REMOTING
using Xunit;

namespace Ninject.Tests.Unit.Activation
{
    public class InstanceReferenceTests
    {
        [Fact]
        public void IsInstanceOf_NotTransparentProxy_IsAnInstanceOf()
        {
            var instance = new ShortSword();
            var reference = new InstanceReference { Instance = instance };

            Assert.True(reference.IsInstanceOf<ShortSword>(out var shortSword));
            Assert.Same(instance, shortSword);

            Assert.True(reference.IsInstanceOf<Sword>(out var sword));
            Assert.Same(instance, sword);

            Assert.True(reference.IsInstanceOf<IWeapon>(out var weapon));
            Assert.Same(instance, weapon);

            Assert.True(reference.IsInstanceOf<IWeapon>(out var obj));
            Assert.Same(instance, obj);
        }

        [Fact]
        public void IsInstanceOf_NotTransparentProxy_IsNotAnInstanceOf()
        {
            var instance = new Monk();
            var reference = new InstanceReference { Instance = instance };

            Assert.False(reference.IsInstanceOf<IWeapon>(out var weapon));
            Assert.Null(weapon);

            Assert.False(reference.IsInstanceOf<Sword>(out var sword));
            Assert.Null(sword);
        }

#if !NO_REMOTING
        [Fact]
        public void IsInstanceOf_TransparentProxy_IsAnInstanceOf()
        {
            using (var server = new RemotingServer())
            using (var client = new RemotingClient())
            {
                server.RegisterActivatedService(typeof(Monk));

                var instance = client.GetService<Monk>();
                var reference = new InstanceReference { Instance = instance };

                Assert.True(reference.IsInstanceOf<ICleric>(out var cleric));
                Assert.Same(instance, cleric);

                Assert.True(reference.IsInstanceOf<Monk>(out var monk));
                Assert.Same(instance, monk);

                Assert.True(reference.IsInstanceOf<Monk>(out var obj));
                Assert.Same(instance, obj);
            }
        }

        [Fact]
        public void IsInstanceOf_TransparentProxy_IsNotAnInstanceOf()
        {
            using (var server = new RemotingServer())
            using (var client = new RemotingClient())
            {
                server.RegisterActivatedService(typeof(Disposable));

                var instance = client.GetService<Disposable>();
                var reference = new InstanceReference { Instance = instance };

                Assert.False(reference.IsInstanceOf<ICleric>(out var cleric));
                Assert.Null(cleric);

                Assert.False(reference.IsInstanceOf<Monk>(out var monk));
                Assert.Null(monk);
            }
        }
#endif // !NO_REMOTING
    }
}
