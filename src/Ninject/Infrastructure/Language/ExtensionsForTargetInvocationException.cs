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

namespace Ninject.Infrastructure.Language
{
    internal static class ExtensionsForTargetInvocationException
    {
        public static void RethrowInnerException(this TargetInvocationException exception)
        {
            var innerException = exception.InnerException;

            var stackTraceField = typeof(Exception).GetTypeInfo().GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
            stackTraceField.SetValue(innerException, innerException.StackTrace);

            throw innerException;
        }
    }
}