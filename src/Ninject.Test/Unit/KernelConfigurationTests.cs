using System.Linq;
using Xunit;
using Ninject.Tests.Fakes;
using Ninject.Planning.Strategies;
using Ninject.Activation.Strategies;
using Ninject.Injection;

namespace Ninject.Test.Unit
{
    public class KernelConfigurationTests
    {
        [Fact]
        public void BuildReadOnlyKernelShouldCloneBindings()
        {
            var kernelConfiguration = new KernelConfiguration();
            kernelConfiguration.Bind<IWeapon>().To<Shuriken>();
            kernelConfiguration.Bind<IWeapon>().To<Sword>();
            kernelConfiguration.Bind<ICleric>().To<Monk>();

            var readOnlyKernel = kernelConfiguration.BuildReadOnlyKernel();
            kernelConfiguration.RemoveBinding(kernelConfiguration.GetBindings(typeof(IWeapon)).First());
            kernelConfiguration.Unbind(typeof(ICleric));

            Assert.NotNull(readOnlyKernel);
            Assert.Equal(2, readOnlyKernel.GetBindings(typeof(IWeapon)).Count());
            Assert.Single(readOnlyKernel.GetBindings(typeof(ICleric)));
        }

        [Fact]
        public void MethodInjection_Disabled()
        {
            var settings = new NinjectSettings { MethodInjection = false };
            var kernelConfiguration = new KernelConfiguration(settings);

            var planningStrategies = kernelConfiguration.Components.GetAll<IPlanningStrategy>();
            Assert.Empty(planningStrategies.Where(p => p is MethodReflectionStrategy));

            var activationStrategies = kernelConfiguration.Components.GetAll<IActivationStrategy>();
            Assert.Empty(planningStrategies.Where(p => p is MethodInjectionStrategy));
        }

        [Fact]
        public void MethodInjection_Enabled()
        {
            var settings = new NinjectSettings { MethodInjection = true };
            var kernelConfiguration = new KernelConfiguration(settings);

            var planningStrategies = kernelConfiguration.Components.GetAll<IPlanningStrategy>();
            Assert.NotNull(planningStrategies.SingleOrDefault(p => p is MethodReflectionStrategy));

            var activationStrategies = kernelConfiguration.Components.GetAll<IActivationStrategy>();
            Assert.NotNull(activationStrategies.SingleOrDefault(p => p is MethodInjectionStrategy));
        }

        [Fact]
        public void PropertyInjection_Disabled()
        {
            var settings = new NinjectSettings { PropertyInjection = false };
            var kernelConfiguration = new KernelConfiguration(settings);

            var planningStrategies = kernelConfiguration.Components.GetAll<IPlanningStrategy>();
            Assert.Empty(planningStrategies.Where(p => p is PropertyReflectionStrategy));

            var activationStrategies = kernelConfiguration.Components.GetAll<IActivationStrategy>();
            Assert.Empty(planningStrategies.Where(p => p is PropertyInjectionStrategy));
        }

        [Fact]
        public void PropertyInjection_Enabled()
        {
            var settings = new NinjectSettings { PropertyInjection = true };
            var kernelConfiguration = new KernelConfiguration(settings);

            var planningStrategies = kernelConfiguration.Components.GetAll<IPlanningStrategy>();
            Assert.NotNull(planningStrategies.SingleOrDefault(p => p is PropertyReflectionStrategy));

            var activationStrategies = kernelConfiguration.Components.GetAll<IActivationStrategy>();
            Assert.NotNull(activationStrategies.SingleOrDefault(p => p is PropertyInjectionStrategy));
        }

        [Fact]
        public void UseReflectionBasedInjection_Disabled()
        {
            var settings = new NinjectSettings { UseReflectionBasedInjection = false };
            var kernelConfiguration = new KernelConfiguration(settings);

            var injectorFactories = kernelConfiguration.Components.GetAll<IInjectorFactory>();
            Assert.Single(injectorFactories);
            Assert.Single(injectorFactories, s => s is ExpressionInjectorFactory);
        }

        [Fact]
        public void UseReflectionBasedInjection_Enabled()
        {
            var settings = new NinjectSettings { UseReflectionBasedInjection = true };
            var kernelConfiguration = new KernelConfiguration(settings);

            var injectorFactories = kernelConfiguration.Components.GetAll<IInjectorFactory>();
            Assert.Single(injectorFactories);
            Assert.Single(injectorFactories, s => s is ReflectionInjectorFactory);
        }
    }
}
