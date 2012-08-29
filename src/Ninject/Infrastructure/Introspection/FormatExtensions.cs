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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
#if WINRT
using System.Diagnostics;  
#endif
using Ninject.Activation;
using Ninject.Activation.Providers;
using Ninject.Infrastructure.Language;
using Ninject.Planning.Bindings;
using Ninject.Planning.Targets;
#endregion

namespace Ninject.Infrastructure.Introspection
{
    /// <summary>
    /// Provides extension methods for string formatting
    /// </summary>
    public static class FormatExtensionsEx
    {
    

        /// <summary>
        /// Formats the specified request into a meaningful string representation.
        /// </summary>
        /// <param name="request">The request to be formatted.</param>
        /// <returns>The request formatted as string.</returns>
        public static string Format(this IRequest request)
        {
            using (var sw = new StringWriter())
            {
                if (request.Target == null)
                    sw.Write("Request for {0}", request.Service.Format());
                else
                    sw.Write("Injection of dependency {0} into {1}", request.Service.Format(), request.Target.Format());

                return sw.ToString();
            }
        }

#if WINRT
        private static MemberTypes GetMemberType(this MemberInfo member)
        {
            if (member is FieldInfo)
                return MemberTypes.Field;
            if (member is ConstructorInfo)
                return MemberTypes.Constructor;
            if (member is PropertyInfo)
                return MemberTypes.Property;
            if (member is EventInfo)
                return MemberTypes.Event;
            if (member is MethodInfo)
                return MemberTypes.Method;

            /*
            var typeInfo = member as Type;
            Debug.Assert(typeInfo != null);
            if (!typeInfo.IsPublic && !typeInfo.IsNotPublic)
                return MemberTypes.NestedType;
            */
            return MemberTypes.TypeInfo;
        } 

        private enum MemberTypes
        {
            Field,
            Event,
            Constructor,
            Property,
            Method,
            NestedType,
            TypeInfo

        }

#endif
        /// <summary>
        /// Formats the specified target into a meaningful string representation..
        /// </summary>
        /// <param name="target">The target to be formatted.</param>
        /// <returns>The target formatted as string.</returns>
        public static string Format(this ITarget target)
        {
            using (var sw = new StringWriter())
            {
#if !WINRT
                switch (target.Member.MemberType)
#else
                switch(target.Member.GetMemberType())
#endif
                {
                    case MemberTypes.Constructor:
                        sw.Write("parameter {0} of constructor", target.Name);
                        break;

                    case MemberTypes.Method:
                        sw.Write("parameter {0} of method {1}", target.Name, target.Member.Name);
                        break;

                    case MemberTypes.Property:
                        sw.Write("property {0}", target.Name);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

#if !WINRT
                sw.Write(" of type {0}", target.Member.ReflectedType.Format());
#else
                sw.Write("of type {0}", target.Member.DeclaringType.Format());
#endif
                return sw.ToString();
            }
        }
    }
}