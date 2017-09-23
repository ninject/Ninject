// -------------------------------------------------------------------------------------------------
// <copyright file="WeakPropertyValue.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Parameters
{
    using System;

    /// <summary>
    /// Overrides the injected value of a property.
    /// Keeps a weak reference to the value.
    /// </summary>
    public class WeakPropertyValue : Parameter, IPropertyValue
    {
        private readonly WeakReference weakReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakPropertyValue"/> class.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="value">The value to inject into the property.</param>
        public WeakPropertyValue(string name, object value)
            : base(name, (object)null, false)
        {
            this.weakReference = new WeakReference(value);
            this.ValueCallback = (ctx, target) => this.weakReference.Target;
        }
    }
}