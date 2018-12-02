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
    using System.Reflection;

    /// <summary>
    /// Provides extension methods for <see cref="MemberInfo"/>.
    /// </summary>
    public static class ExtensionsForMemberInfo
    {
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
                return Attribute.IsDefined(propertyInfo, type, true);
            }

            return member.IsDefined(type, true);
        }

        /// <summary>
        /// Determines whether the specified constructor has attribute.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <param name="type">The type of the attribute.</param>
        /// <returns>
        /// <c>true</c> if the specified constructor has attribute; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAttribute(this ConstructorInfo constructor, Type type)
        {
            return constructor.IsDefined(type, true);
        }

        /// <summary>
        /// Determines whether the specified property has attribute.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="type">The type of the attribute.</param>
        /// <returns>
        /// <c>true</c> if the specified property has attribute; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAttribute(this PropertyInfo property, Type type)
        {
            return Attribute.IsDefined(property, type, true);
        }

        /// <summary>
        /// Determines whether the specified method has attribute.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="type">The type of the attribute.</param>
        /// <returns>
        /// <c>true</c> if the specified method has attribute; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAttribute(this MethodInfo method, Type type)
        {
            return method.IsDefined(type, true);
        }

        /// <summary>
        /// Gets the property info from its declared type.
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
                GetIndexParameterTypes(propertyDefinition),
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
            if (member is PropertyInfo propertyInfo)
            {
                return Attribute.GetCustomAttributes(propertyInfo, attributeType, inherited);
            }

            return member.GetCustomAttributes(attributeType, inherited);
        }

        private static Type[] GetIndexParameterTypes(PropertyInfo property)
        {
            var indexParameters = property.GetIndexParameters();
            if (indexParameters.Length == 0)
            {
#if FEATURE_ARRAY_EMPTY
                return Array.Empty<Type>();
#else
                return Arrays.Empty<Type>();
#endif
            }

            var types = new Type[indexParameters.Length];

            for (var i = 0; i < indexParameters.Length; i++)
            {
                types[i] = indexParameters[i].ParameterType;
            }

            return types;
        }
    }
}