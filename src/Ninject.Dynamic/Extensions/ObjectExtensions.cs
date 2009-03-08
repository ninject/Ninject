using System;

namespace Ninject.Dynamic.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="object" />
    /// </summary>
    internal static class ObjectExtensions
    {
        /// <summary>
        /// Determines whether [is not null] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if [is not null] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotNull(this object value)
        {
            return value != null;
        }

        /// <summary>
        /// Determines whether the specified value is null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is null; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNull(this object value)
        {
            return value == null;
        }

        /// <summary>
        /// Ensures that the argument not null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="argumentName">Name of the argument.</param>
        public static void EnsureArgumentNotNull(this object value, string argumentName)
        {
            if (value.IsNull()) throw new ArgumentNullException(argumentName, "Cannot be null");
        }
    }
}