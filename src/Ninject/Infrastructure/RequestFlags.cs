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
#if !NO_WEB
using System.Web;
#endif
using Ninject.Activation;
#endregion

namespace Ninject.Infrastructure
{
	/// <summary>
	/// Defines the style of request (single or multi-injection, whether it is optional, etc.)
	/// </summary>
	[Flags]
	public enum RequestFlags
	{
		/// <summary>
		/// Indicates a request for a single instance of a service.
		/// </summary>
		Single = 0,

		/// <summary>
		/// Indicates a request for multiple instances of a service.
		/// </summary>
		Multiple = 1,

		/// <summary>
		/// Indicates that null should be returned (instead of throwing) if the service cannot be resolved.
		/// </summary>
		Optional = 2
	}
}