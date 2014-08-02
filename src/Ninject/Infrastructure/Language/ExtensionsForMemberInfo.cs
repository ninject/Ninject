#region License
//
// Author: Remo Gloor (remo.gloor@bbv.ch)
// Copyright (c) 2010, bbv Software Engineering AG.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion

namespace Ninject.Infrastructure.Language
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    

    /// <summary>
    /// Extensions for MemberInfo
    /// </summary>
    public static class ExtensionsForMemberInfo
    {
#if !WINRT
        const BindingFlags DefaultFlags = BindingFlags.Public | BindingFlags.Instance;
#if !NO_LCG && !SILVERLIGHT || __IOS__
        const BindingFlags Flags = DefaultFlags | BindingFlags.NonPublic;
#else
        const BindingFlags Flags = DefaultFlags;
#endif
#endif

#if !MONO && !WINRT
        private static MethodInfo parentDefinitionMethodInfo;

        private static MethodInfo ParentDefinitionMethodInfo
        {
            get
            {
                if (parentDefinitionMethodInfo == null)
                {
                    var runtimeAssemblyInfoType = typeof(MethodInfo).Assembly.GetType("System.Reflection.RuntimeMethodInfo");
                    parentDefinitionMethodInfo = runtimeAssemblyInfoType.GetMethod("GetParentDefinition", Flags);

                }

                return parentDefinitionMethodInfo;
            }
        }
#endif

        /// <summary>
        /// Determines whether the specified member has attribute.
        /// </summary>
        /// <typeparam name="T">The type of the attribute.</typeparam>
        /// <param name="member">The member.</param>
        /// <returns>
        /// 	<c>true</c> if the specified member has attribute; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAttribute<T>(this MemberInfo member)
        {
            return member.HasAttribute(typeof(T));
        }

        /// <summary>
        /// Determines whether the specified member has attribute.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="type">The type of the attribute.</param>
        /// <returns>
        /// 	<c>true</c> if the specified member has attribute; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAttribute(this MemberInfo member, Type type)
        {
            var propertyInfo = member as PropertyInfo;
            if (propertyInfo != null)
            {
                return IsDefined(propertyInfo, type, true);
            }
#if NETCF
            // Workaround for the CF bug that derived generic methods throw an exception for IsDefined
            // This means that the Inject attribute can not be defined on base methods for CF framework
            var methodInfo = member as MethodInfo;
            if (methodInfo != null)
            {
                return methodInfo.IsDefined(type, false);
            }
#endif
            return member.IsDefined(type, true);
        }

        /// <summary>
        /// Gets the property info from its declared tpe.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <param name="propertyDefinition">The property definition.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>The property info from the declared type of the property.</returns>
        public static PropertyInfo GetPropertyFromDeclaredType(
            this MemberInfo memberInfo,
            PropertyInfo propertyDefinition
#if !WINRT
            ,BindingFlags flags
#endif
            
            )
        {
#if PCL
            throw new NotImplementedException();
#else

#if WINRT
            return memberInfo.DeclaringType.GetRuntimeProperties().FirstOrDefault(
                p => p.Name == propertyDefinition.Name &&
                    !p.GetMethod.IsStatic && 
                     p.PropertyType == propertyDefinition.PropertyType &&
                     p.GetIndexParameters().SequenceEqual(propertyDefinition.GetIndexParameters(), new ParameterInfoEqualityComparer())
                );
#else
            return memberInfo.DeclaringType.GetProperty(
                propertyDefinition.Name,
                flags,
                null,
                propertyDefinition.PropertyType,
                propertyDefinition.GetIndexParameters().Select(parameter => parameter.ParameterType).ToArray(),
                null);
#endif
#endif
        }

#if WINRT
        private class ParameterInfoEqualityComparer : IEqualityComparer<ParameterInfo>
        {
            public bool Equals(ParameterInfo x, ParameterInfo y)
            {
                return x.Position == y.Position && x.ParameterType == y.ParameterType;
            }

            public int GetHashCode(ParameterInfo obj)
            {
                return obj.Position.GetHashCode() ^ obj.ParameterType.GetHashCode();
            }
        }
#endif

        /// <summary>
        /// Determines whether the specified property info is private.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns>
        /// 	<c>true</c> if the specified property info is private; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPrivate(this PropertyInfo propertyInfo)
        {
#if !WINRT
            var getMethod = propertyInfo.GetGetMethod(true);
            var setMethod = propertyInfo.GetSetMethod(true);
#else
            var getMethod = propertyInfo.GetMethod;
            var setMethod = propertyInfo.SetMethod;
#endif
            return (getMethod == null || getMethod.IsPrivate) && (setMethod == null || setMethod.IsPrivate);
        }


        /// <summary>
        /// Gets the custom attributes.
        /// This version is able to get custom attributes for properties from base types even if the property is none public.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <param name="inherited">if set to <c>true</c> [inherited].</param>
        /// <returns></returns>
        public static 
#if !WINRT
            object[] 
#else
            IEnumerable<Attribute>
#endif
            GetCustomAttributesExtended(this MemberInfo member, Type attributeType, bool inherited)
        {
#if !NET_35 && !MONO_40 && !WINRT
            return Attribute.GetCustomAttributes(member, attributeType, inherited);
#else
            var propertyInfo = member as PropertyInfo;
            if (propertyInfo != null)
            {
                return GetCustomAttributes(propertyInfo, attributeType, inherited);
            }

            return member.GetCustomAttributes(attributeType, inherited);
#endif
        }


        private static PropertyInfo GetParentDefinition(PropertyInfo property)
        {

#if !WINRT
            var propertyMethod = property.GetGetMethod(true) ?? property.GetSetMethod(true);
#else
            var propertyMethod = property.GetMethod ?? property.SetMethod;
#endif

            if (propertyMethod != null)
            {
                propertyMethod = propertyMethod.GetParentDefinition(
#if !WINRT
                    Flags
#endif
                    );
                if (propertyMethod != null)
                {
                    return propertyMethod.GetPropertyFromDeclaredType(property
#if !WINRT
                        , Flags
#endif
                        );
                }
            }

            return null;
        }

        private static MethodInfo GetParentDefinition(this MethodInfo method
#if !WINRT
            , BindingFlags flags
#endif
            )
        {
#if PCL
            throw new NotImplementedException();
#else
#if MEDIUM_TRUST || MONO
            
#if !WINRT
            var baseDefinition = method.GetBaseDefinition(); 
#else
            var baseDefinition = method.GetRuntimeBaseDefinition();
#endif
            var type = method.DeclaringType
#if WINRT
                .GetTypeInfo()
#endif
                .BaseType;

            MethodInfo result = null;
            while (result == null && type != null)
            {
#if !WINRT
                result = type.GetMethods(flags).Where(m => m.GetBaseDefinition().Equals(baseDefinition)).SingleOrDefault();
                type = type.BaseType;
#else
                result = type.GetRuntimeMethods().Where(m => !m.IsStatic && m.GetRuntimeBaseDefinition().Equals(baseDefinition)).SingleOrDefault();
                type = type.GetTypeInfo().BaseType;
#endif
            }

            return result;
#else
            if (ParentDefinitionMethodInfo == null)
            {
                return null;
            }

            return (MethodInfo)ParentDefinitionMethodInfo.Invoke(method, flags, null, null, CultureInfo.InvariantCulture);
#endif
#endif
        }


        private static bool IsDefined(PropertyInfo element, Type attributeType, bool inherit)
        {
            if (element.IsDefined(attributeType, false))
            {
                return true;
            }

            if (inherit)
            {
                if (!InternalGetAttributeUsage(attributeType).Inherited)
                {
                    return false;
                }

                for (var info = GetParentDefinition(element);
                     info != null;
                     info = GetParentDefinition(info))
                {
                    if (info.IsDefined(attributeType, false))
                    {
                        return true;
                    }
                }
            }


            return false;
        }

        private static
#if !WINRT
            object[] 
#else
 IEnumerable<Attribute>
#endif
            GetCustomAttributes(PropertyInfo propertyInfo, Type attributeType, bool inherit)
        {
            if (inherit)
            {
                if (InternalGetAttributeUsage(attributeType).Inherited)
                {
#if !WINRT
                    var attributes = new List<object>();
#else
                    var attributes = new List<Attribute>();
#endif
                    var attributeUsages = new Dictionary<Type, bool>();
                    attributes.AddRange(propertyInfo.GetCustomAttributes(attributeType, false));
                    for (var info = GetParentDefinition(propertyInfo);
                         info != null;
                         info = GetParentDefinition(info))
                    {
                        var customAttributes = info.GetCustomAttributes(attributeType, false);
                        AddAttributes(attributes, customAttributes, attributeUsages);
                    }

                    
#if !WINRT
                    var result = Array.CreateInstance(attributeType, attributes.Count) as object[];
                    Array.Copy(attributes.ToArray(), result, result.Length);
                    return result;
#else
                    return attributes;
#endif

                }
            }

            return propertyInfo.GetCustomAttributes(attributeType, inherit);
        }


        private static void AddAttributes(
#if !WINRT
            List<object> 
#else
            List<Attribute>
#endif
            attributes,

#if !WINRT
            object[] 
#else
 IEnumerable<Attribute>
#endif
            customAttributes, Dictionary<Type, bool> attributeUsages)
        {
            foreach (var attribute in customAttributes)
            {
                Type type = attribute.GetType();
                if (!attributeUsages.ContainsKey(type))
                {
                    attributeUsages[type] = InternalGetAttributeUsage(type).Inherited;
                }

                if (attributeUsages[type])
                {
                    attributes.Add(attribute);
                }
            }
        }

        private static AttributeUsageAttribute InternalGetAttributeUsage(Type type)
        {
#if !WINRT
            object[] customAttributes = type.GetCustomAttributes(typeof(AttributeUsageAttribute), true);
            return (AttributeUsageAttribute)customAttributes[0];
#else
            var customAttributes = type.GetTypeInfo().GetCustomAttributes(typeof(AttributeUsageAttribute), true);
            return (AttributeUsageAttribute)customAttributes.First();
#endif
        } 
    }
}