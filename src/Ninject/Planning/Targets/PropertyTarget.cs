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
#endregion

namespace Ninject.Planning.Targets
{
	/// <summary>
	/// Represents an injection target for a <see cref="PropertyInfo"/>.
	/// </summary>
	public class PropertyTarget : Target<PropertyInfo>
	{
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
			get { return Site.PropertyType; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyTarget"/> class.
		/// </summary>
		/// <param name="site">The property that this target represents.</param>
		public PropertyTarget(PropertyInfo site) : base(site, site) { }
	}
}