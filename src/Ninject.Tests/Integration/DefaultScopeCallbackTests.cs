﻿namespace Ninject.Tests.Integration.DefaultScopeCallbackTests
{
    using System.Collections.Generic;
    using Ninject.Activation;
    using Ninject.Parameters;
    using Ninject.Planning.Bindings;
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

        protected virtual void TestSelfBindedTypesAreTransient()
        {
            var firstInstance = this.kernel.Get<SelfBindedType>();
            var secondInstance = this.kernel.Get<SelfBindedType>();
            firstInstance.Should().NotBeSameAs(secondInstance, "because types are transient");
        }

        public interface IService { }

        public class ServiceImpl : IService { }

        public class SelfBindedType
        {
            public override string ToString()
            {
                return "SelfBindedType";
            }
        }
    }

    public class WhenKernelIsCreatedWithDefaults : DefaultScopeContext
    {
        [Fact]
        public void CanOverrideDefaultScopeWithSingletonInBinding()
        {
            this.kernel.Bind<IService>().To<ServiceImpl>().InSingletonScope();
            var binding = this.kernel.GetBindings(typeof(IService)).FirstOrDefault();
            binding.ScopeCallback.Should().BeSameAs(StandardScopeCallbacks.Singleton);
        }

        [Fact]
        public void CanOverrideDefaultScopeWithThreadInBinding()
        {
            this.kernel.Bind<IService>().To<ServiceImpl>().InThreadScope();
            var binding = this.kernel.GetBindings(typeof(IService)).FirstOrDefault();
            binding.ScopeCallback.Should().BeSameAs(StandardScopeCallbacks.Thread);
        }

        [Fact]
        public void ScopeShouldBeTransient()
        {
            this.kernel.Settings.DefaultScopeCallback.Should().BeSameAs(StandardScopeCallbacks.Transient);
        }

        [Fact]
        public void ImplicitSelfBindedTypeShouldBeTransient()
        {
            TestSelfBindedTypesAreTransient();
        }

        [Fact]
        public void ExplicitSelfBindedTypeShouldBeTransient()
        {
            this.kernel.Bind<SelfBindedType>().ToSelf();
            var binding = this.kernel.GetBindings(typeof(SelfBindedType)).FirstOrDefault();
            binding.ScopeCallback.Should().BeSameAs(StandardScopeCallbacks.Transient);
        }
    }

    public class WhenKernelIsCreatedWithNewObjectScope : DefaultScopeContext
    {
        private Func<IContext, object> scopeDelegate;

        [Fact]
        public void ScopeShouldBeObject()
        {
            this.kernel.Settings.DefaultScopeCallback.Should().BeSameAs(this.scopeDelegate);
        }

        [Fact]
        public void ImplicitSelfBindedTypeShouldBeTransient()
        {
            TestSelfBindedTypesAreTransient();
        }

        [Fact]
        public void ExplicitSelfBindedTypeShouldHaveObjectScope()
        {
            this.kernel.Bind<SelfBindedType>().ToSelf();
            var binding = this.kernel.GetBindings(typeof(SelfBindedType)).FirstOrDefault();
            binding.ScopeCallback.Should().BeSameAs(this.scopeDelegate);
        }

        protected override void InitializeKernel()
        {
            object obj = new object();
            this.scopeDelegate = ctx => obj;
            var settings = new NinjectSettings
                           {
                               DefaultScopeCallback = this.scopeDelegate
                           };
            this.kernel = new StandardKernel(settings);
        }
    }

    public class WhenKernelIsCreatedWithThreadScopeAsDefault : DefaultScopeContext
    {
        [Fact]
        public void CanOverrideDefaultScopeWithSingletonInBinding()
        {
            this.kernel.Bind<IService>().To<ServiceImpl>().InSingletonScope();
            var binding = this.kernel.GetBindings(typeof(IService)).FirstOrDefault();
            binding.ScopeCallback.Should().BeSameAs(StandardScopeCallbacks.Singleton);
        }

        [Fact]
        public void CanOverrideDefaultScopeWithTransientInBinding()
        {
            this.kernel.Bind<IService>().To<ServiceImpl>().InTransientScope();
            var binding = this.kernel.GetBindings(typeof(IService)).FirstOrDefault();
            binding.ScopeCallback.Should().BeSameAs(StandardScopeCallbacks.Transient);
        }

        [Fact]
        public void ScopeShouldBeThread()
        {
            this.kernel.Settings.DefaultScopeCallback.Should().BeSameAs(StandardScopeCallbacks.Thread);
        }

        [Fact]
        public void ImplicitSelfBindedTypeShouldBeTransient()
        {
            TestSelfBindedTypesAreTransient();
        }

        [Fact]
        public void ExplicitSelfBindedTypeShouldHaveThreadScope()
        {
            this.kernel.Bind<SelfBindedType>().ToSelf();
            var binding = this.kernel.GetBindings(typeof(SelfBindedType)).FirstOrDefault();
            binding.ScopeCallback.Should().BeSameAs(StandardScopeCallbacks.Thread);
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
