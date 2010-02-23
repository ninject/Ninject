#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion

namespace Ninject.Injection
{
	/// <summary>
	/// A delegate that can inject values into a constructor.
	/// </summary>
	public delegate object ConstructorInjector(params object[] arguments);
}