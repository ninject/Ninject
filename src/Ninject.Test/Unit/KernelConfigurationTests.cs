using System.Linq;
using Xunit;
using Ninject.Tests.Fakes;

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
    }
}
