#region Usings

using System.Globalization;

#endregion

namespace Ninject.Dynamic.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="string"/>
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Determines whether the value is null or blank.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is null or blank; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrBlank(this string value)
        {
            return string.IsNullOrEmpty(value) || value.Trim().Length == 0;
        }

        /// <summary>
        /// Determines whether the specified value is not null or blank.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is not null or blank; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotNullOrBlank(this string value)
        {
            return !value.IsNullOrBlank();
        }

        /// <summary>
        /// Formats the specified format string with the provided parameters.
        /// </summary>
        /// <param name="value">The format string.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static string FormattedWith(this string value, params object[] parameters)
        {
            return string.Format(CultureInfo.CurrentUICulture, value, parameters);
        }
    }
}