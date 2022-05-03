namespace Ninject.Tests.Unit
{
    using Ninject.Activation;
    using Ninject.Infrastructure;
    using Ninject.Tests.Fakes;
    using System;
    using Xunit;

    public class NinjectSettingsTests
    {
        private NinjectSettings _settings;

        public NinjectSettingsTests()
        {
            _settings = new NinjectSettings();
        }

        [Fact]
        public void InjectAttribute()
        {
            Assert.Equal(typeof(InjectAttribute), _settings.InjectAttribute);
            _settings.InjectAttribute = typeof(Monk);
            Assert.Equal(typeof(Monk), _settings.InjectAttribute);
            _settings.InjectAttribute = null;
            Assert.Null(_settings.InjectAttribute);
        }

        [Fact]
        public void CachePruningInterval()
        {
            Assert.Equal(TimeSpan.FromSeconds(30), _settings.CachePruningInterval);
            _settings.CachePruningInterval = TimeSpan.FromMinutes(1);
            Assert.Equal(TimeSpan.FromMinutes(1), _settings.CachePruningInterval);
        }

        [Fact]
        public void DefaultScopeCallback()
        {
            Func<IContext, object> myCallback = (context) => new object();

            Assert.Same(StandardScopeCallbacks.Transient, _settings.DefaultScopeCallback);
            _settings.DefaultScopeCallback = myCallback;
            Assert.Same(myCallback, _settings.DefaultScopeCallback);
            _settings.DefaultScopeCallback = null;
            Assert.Null(_settings.DefaultScopeCallback);
        }

        [Fact]
        public void LoadExtensions()
        {
            Assert.True(_settings.LoadExtensions);
            _settings.LoadExtensions = false;
            Assert.False(_settings.LoadExtensions);
            _settings.LoadExtensions = true;
            Assert.True(_settings.LoadExtensions);
        }

        [Fact]
        public void ExtensionSearchPatterns()
        {
            Assert.Equal(new[] { "Ninject.Extensions.*.dll", "Ninject.Web*.dll" }, _settings.ExtensionSearchPatterns);
            _settings.ExtensionSearchPatterns = new[] { "Ninject.*.dll" };
            Assert.Equal(new[] { "Ninject.*.dll" }, _settings.ExtensionSearchPatterns);
            _settings.ExtensionSearchPatterns = null;
            Assert.Null(_settings.ExtensionSearchPatterns);
        }

        [Fact]
        public void UseReflectionBasedInjection()
        {
            Assert.False(_settings.UseReflectionBasedInjection);
            _settings.UseReflectionBasedInjection = true;
            Assert.True(_settings.UseReflectionBasedInjection);
            _settings.UseReflectionBasedInjection = false;
            Assert.False(_settings.UseReflectionBasedInjection);
        }

        [Fact]
        public void InjectNonPublic()
        {
            Assert.False(_settings.InjectNonPublic);
            _settings.InjectNonPublic = true;
            Assert.True(_settings.InjectNonPublic);
            _settings.InjectNonPublic = false;
            Assert.False(_settings.InjectNonPublic);
        }

        [Fact]
        public void InjectParentPrivateProperties()
        {
            Assert.False(_settings.InjectParentPrivateProperties);
            _settings.InjectParentPrivateProperties = true;
            Assert.True(_settings.InjectParentPrivateProperties);
            _settings.InjectParentPrivateProperties = false;
            Assert.False(_settings.InjectParentPrivateProperties);
        }

        [Fact]
        public void ActivationCacheDisabled()
        {
            Assert.False(_settings.ActivationCacheDisabled);
            _settings.ActivationCacheDisabled = true;
            Assert.True(_settings.ActivationCacheDisabled);
            _settings.ActivationCacheDisabled = false;
            Assert.False(_settings.ActivationCacheDisabled);
        }

        [Fact]
        public void AllowNullInjection()
        {
            Assert.False(_settings.AllowNullInjection);
            _settings.AllowNullInjection = true;
            Assert.True(_settings.AllowNullInjection);
            _settings.AllowNullInjection = false;
            Assert.False(_settings.AllowNullInjection);
        }

        [Fact]
        public void MethodInjection()
        {
            Assert.True(_settings.MethodInjection);
            _settings.MethodInjection = false;
            Assert.False(_settings.MethodInjection);
            _settings.MethodInjection = true;
            Assert.True(_settings.MethodInjection);
        }

        [Fact]
        public void PropertyInjection()
        {
            Assert.True(_settings.PropertyInjection);
            _settings.PropertyInjection = false;
            Assert.False(_settings.PropertyInjection);
            _settings.PropertyInjection = true;
            Assert.True(_settings.PropertyInjection);
        }
    }
}
