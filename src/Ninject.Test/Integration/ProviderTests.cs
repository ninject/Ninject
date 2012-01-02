namespace Ninject.Tests.Integration
{
    using System;
    using FluentAssertions;
    using Ninject.Activation;
    using Xunit;

    public class ProviderTests : IDisposable
    {
        private readonly IKernel kernel;

        public ProviderTests()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }

        [Fact]
        public void InstancesCanBeCreated()
        {
            this.kernel.Bind<IConfig>().ToProvider<ConfigProvider>();

            var instance = this.kernel.Get<IConfig>();

            instance.Should().NotBeNull();
        }

        public class ConfigProvider : IProvider
        {
            public Type Type
            {
                get
                {
                    return typeof(DynamicConfigReader);
                }
            }
            
            public object Create(IContext context)
            {
                return new DynamicConfigReader("test");
            }
        }

        public interface IConfig
        {
            string Get();
        }

        public class DynamicConfigReader : IConfig
        {
            private readonly string name;

            public DynamicConfigReader(string name)
            {
                this.name = name;
            }

            public string Get()
            {
                return this.name;
            }
        }
    }
}