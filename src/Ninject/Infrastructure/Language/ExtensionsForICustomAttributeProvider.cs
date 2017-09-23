// -------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsForICustomAttributeProvider.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Infrastructure.Language
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Provides extension methods for <see cref="ICustomAttributeProvider"/>.
    /// </summary>
    internal static class ExtensionsForICustomAttributeProvider
    {
        /// <summary>
        /// Determines if the <see cref="ICustomAttributeProvider"/> has the specified attribute.
        /// </summary>
        /// <param name="member">The <see cref="ICustomAttributeProvider"/>.</param>
        /// <param name="type">The attribute type.</param>
        /// <returns><c>True</c> if the <see cref="ICustomAttributeProvider"/> has the attribute, otherwise <c>False</c>.</returns>
        public static bool HasAttribute(this ICustomAttributeProvider member, Type type)
        {
            if (member is MemberInfo memberInfo)
            {
                return memberInfo.HasAttribute(type);
            }

            return member.IsDefined(type, true);
        }

        /// <summary>
        /// Gets custom attributes which supports <see cref="MemberInfo"/> and <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="member">The <see cref="ICustomAttributeProvider"/>.</param>
        /// <param name="attributeType">The attribute type.</param>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
        /// <returns>The attributes.</returns>
        public static object[] GetCustomAttributesExtended(this ICustomAttributeProvider member, Type attributeType, bool inherit)
        {
            if (member is MemberInfo memberInfo)
            {
                return memberInfo.GetCustomAttributesExtended(attributeType, inherit);
            }

            return member.GetCustomAttributes(attributeType, inherit);
        }
    }
}