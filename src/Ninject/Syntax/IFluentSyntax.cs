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
using System.ComponentModel;
#endregion

namespace Ninject.Syntax
{
    /// <summary>
    /// A hack to hide methods defined on <see cref="object"/> for IntelliSense
    /// on fluent interfaces. Credit to Daniel Cazzulino.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IFluentSyntax
    {
        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)] Type GetType();

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)] int GetHashCode();

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)] string ToString();

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)] bool Equals(object other);
    }
}