#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion
#if !SILVERLIGHT
#region Using Directives
using System;
using System.Collections.Generic;
using Ninject.Components;
#endregion

namespace Ninject.Modules
{
    /// <summary>
    /// Finds modules defined in external files.
    /// </summary>
    public interface IModuleLoader : INinjectComponent
    {
#if !PCL // PCL can't contain this method as the signature differs
        /// <summary>
        /// Loads any modules found in the files that match the specified patterns.
        /// </summary>
        /// <param name="patterns">The patterns to search.</param>

#if !WINRT
        void 
#else
        System.Threading.Tasks.Task
#endif
        LoadModules(IEnumerable<string> patterns);
#endif
    }

}
#endif