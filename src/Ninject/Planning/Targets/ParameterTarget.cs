#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion
#region Using Directives
using System;
using System.Reflection;
using Ninject.Infrastructure;
#endregion

namespace Ninject.Planning.Targets
{
    /// <summary>
    /// Represents an injection target for a <see cref="ParameterInfo"/>.
    /// </summary>
    public class ParameterTarget : Target<ParameterInfo>
    {
        private readonly Future<object> defaultValue;

        /// <summary>
        /// Gets the name of the target.
        /// </summary>
        public override string Name
        {
            get { return Site.Name; }
        }

        /// <summary>
        /// Gets the type of the target.
        /// </summary>
        public override Type Type
        {
            get { return Site.ParameterType; }
        }

// Windows Phone doesn't support default values and returns null instead of DBNull.
#if !WINDOWS_PHONE
        /// <summary>
        /// Gets a value indicating whether the target has a default value.
        /// </summary>
        public override bool HasDefaultValue
        {
            get { return defaultValue.Value != DBNull.Value; }
        }

        /// <summary>
        /// Gets the default value for the target.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">If the item does not have a default value.</exception>
        public override object DefaultValue
        {
            get { return HasDefaultValue ? defaultValue.Value : base.DefaultValue; }
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterTarget"/> class.
        /// </summary>
        /// <param name="method">The method that defines the parameter.</param>
        /// <param name="site">The parameter that this target represents.</param>
        public ParameterTarget(MethodBase method, ParameterInfo site) : base(method, site)
        {
            defaultValue = new Future<object>(() => site.DefaultValue);
        }
    }
}