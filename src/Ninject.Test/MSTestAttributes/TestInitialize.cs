namespace Ninject.Tests.MSTestAttributes
{
    using System;

    /// <summary>
    /// Fake of the MSTest TestInitialize Attribute to reuse Tests for Silverlight.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TestInitializeAttribute : Attribute
    {
    }
}