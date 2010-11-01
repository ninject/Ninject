namespace Ninject.Tests.MSTestAttributes
{
    using System;

    /// <summary>
    /// Fake of the MSTest TestClass Attribute to reuse Tests for Silverlight.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]   
    public class TestClassAttribute : Attribute
    {
    }
}
