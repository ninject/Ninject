#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion

using System.Collections.Generic;
using System.Linq;

namespace Ninject.Infrastructure.Language
{
    using System;
    using System.Reflection;
    
#if WINRT
    using Ninject.Planning.Targets;
#endif

    internal static class ExtensionsForICustomAttributeProvider
    {
#if !NETSTANDARD1_3

        public static bool HasAttribute(this ICustomAttributeProvider member, Type type)
        {
            var memberInfo = member as MemberInfo;
            if (memberInfo != null)
            {
                return memberInfo.HasAttribute(type);
            }

            return member.IsDefined(type, true);
        }




        public static IEnumerable<Attribute> GetCustomAttributesExtended(this ICustomAttributeProvider member, Type attributeType, bool inherit)
        {
            var memberInfo = member as MemberInfo;
            if (memberInfo != null)
            {
                return memberInfo.GetCustomAttributesExtended(attributeType, inherit);
            }

            return member.GetCustomAttributes(attributeType, inherit).Cast<Attribute>();
        }

#endif
        public static bool HasAttribute(object target, Type type)
        {
            var memberInfo = target as MemberInfo;
            if (memberInfo != null)
            {
                return memberInfo.HasAttribute(type);
            }

            return ((ParameterInfo)target).IsDefined(type, true);
        }



        public static IEnumerable<Attribute> GetCustomAttributesExtended(object target, Type attributeType, bool inherit)
        {
            var memberInfo = target as MemberInfo;
            if (memberInfo != null)
            {
                return memberInfo.GetCustomAttributesExtended(attributeType, inherit);
            }

            return ((ParameterInfo)target).GetCustomAttributes(attributeType, inherit)
#if !NETSTANDARD1_3
                .Cast<Attribute>()
#endif
                ;
        }


        public static bool IsDefined(object target, Type attributeType, bool inherit)
        {
            var memberInfo = target as MemberInfo;
            if (memberInfo != null)
            {
                return memberInfo.IsDefined(attributeType, inherit);
            }

            return ((ParameterInfo)target).IsDefined(attributeType, inherit);
        }

        public static IEnumerable<Attribute> GetCustomAttributes(object target, bool inherit)
        {
            var memberInfo = target as MemberInfo;
            if (memberInfo != null)
            {
                return memberInfo.GetCustomAttributes(inherit)
#if !NETSTANDARD1_3
                .Cast<Attribute>()
#endif
                    ;
            }

            return ((ParameterInfo)target).GetCustomAttributes(inherit)
#if !NETSTANDARD1_3
                .Cast<Attribute>()
#endif
                ;
        }

    }
}