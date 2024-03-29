﻿namespace Ninject.Tests.Unit.PropertyInjectionDirectiveTests
{
    using System;
    using System.Reflection;
    using Ninject.Injection;
    using Ninject.Planning.Directives;
    using Xunit;
    using FluentAssertions;

    public class PropertyInjectionDirectiveContext
    {
        protected PropertyInjectionDirective directive;
    }

    public class WhenDirectiveIsCreated : PropertyInjectionDirectiveContext
    {
        [Fact]
        public void CreatesTargetForProperty()
        {
            var method = typeof(Dummy).GetProperty("Foo");

            PropertyInjector injector = delegate { };

            this.directive = new PropertyInjectionDirective(method, injector);

            this.directive.Target.Name.Should().Be("Foo");
            this.directive.Target.Type.Should().Be(typeof(int));
        }
    }

    public class Dummy
    {
        public int Foo { get; set; }
    }
}