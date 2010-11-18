//-------------------------------------------------------------------------------
// <copyright file="AssertWithThrows.cs" company="bbv Software Services AG">
//   Copyright (c) 2010 Software Services AG
//   Remo Gloor (remo.gloor@gmail.com)
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace Ninject
{
    using System;
    using System.Globalization;
#if SILVERLIGHT_MSTEST
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
    using Assert = UnitDriven.Assert;
#endif

    /// <summary>
    /// An assert implementation that adds Throws and deos not throw for UnitDriven and MsTest
    /// </summary>
    public class AssertWithThrows
    {
        /// <summary>
        /// Asserts that the action does not throw an exception.
        /// </summary>
        /// <param name="action">The action.</param>
        public static void DoesNotThrow(Action action)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
                Assert.Fail("Expected no exception");
            }
        }

        /// <summary>
        /// Asserts that the action throws the specified exception.
        /// </summary>
        /// <typeparam name="T">The type of exception that is expected to be thrown.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>The exception</returns>
        public static T Throws<T>(Action action)
            where T : Exception
        {
            try
            {
                action();
                Assert.Fail(string.Format(CultureInfo.InvariantCulture, "Expected excpetion {0} did not occur!", typeof(T).Name));
            }
            catch (T e)
            {
                return e;
            }

            return null;
        }
    }
}
