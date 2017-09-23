// -------------------------------------------------------------------------------------------------
// <copyright file="PropertyTarget.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Planning.Targets
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Represents an injection target for a <see cref="PropertyInfo"/>.
    /// </summary>
    public class PropertyTarget : Target<PropertyInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyTarget"/> class.
        /// </summary>
        /// <param name="site">The property that this target represents.</param>
        public PropertyTarget(PropertyInfo site)
            : base(site, site)
        {
        }

        /// <summary>
        /// Gets the name of the target.
        /// </summary>
        public override string Name
        {
            get { return this.Site.Name; }
        }

        /// <summary>
        /// Gets the type of the target.
        /// </summary>
        public override Type Type
        {
            get { return this.Site.PropertyType; }
        }
    }
}