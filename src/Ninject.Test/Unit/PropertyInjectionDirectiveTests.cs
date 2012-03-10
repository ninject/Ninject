using System;
using Ninject.Injection;
using Ninject.Planning.Directives;
using Xunit;

#if WINRT
using System.Reflection;
#endif

namespace Ninject.Tests.Unit.PropertyInjectionDirectiveTests
{
    using FluentAssertions;

    public class PropertyInjectionDirectiveContext
    {
        protected PropertyInjectionDirective directive;
    }

#if MSTEST
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
#endif
    public class WhenDirectiveIsCreated : PropertyInjectionDirectiveContext
    {
#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void CreatesTargetForProperty()
        {
#if !WINRT
            var method = typeof(Dummy).GetProperty("Foo");
#else
            var method = typeof (Dummy).GetTypeInfo().GetDeclaredProperty("Foo");
#endif
            PropertyInjector injector = delegate { };

            directive = new PropertyInjectionDirective(method, injector);

            directive.Target.Name.Should().Be("Foo");
            directive.Target.Type.Should().Be(typeof(int));
        }
    }

    public class Dummy
    {
        public int Foo { get; set; }
    }
}