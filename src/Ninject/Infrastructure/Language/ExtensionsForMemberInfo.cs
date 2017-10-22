// -------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsForMemberInfo.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2017 Ninject Project Contributors. All rights reserved.
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   You may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Infrastructure.Language
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides extension methods for <see cref="MemberInfo"/>.
    /// </summary>
    public static class ExtensionsForMemberInfo
    {
        private const BindingFlags DefaultFlags = BindingFlags.Public | BindingFlags.Instance;
#if !NO_LCG
        private const BindingFlags Flags = DefaultFlags | BindingFlags.NonPublic;
#else
        private const BindingFlags Flags = DefaultFlags;
#endif

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

        /// <summary>
        /// Determines whether the specified member has attribute.
        /// </summary>
        /// <typeparam name="T">The type of the attribute.</typeparam>
        /// <param name="member">The member.</param>
        /// <returns>
        /// <c>true</c> if the specified member has attribute; otherwise, <c>false</c>.
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
        /// <c>true</c> if the specified member has attribute; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAttribute(this MemberInfo member, Type type)
        {
            if (member is PropertyInfo propertyInfo)
            {
                return IsDefined(propertyInfo, type, true);
            }

            return member.IsDefined(type, true);
        }

        /// <summary>
        /// Gets the property info from its declared tpe.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <param name="propertyDefinition">The property definition.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>The property info from the declared type of the property.</returns>
        public static PropertyInfo GetPropertyFromDeclaredType(this MemberInfo memberInfo, PropertyInfo propertyDefinition, BindingFlags flags)
        {
            return memberInfo.DeclaringType.GetProperty(
                propertyDefinition.Name,
                flags,
                null,
                propertyDefinition.PropertyType,
                propertyDefinition.GetIndexParameters().Select(parameter => parameter.ParameterType).ToArray(),
                null);
        }

        /// <summary>
        /// Determines whether the specified property info is private.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns>
        /// <c>true</c> if the specified property info is private; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPrivate(this PropertyInfo propertyInfo)
        {
            var getMethod = propertyInfo.GetGetMethod(true);
            var setMethod = propertyInfo.GetSetMethod(true);
            return (getMethod == null || getMethod.IsPrivate) && (setMethod == null || setMethod.IsPrivate);
        }

        /// <summary>
        /// Gets the custom attributes.
        /// This version is able to get custom attributes for properties from base types even if the property is non-public.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <param name="inherited">if set to <c>true</c> [inherited].</param>
        /// <returns>The custom attributes.</returns>
        public static object[] GetCustomAttributesExtended(this MemberInfo member, Type attributeType, bool inherited)
        {
            return Attribute.GetCustomAttributes(member, attributeType, inherited);
        }

        private static PropertyInfo GetParentDefinition(PropertyInfo property)
        {
            var propertyMethod = property.GetGetMethod(true) ?? property.GetSetMethod(true);
            if (propertyMethod != null)
            {
                propertyMethod = propertyMethod.GetParentDefinition(Flags);
                if (propertyMethod != null)
                {
                    return propertyMethod.GetPropertyFromDeclaredType(property, Flags);
                }
            }

            return null;
        }

        private static MethodInfo GetParentDefinition(this MethodInfo method, BindingFlags flags)
        {
            if (ParentDefinitionMethodInfo == null)
            {
                return null;
            }

            return (MethodInfo)ParentDefinitionMethodInfo.Invoke(method, flags, null, null, CultureInfo.InvariantCulture);
        }

        private static bool IsDefined(PropertyInfo element, Type attributeType, bool inherit)
        {
            if (element.IsDefined(attributeType, inherit))
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

        private static object[] GetCustomAttributes(PropertyInfo propertyInfo, Type attributeType, bool inherit)
        {
            if (inherit)
            {
                if (InternalGetAttributeUsage(attributeType).Inherited)
                {
                    var attributeUsages = new Dictionary<Type, bool>();
                    var attributes = new List<object>();
                    attributes.AddRange(propertyInfo.GetCustomAttributes(attributeType, false));
                    for (var info = GetParentDefinition(propertyInfo);
                         info != null;
                         info = GetParentDefinition(info))
                    {
                        var customAttributes = info.GetCustomAttributes(attributeType, false);
                        AddAttributes(attributes, customAttributes, attributeUsages);
                    }

                    var result = Array.CreateInstance(attributeType, attributes.Count) as object[];
                    Array.Copy(attributes.ToArray(), result, result.Length);
                    return result;
                }
            }

            return propertyInfo.GetCustomAttributes(attributeType, inherit);
        }

        private static void AddAttributes(List<object> attributes, object[] customAttributes, Dictionary<Type, bool> attributeUsages)
        {
            foreach (var attribute in customAttributes)
            {
                var type = attribute.GetType();
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
            return type.GetCustomAttribute<AttributeUsageAttribute>(true);
        }
    }
}