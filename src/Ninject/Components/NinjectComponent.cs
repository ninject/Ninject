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
using Ninject.Infrastructure.Disposal;
#endregion

namespace Ninject.Components
{
	/// <summary>
	/// A component that contributes to the internals of Ninject.
	/// </summary>
	public abstract class NinjectComponent : DisposableObject, INinjectComponent
	{
		/// <summary>
		/// Gets or sets the settings.
		/// </summary>
		public INinjectSettings Settings { get; set; }
	}
}