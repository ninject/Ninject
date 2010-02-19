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
	/// A service that is started when activated, and stopped when deactivated.
	/// </summary>
	public interface IStartable
	{
		/// <summary>
		/// Starts this instance. Called during activation.
		/// </summary>
		void Start();

		/// <summary>
		/// Stops this instance. Called during deactivation.
		/// </summary>
		void Stop();
	}
}
