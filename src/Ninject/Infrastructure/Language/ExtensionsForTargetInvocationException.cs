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
#if WINRT
using System.Runtime.ExceptionServices;
#endif
#endregion

namespace Ninject.Infrastructure.Language
{
    internal static class ExtensionsForTargetInvocationException
    {
        public static void RethrowInnerException(this TargetInvocationException exception)
        {
            Exception innerException = exception.InnerException;

#if !WINRT
            FieldInfo stackTraceField = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
            stackTraceField.SetValue(innerException, innerException.StackTrace);
#else
            ExceptionDispatchInfo.Capture(innerException).Throw();
#endif

            throw innerException;
        }
    }
}