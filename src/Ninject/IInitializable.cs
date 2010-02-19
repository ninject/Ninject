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
#endregion

namespace Ninject
{
	/// <summary>
	/// A service that requires initialization after it is activated.
	/// </summary>
	public interface IInitializable
	{
		/// <summary>
		/// Initializes the instance. Called during activation.
		/// </summary>
		void Initialize();
	}
}
