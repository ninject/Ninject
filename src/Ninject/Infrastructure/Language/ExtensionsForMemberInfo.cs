//-------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsForMemberInfo.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2016, Ninject Project Contributors
//   Authors: Nate Kohari (nate@enkari.com)
//            Remo Gloor (remo.gloor@gmail.com)
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   you may not use this file except in compliance with one of the Licenses.
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
//-------------------------------------------------------------------------------------------------

namespace Ninject.Infrastructure.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Extensions for MemberInfo
    /// </summary>
    public static class ExtensionsForMemberInfo
    {
        /// <summary>
        /// Determines whether the specified member has attribute.
        /// </summary>
        /// <typeparam name="T">The type of the attribute.</typeparam>
        /// <param name="member">The member.</param>
        /// <returns>
        ///     <c>true</c> if the specified member has attribute; otherwise, <c>false</c>.
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
        ///     <c>true</c> if the specified member has attribute; otherwise, <c>false</c>.
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
        /// <returns>The property info from the declared type of the property.</returns>
        public static PropertyInfo GetPropertyFromDeclaredType(this MemberInfo memberInfo, PropertyInfo propertyDefinition)
        {
            return memberInfo.DeclaringType.GetRuntimeProperties().FirstOrDefault(
                p => p.Name == propertyDefinition.Name &&
                    !p.GetMethod.IsStatic &&
                     p.PropertyType == propertyDefinition.PropertyType &&
                     p.GetIndexParameters().Select(pi => pi.ParameterType).SequenceEqual(
                         propertyDefinition.GetIndexParameters().Select(pi => pi.ParameterType)));
        }

        /// <summary>
        /// Determines whether the specified property info is private.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns>
        ///     <c>true</c> if the specified property info is private; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPrivate(this PropertyInfo propertyInfo)
        {
            var getMethod = propertyInfo.GetMethod;
            var setMethod = propertyInfo.SetMethod;

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
        public static IEnumerable<Attribute> GetCustomAttributesExtended(this MemberInfo member, Type attributeType, bool inherited)
        {
#if !NO_GETCUSTOMATTRIBUTES
            return Attribute.GetCustomAttributes(member, attributeType, inherited);
#else
            if (member is PropertyInfo propertyInfo)
            {
                return GetCustomAttributes(propertyInfo, attributeType, inherited);
            }

            return member.GetCustomAttributes(attributeType, inherited).Cast<Attribute>();
#endif
        }

        private static PropertyInfo GetParentDefinition(PropertyInfo property)
        {
            var propertyMethod = property.GetMethod ?? property.SetMethod;

            if (propertyMethod != null)
            {
                propertyMethod = propertyMethod.GetParentDefinition();
                if (propertyMethod != null)
                {
                    return propertyMethod.GetPropertyFromDeclaredType(property);
                }
            }

            return null;
        }

        private static MethodInfo GetParentDefinition(this MethodInfo method)
        {
            var baseDefinition = method.GetRuntimeBaseDefinition();

            var type = method.DeclaringType.GetTypeInfo().BaseType;

            MethodInfo result = null;
            while (result == null && type != null)
            {
                result = type.GetRuntimeMethods().SingleOrDefault(m => !m.IsStatic && m.GetRuntimeBaseDefinition().Equals(baseDefinition));
                type = type.GetTypeInfo().BaseType;
            }

            return result;
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

        private static IEnumerable<Attribute> GetCustomAttributes(PropertyInfo propertyInfo, Type attributeType, bool inherit)
        {
            if (inherit)
            {
                if (InternalGetAttributeUsage(attributeType).Inherited)
                {
                    var attributes = new List<Attribute>();

                    var attributeUsages = new Dictionary<Type, bool>();
                    attributes.AddRange(propertyInfo.GetCustomAttributes(attributeType, false).Cast<Attribute>());
                    for (var info = GetParentDefinition(propertyInfo);
                         info != null;
                         info = GetParentDefinition(info))
                    {
                        var customAttributes = info.GetCustomAttributes(attributeType, false).Cast<Attribute>();
                        AddAttributes(attributes, customAttributes, attributeUsages);
                    }

                    return attributes;
                }
            }

            return propertyInfo.GetCustomAttributes(attributeType, inherit).Cast<Attribute>();
        }

        private static void AddAttributes(List<Attribute> attributes, IEnumerable<Attribute> customAttributes, Dictionary<Type, bool> attributeUsages)
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
            return type.GetTypeInfo().GetCustomAttribute<AttributeUsageAttribute>(true);
        }
    }
}