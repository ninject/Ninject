using System.Collections.Generic;
using Ninject.Activation;
using Ninject.Parameters;
using Ninject.Planning.Bindings;

namespace Ninject.Tests.Integration.DefaultScopeCallbackTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Ninject.Infrastructure;
    using Xunit;

    public class DefaultScopeContext : IDisposable
    {
        protected StandardKernel kernel;        

        public DefaultScopeContext()
        {
            InitializeKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }

        protected virtual void InitializeKernel()
        {
            this.kernel = new StandardKernel();
        }

        public interface IService { }

        public class ServiceImpl : IService { }
    }

    public class WhenKernelIsCreatedWithDefaults : DefaultScopeContext
    {
        [Fact]
        public void CanOverrideDefaultScopeWithSingletonInBinding()
        {
            kernel.Bind<IService>().To<ServiceImpl>().InSingletonScope();
            var binding = kernel.GetBindings(typeof(IService)).FirstOrDefault();
            binding.ScopeCallback.Should().BeSameAs(StandardScopeCallbacks.Singleton);
        }

        [Fact]
        public void CanOverrideDefaultScopeWithThreadInBinding()
        {
            kernel.Bind<IService>().To<ServiceImpl>().InThreadScope();
            var binding = kernel.GetBindings(typeof(IService)).FirstOrDefault();
            binding.ScopeCallback.Should().BeSameAs(StandardScopeCallbacks.Thread);
        }

        [Fact]
        public void ScopeShouldBeTransient()
        {
            kernel.Settings.DefaultScopeCallback.Should().BeSameAs(StandardScopeCallbacks.Transient);
        }
    }

    public class WhenKernelIsCreatedWithNewObjectScope : DefaultScopeContext
    {
        private Func<IContext, object> scopeDelegate;

        [Fact]
        public void ScopeShouldBeObject()
        {
            this.kernel.Settings.DefaultScopeCallback.Should().BeSameAs(scopeDelegate);
        }

        protected override void InitializeKernel()
        {
            object obj = new object();
            scopeDelegate = ctx => obj;
            var settings = new NinjectSettings
                           {
                               DefaultScopeCallback = scopeDelegate
                           };
            this.kernel = new StandardKernel(settings);
        }
    }

    public class WhenKernelIsCreatedWithTheadScopeAsDefault : DefaultScopeContext
    {
        [Fact]
        public void CanOverrideDefaultScopeWithSingletonInBinding()
        {
            kernel.Bind<IService>().To<ServiceImpl>().InSingletonScope();
            var binding = kernel.GetBindings(typeof(IService)).FirstOrDefault();
            binding.ScopeCallback.Should().BeSameAs(StandardScopeCallbacks.Singleton);
        }

        [Fact]
        public void CanOverrideDefaultScopeWithTransientInBinding()
        {
            kernel.Bind<IService>().To<ServiceImpl>().InTransientScope();
            var binding = kernel.GetBindings(typeof(IService)).FirstOrDefault();
            binding.ScopeCallback.Should().BeSameAs(StandardScopeCallbacks.Transient);
        }

        [Fact]
        public void ScopeShouldBeThread()
        {
            kernel.Settings.DefaultScopeCallback.Should().BeSameAs(StandardScopeCallbacks.Thread);
        }

        protected override void InitializeKernel()
        {
            var settings = new NinjectSettings
                           {
                               DefaultScopeCallback = StandardScopeCallbacks.Thread
                           };
            this.kernel = new StandardKernel(settings);
        }
    }
}
