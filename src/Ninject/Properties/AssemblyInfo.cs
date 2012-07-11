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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
#endregion

[assembly: AssemblyTitle("Ninject Core Library")]

#if !NO_PARTIAL_TRUST
[assembly: AllowPartiallyTrustedCallers]
#endif
[assembly: AssemblyDescriptionAttribute("IoC container")]
