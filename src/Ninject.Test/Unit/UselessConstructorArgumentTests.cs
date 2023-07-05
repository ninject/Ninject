namespace Ninject.Tests.Unit
{
    using System;
    using FluentAssertions;
    using Ninject;
    using Xunit;

    public class UselessConstructorArgumentTests
    {
        public UselessConstructorArgumentTests()
        {
        }

        [Fact]
        public void UselessConstructorArgument_FailedTest()
        {
            using (var kernel = new StandardKernel())
            {
                kernel.Settings.CheckForUselessConstructorArgument = true;

                kernel.Bind<PulseLaser>().ToSelf()
                    .WithConstructorArgument("power", 100)
                    .WithConstructorArgument("pulseInterval", TimeSpan.FromSeconds(1))
                    .WithConstructorArgument("unknownArgument", 1)
                    ;

                Action getLaser = () => kernel.Get<PulseLaser>();

                getLaser.ShouldThrow<ActivationException>();
            }
        }

        [Fact]
        public void UselessConstructorArgument_SuccessfulTest()
        {
            using (var kernel = new StandardKernel())
            {
                kernel.Settings.CheckForUselessConstructorArgument = false;

                kernel.Bind<PulseLaser>().ToSelf()
                    .WithConstructorArgument("power", 100)
                    .WithConstructorArgument("pulseInterval", TimeSpan.FromSeconds(1))
                    .WithConstructorArgument("unknownArgument", 1)
                    ;

                var pulseLaser = kernel.Get<PulseLaser>();
                pulseLaser.Should().NotBeNull();
                pulseLaser.Power.Should().Be(100);
                pulseLaser.PulseInterval.Should().Be(TimeSpan.FromSeconds(1));
            }
        }
    }

    public class Laser
    {
        public readonly int Power;

        public Laser(
            int power
            )
        {
            Power = power;
        }
    }

    public sealed class PulseLaser : Laser
    {
        public readonly TimeSpan PulseInterval;

        public PulseLaser(int power, TimeSpan pulseInterval)
            : base(power)
        {
            PulseInterval = pulseInterval;
        }
    }

}
