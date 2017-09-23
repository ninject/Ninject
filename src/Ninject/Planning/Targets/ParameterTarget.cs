// -------------------------------------------------------------------------------------------------
// <copyright file="ParameterTarget.cs" company="Ninject Project Contributors">
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
    /// Represents an injection target for a <see cref="ParameterInfo"/>.
    /// </summary>
    public class ParameterTarget : Target<ParameterInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterTarget"/> class.
        /// </summary>
        /// <param name="method">The method that defines the parameter.</param>
        /// <param name="site">The parameter that this target represents.</param>
        public ParameterTarget(MethodBase method, ParameterInfo site)
            : base(method, site)
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
            get { return this.Site.ParameterType; }
        }

        /// <summary>
        /// Gets a value indicating whether the target has a default value.
        /// </summary>
        public override bool HasDefaultValue
        {
            get { return this.Site.HasDefaultValue; }
        }

        /// <summary>
        /// Gets the default value for the target.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the item does not have a default value.</exception>
        public override object DefaultValue
        {
            get { return this.HasDefaultValue ? this.Site.DefaultValue : base.DefaultValue; }
        }
    }
}