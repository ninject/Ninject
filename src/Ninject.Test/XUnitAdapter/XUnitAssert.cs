
#if MSTEST
// Type: Xunit.Assert
// Assembly: xunit, Version=1.8.0.0, Culture=neutral, PublicKeyToken=8d05b1bb7a6fdb6c
// Assembly location: C:\Users\Public\Downloads\Ninject3\ninject\tools\xunit.net\xunit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Xunit.Sdk;

namespace Xunit
{
    /// <summary>
    /// Contains various static methods that are used to verify that conditions are met during the
    ///             process of running tests.
    /// 
    /// </summary>
    public class Assert
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Xunit.Assert"/> class.
        /// 
        /// </summary>
        protected Assert()
        {
        }

        /// <summary>
        /// Verifies that a collection contains a given object.
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the object to be verified</typeparam><param name="expected">The object expected to be in the collection</param><param name="collection">The collection to be inspected</param><exception cref="T:Xunit.Sdk.ContainsException">Thrown when the object is not present in the collection</exception>
        public static void Contains<T>(T expected, IEnumerable<T> collection)
        {
            Assert.Contains<T>(expected, collection, Assert.GetEqualityComparer<T>());
        }

        /// <summary>
        /// Verifies that a collection contains a given object, using an equality comparer.
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the object to be verified</typeparam><param name="expected">The object expected to be in the collection</param><param name="collection">The collection to be inspected</param><param name="comparer">The comparer used to equate objects in the collection with the expected object</param><exception cref="T:Xunit.Sdk.ContainsException">Thrown when the object is not present in the collection</exception>
        public static void Contains<T>(T expected, IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            if (collection != null)
            {
                foreach (T y in collection)
                {
                    if (comparer.Equals(expected, y))
                        return;
                }
            }
            throw new ContainsException((object)expected);
        }

        /// <summary>
        /// Verifies that a string contains a given sub-string, using the current culture.
        /// 
        /// </summary>
        /// <param name="expectedSubstring">The sub-string expected to be in the string</param><param name="actualString">The string to be inspected</param><exception cref="T:Xunit.Sdk.ContainsException">Thrown when the sub-string is not present inside the string</exception>
        public static void Contains(string expectedSubstring, string actualString)
        {
            Assert.Contains(expectedSubstring, actualString, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Verifies that a string contains a given sub-string, using the given comparison type.
        /// 
        /// </summary>
        /// <param name="expectedSubstring">The sub-string expected to be in the string</param><param name="actualString">The string to be inspected</param><param name="comparisonType">The type of string comparison to perform</param><exception cref="T:Xunit.Sdk.ContainsException">Thrown when the sub-string is not present inside the string</exception>
        public static void Contains(string expectedSubstring, string actualString, StringComparison comparisonType)
        {
            if (actualString == null || actualString.IndexOf(expectedSubstring, comparisonType) < 0)
                throw new ContainsException((object)expectedSubstring, (object)actualString);
        }

        /// <summary>
        /// Verifies that a collection does not contain a given object.
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the object to be compared</typeparam><param name="expected">The object that is expected not to be in the collection</param><param name="collection">The collection to be inspected</param><exception cref="T:Xunit.Sdk.DoesNotContainException">Thrown when the object is present inside the container</exception>
        public static void DoesNotContain<T>(T expected, IEnumerable<T> collection)
        {
            Assert.DoesNotContain<T>(expected, collection, Assert.GetEqualityComparer<T>());
        }

        /// <summary>
        /// Verifies that a collection does not contain a given object, using an equality comparer.
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the object to be compared</typeparam><param name="expected">The object that is expected not to be in the collection</param><param name="collection">The collection to be inspected</param><param name="comparer">The comparer used to equate objects in the collection with the expected object</param><exception cref="T:Xunit.Sdk.DoesNotContainException">Thrown when the object is present inside the container</exception>
        public static void DoesNotContain<T>(T expected, IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            if (collection == null)
                return;
            foreach (T y in collection)
            {
                if (comparer.Equals(expected, y))
                    throw new DoesNotContainException((object)expected);
            }
        }

        /// <summary>
        /// Verifies that a string does not contain a given sub-string, using the current culture.
        /// 
        /// </summary>
        /// <param name="expectedSubstring">The sub-string which is expected not to be in the string</param><param name="actualString">The string to be inspected</param><exception cref="T:Xunit.Sdk.DoesNotContainException">Thrown when the sub-string is present inside the string</exception>
        public static void DoesNotContain(string expectedSubstring, string actualString)
        {
            Assert.DoesNotContain(expectedSubstring, actualString, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Verifies that a string does not contain a given sub-string, using the current culture.
        /// 
        /// </summary>
        /// <param name="expectedSubstring">The sub-string which is expected not to be in the string</param><param name="actualString">The string to be inspected</param><param name="comparisonType">The type of string comparison to perform</param><exception cref="T:Xunit.Sdk.DoesNotContainException">Thrown when the sub-string is present inside the given string</exception>
        public static void DoesNotContain(string expectedSubstring, string actualString, StringComparison comparisonType)
        {
            if (actualString != null && actualString.IndexOf(expectedSubstring, comparisonType) >= 0)
                throw new DoesNotContainException((object)expectedSubstring);
        }

        /// <summary>
        /// Verifies that a block of code does not throw any exceptions.
        /// 
        /// </summary>
        /// <param name="testCode">A delegate to the code to be tested</param>
        public static void DoesNotThrow(Assert.ThrowsDelegate testCode)
        {
            Exception actual = Record.Exception(testCode);
            if (actual != null)
                throw new DoesNotThrowException(actual);
        }

        /// <summary>
        /// Verifies that a collection is empty.
        /// 
        /// </summary>
        /// <param name="collection">The collection to be inspected</param><exception cref="T:System.ArgumentNullException">Thrown when the collection is null</exception><exception cref="T:Xunit.Sdk.EmptyException">Thrown when the collection is not empty</exception>
        public static void Empty(IEnumerable collection)
        {
            Guard.ArgumentNotNull("collection", (object)collection);
            IEnumerator enumerator = collection.GetEnumerator();
            try
            {
                if (!enumerator.MoveNext())
                    return;
                object current = enumerator.Current;
                throw new EmptyException();
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }

        /// <summary>
        /// Verifies that two objects are equal, using a default comparer.
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the objects to be compared</typeparam><param name="expected">The expected value</param><param name="actual">The value to be compared against</param><exception cref="T:Xunit.Sdk.EqualException">Thrown when the objects are not equal</exception>
        public static void Equal<T>(T expected, T actual)
        {
            Assert.Equal<T>(expected, actual, Assert.GetEqualityComparer<T>());
        }

        /// <summary>
        /// Verifies that two objects are equal, using a custom equatable comparer.
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the objects to be compared</typeparam><param name="expected">The expected value</param><param name="actual">The value to be compared against</param><param name="comparer">The comparer used to compare the two objects</param><exception cref="T:Xunit.Sdk.EqualException">Thrown when the objects are not equal</exception>
        public static void Equal<T>(T expected, T actual, IEqualityComparer<T> comparer)
        {
            if (!comparer.Equals(expected, actual))
                throw new EqualException((object)expected, (object)actual);
        }

        /// <summary>
        /// Verifies that two <see cref="T:System.Double"/> values are equal, within the number of decimal
        ///             places given by <paramref name="precision"/>.
        /// 
        /// </summary>
        /// <param name="expected">The expected value</param><param name="actual">The value to be compared against</param><param name="precision">The number of decimal places (valid values: 0-15)</param><exception cref="T:Xunit.Sdk.EqualException">Thrown when the values are not equal</exception>
        public static void Equal(double expected, double actual, int precision)
        {
            double x = Math.Round(expected, precision);
            double y = Math.Round(actual, precision);
            if (Assert.GetEqualityComparer<double>().Equals(x, y))
                return;
            bool skipPositionCheck = true;
            throw new EqualException((object)string.Format("{0} (rounded from {1})", (object)x, (object)expected), (object)string.Format("{0} (rounded from {1})", (object)y, (object)actual), skipPositionCheck);
        }

        /// <summary>
        /// Verifies that two <see cref="T:System.Decimal"/> values are equal, within the number of decimal
        ///             places given by <paramref name="precision"/>.
        /// 
        /// </summary>
        /// <param name="expected">The expected value</param><param name="actual">The value to be compared against</param><param name="precision">The number of decimal places (valid values: 0-15)</param><exception cref="T:Xunit.Sdk.EqualException">Thrown when the values are not equal</exception>
        public static void Equal(Decimal expected, Decimal actual, int precision)
        {
            Decimal x = Math.Round(expected, precision);
            Decimal y = Math.Round(actual, precision);
            if (Assert.GetEqualityComparer<Decimal>().Equals(x, y))
                return;
            bool skipPositionCheck = true;
            throw new EqualException((object)string.Format("{0} (rounded from {1})", (object)x, (object)expected), (object)string.Format("{0} (rounded from {1})", (object)y, (object)actual), skipPositionCheck);
        }

        /// <summary>
        /// Do not call this method.
        /// </summary>
        [Obsolete("This is an override of Object.Equals(). Call Assert.Equal() instead.", true)]
        public new static bool Equals(object a, object b)
        {
            throw new InvalidOperationException("Assert.Equals should not be used");
        }

        /// <summary>
        /// Verifies that the condition is false.
        /// 
        /// </summary>
        /// <param name="condition">The condition to be tested</param><exception cref="T:Xunit.Sdk.FalseException">Thrown if the condition is not false</exception>
        public static void False(bool condition)
        {
            Assert.False(condition, (string)null);
        }

        /// <summary>
        /// Verifies that the condition is false.
        /// 
        /// </summary>
        /// <param name="condition">The condition to be tested</param><param name="userMessage">The message to show when the condition is not false</param><exception cref="T:Xunit.Sdk.FalseException">Thrown if the condition is not false</exception>
        public static void False(bool condition, string userMessage)
        {
            if (condition)
                throw new FalseException(userMessage);
        }

        private static IComparer<T> GetComparer<T>() where T : IComparable
        {
            return (IComparer<T>)new Assert.AssertComparer<T>();
        }

        private static IEqualityComparer<T> GetEqualityComparer<T>()
        {
            return (IEqualityComparer<T>)new Assert.AssertEqualityComparer<T>();
        }

        /// <summary>
        /// Verifies that a value is within a given range.
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the value to be compared</typeparam><param name="actual">The actual value to be evaluated</param><param name="low">The (inclusive) low value of the range</param><param name="high">The (inclusive) high value of the range</param><exception cref="T:Xunit.Sdk.InRangeException">Thrown when the value is not in the given range</exception>
        public static void InRange<T>(T actual, T low, T high) where T : IComparable
        {
            Assert.InRange<T>(actual, low, high, Assert.GetComparer<T>());
        }

        /// <summary>
        /// Verifies that a value is within a given range, using a comparer.
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the value to be compared</typeparam><param name="actual">The actual value to be evaluated</param><param name="low">The (inclusive) low value of the range</param><param name="high">The (inclusive) high value of the range</param><param name="comparer">The comparer used to evaluate the value's range</param><exception cref="T:Xunit.Sdk.InRangeException">Thrown when the value is not in the given range</exception>
        public static void InRange<T>(T actual, T low, T high, IComparer<T> comparer)
        {
            if (comparer.Compare(low, actual) > 0 || comparer.Compare(actual, high) > 0)
                throw new InRangeException((object)actual, (object)low, (object)high);
        }

        /// <summary>
        /// Verifies that an object is of the given type or a derived type.
        /// 
        /// </summary>
        /// <typeparam name="T">The type the object should be</typeparam><param name="object">The object to be evaluated</param>
        /// <returns>
        /// The object, casted to type T when successful
        /// </returns>
        /// <exception cref="T:Xunit.Sdk.IsAssignableFromException">Thrown when the object is not the given type</exception>
        public static T IsAssignableFrom<T>(object @object)
        {
            Assert.IsAssignableFrom(typeof(T), @object);
            return (T)@object;
        }

        /// <summary>
        /// Verifies that an object is of the given type or a derived type.
        /// 
        /// </summary>
        /// <param name="expectedType">The type the object should be</param><param name="object">The object to be evaluated</param><exception cref="T:Xunit.Sdk.IsAssignableFromException">Thrown when the object is not the given type</exception>
        public static void IsAssignableFrom(Type expectedType, object @object)
        {
            if (@object == null || !expectedType.GetTypeInfo().IsAssignableFrom(@object.GetType().GetTypeInfo()))
                throw new IsAssignableFromException(expectedType, @object);
        }

        /// <summary>
        /// Verifies that an object is not exactly the given type.
        /// 
        /// </summary>
        /// <typeparam name="T">The type the object should not be</typeparam><param name="object">The object to be evaluated</param><exception cref="T:Xunit.Sdk.IsNotTypeException">Thrown when the object is the given type</exception>
        public static void IsNotType<T>(object @object)
        {
            Assert.IsNotType(typeof(T), @object);
        }

        /// <summary>
        /// Verifies that an object is not exactly the given type.
        /// 
        /// </summary>
        /// <param name="expectedType">The type the object should not be</param><param name="object">The object to be evaluated</param><exception cref="T:Xunit.Sdk.IsNotTypeException">Thrown when the object is the given type</exception>
        public static void IsNotType(Type expectedType, object @object)
        {
            if (@object != null && expectedType.Equals(@object.GetType()))
                throw new IsNotTypeException(expectedType, @object);
        }

        /// <summary>
        /// Verifies that an object is exactly the given type (and not a derived type).
        /// 
        /// </summary>
        /// <typeparam name="T">The type the object should be</typeparam><param name="object">The object to be evaluated</param>
        /// <returns>
        /// The object, casted to type T when successful
        /// </returns>
        /// <exception cref="T:Xunit.Sdk.IsTypeException">Thrown when the object is not the given type</exception>
        public static T IsType<T>(object @object)
        {
            Assert.IsType(typeof(T), @object);
            return (T)@object;
        }

        /// <summary>
        /// Verifies that an object is exactly the given type (and not a derived type).
        /// 
        /// </summary>
        /// <param name="expectedType">The type the object should be</param><param name="object">The object to be evaluated</param><exception cref="T:Xunit.Sdk.IsTypeException">Thrown when the object is not the given type</exception>
        public static void IsType(Type expectedType, object @object)
        {
            if (@object == null || !expectedType.Equals(@object.GetType()))
                throw new IsTypeException(expectedType, @object);
        }

        /// <summary>
        /// Verifies that a collection is not empty.
        /// 
        /// </summary>
        /// <param name="collection">The collection to be inspected</param><exception cref="T:System.ArgumentNullException">Thrown when a null collection is passed</exception><exception cref="T:Xunit.Sdk.NotEmptyException">Thrown when the collection is empty</exception>
        public static void NotEmpty(IEnumerable collection)
        {
            Guard.ArgumentNotNull("collection", (object)collection);
            IEnumerator enumerator = collection.GetEnumerator();
            try
            {
                if (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    return;
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
            throw new NotEmptyException();
        }

        /// <summary>
        /// Verifies that two objects are not equal, using a default comparer.
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the objects to be compared</typeparam><param name="expected">The expected object</param><param name="actual">The actual object</param><exception cref="T:Xunit.Sdk.NotEqualException">Thrown when the objects are equal</exception>
        public static void NotEqual<T>(T expected, T actual)
        {
            Assert.NotEqual<T>(expected, actual, Assert.GetEqualityComparer<T>());
        }

        /// <summary>
        /// Verifies that two objects are not equal, using a custom equality comparer.
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the objects to be compared</typeparam><param name="expected">The expected object</param><param name="actual">The actual object</param><param name="comparer">The comparer used to examine the objects</param><exception cref="T:Xunit.Sdk.NotEqualException">Thrown when the objects are equal</exception>
        public static void NotEqual<T>(T expected, T actual, IEqualityComparer<T> comparer)
        {
            if (comparer.Equals(expected, actual))
                throw new NotEqualException();
        }

        /// <summary>
        /// Verifies that a value is not within a given range, using the default comparer.
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the value to be compared</typeparam><param name="actual">The actual value to be evaluated</param><param name="low">The (inclusive) low value of the range</param><param name="high">The (inclusive) high value of the range</param><exception cref="T:Xunit.Sdk.NotInRangeException">Thrown when the value is in the given range</exception>
        public static void NotInRange<T>(T actual, T low, T high) where T : IComparable
        {
            Assert.NotInRange<T>(actual, low, high, Assert.GetComparer<T>());
        }

        /// <summary>
        /// Verifies that a value is not within a given range, using a comparer.
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the value to be compared</typeparam><param name="actual">The actual value to be evaluated</param><param name="low">The (inclusive) low value of the range</param><param name="high">The (inclusive) high value of the range</param><param name="comparer">The comparer used to evaluate the value's range</param><exception cref="T:Xunit.Sdk.NotInRangeException">Thrown when the value is in the given range</exception>
        public static void NotInRange<T>(T actual, T low, T high, IComparer<T> comparer)
        {
            if (comparer.Compare(low, actual) <= 0 && comparer.Compare(actual, high) <= 0)
                throw new NotInRangeException((object)actual, (object)low, (object)high);
        }

        /// <summary>
        /// Verifies that an object reference is not null.
        /// 
        /// </summary>
        /// <param name="object">The object to be validated</param><exception cref="T:Xunit.Sdk.NotNullException">Thrown when the object is not null</exception>
        public static void NotNull(object @object)
        {
            if (@object == null)
                throw new NotNullException();
        }

        /// <summary>
        /// Verifies that two objects are not the same instance.
        /// 
        /// </summary>
        /// <param name="expected">The expected object instance</param><param name="actual">The actual object instance</param><exception cref="T:Xunit.Sdk.NotSameException">Thrown when the objects are the same instance</exception>
        public static void NotSame(object expected, object actual)
        {
            if (object.ReferenceEquals(expected, actual))
                throw new NotSameException();
        }

        /// <summary>
        /// Verifies that an object reference is null.
        /// 
        /// </summary>
        /// <param name="object">The object to be inspected</param><exception cref="T:Xunit.Sdk.NullException">Thrown when the object reference is not null</exception>
        public static void Null(object @object)
        {
            if (@object != null)
                throw new NullException(@object);
        }

        ///// <summary>
        ///// Verifies that the provided object raised INotifyPropertyChanged.PropertyChanged
        /////             as a result of executing the given test code.
        ///// 
        ///// </summary>
        ///// <param name="object">The object which should raise the notification</param><param name="propertyName">The property name for which the notification should be raised</param><param name="testCode">The test code which should cause the notification to be raised</param><exception cref="T:Xunit.Sdk.PropertyChangedException">Thrown when the notification is not raised</exception>
        //public static void PropertyChanged(INotifyPropertyChanged @object, string propertyName, Assert.PropertyChangedDelegate testCode)
        //{
        //    bool propertyChangeHappened = false;
        //    PropertyChangedEventHandler changedEventHandler = (PropertyChangedEventHandler)((sender, args) =>
        //    {
        //        if (!propertyName.Equals(args.PropertyName, StringComparison.InvariantCultureIgnoreCase))
        //            ;
        //    });
        //    @object.PropertyChanged += changedEventHandler;
        //    try
        //    {
        //        testCode();
        //        if (!propertyChangeHappened)
        //            throw new PropertyChangedException(propertyName);
        //    }
        //    finally
        //    {
        //        @object.PropertyChanged -= changedEventHandler;
        //    }
        //}

        /// <summary>
        /// Verifies that two objects are the same instance.
        /// 
        /// </summary>
        /// <param name="expected">The expected object instance</param><param name="actual">The actual object instance</param><exception cref="T:Xunit.Sdk.SameException">Thrown when the objects are not the same instance</exception>
        public static void Same(object expected, object actual)
        {
            if (!object.ReferenceEquals(expected, actual))
                throw new SameException(expected, actual);
        }

        /// <summary>
        /// Verifies that the given collection contains only a single
        ///             element of the given type.
        /// 
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>
        /// The single item in the collection.
        /// </returns>
        /// <exception cref="T:Xunit.Sdk.SingleException">Thrown when the collection does not contain
        ///             exactly one element.</exception>
        public static object Single(IEnumerable collection)
        {
            Guard.ArgumentNotNull("collection", (object)collection);
            int count = 0;
            object obj1 = (object)null;
            foreach (object obj2 in collection)
            {
                obj1 = obj2;
                ++count;
            }
            if (count != 1)
                throw new SingleException(count);
            else
                return obj1;
        }

        /// <summary>
        /// Verifies that the given collection contains only a single
        ///             element of the given value. The collection may or may not
        ///             contain other values.
        /// 
        /// </summary>
        /// <param name="collection">The collection.</param><param name="expected">The value to find in the collection.</param>
        /// <returns>
        /// The single item in the collection.
        /// </returns>
        /// <exception cref="T:Xunit.Sdk.SingleException">Thrown when the collection does not contain
        ///             exactly one element.</exception>
        public static void Single(IEnumerable collection, object expected)
        {
            Guard.ArgumentNotNull("collection", (object)collection);
            int count = 0;
            foreach (object objA in collection)
            {
                if (object.Equals(objA, expected))
                    ++count;
            }
            if (count != 1)
                throw new SingleException(count, expected);
        }

        /// <summary>
        /// Verifies that the given collection contains only a single
        ///             element of the given type.
        /// 
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam><param name="collection">The collection.</param>
        /// <returns>
        /// The single item in the collection.
        /// </returns>
        /// <exception cref="T:Xunit.Sdk.SingleException">Thrown when the collection does not contain
        ///             exactly one element.</exception>
        public static T Single<T>(IEnumerable<T> collection)
        {
            return Assert.Single<T>(collection, (Predicate<T>)(item => true));
        }

        /// <summary>
        /// Verifies that the given collection contains only a single
        ///             element of the given type which matches the given predicate. The
        ///             collection may or may not contain other values which do not
        ///             match the given predicate.
        /// 
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam><param name="collection">The collection.</param><param name="predicate">The item matching predicate.</param>
        /// <returns>
        /// The single item in the filtered collection.
        /// </returns>
        /// <exception cref="T:Xunit.Sdk.SingleException">Thrown when the filtered collection does
        ///             not contain exactly one element.</exception>
        public static T Single<T>(IEnumerable<T> collection, Predicate<T> predicate)
        {
            Guard.ArgumentNotNull("collection", (object)collection);
            Guard.ArgumentNotNull("predicate", (object)predicate);
            int count = 0;
            T obj1 = default(T);
            foreach (T obj2 in collection)
            {
                if (predicate(obj2))
                {
                    obj1 = obj2;
                    ++count;
                }
            }
            if (count != 1)
                throw new SingleException(count);
            else
                return obj1;
        }

        /// <summary>
        /// Verifies that the exact exception is thrown (and not a derived exception type).
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the exception expected to be thrown</typeparam><param name="testCode">A delegate to the code to be tested</param>
        /// <returns>
        /// The exception that was thrown, when successful
        /// </returns>
        /// <exception cref="T:Xunit.Sdk.ThrowsException">Thrown when an exception was not thrown, or when an exception of the incorrect type is thrown</exception>
        public static T Throws<T>(Assert.ThrowsDelegate testCode) where T : Exception
        {
            return (T)Assert.Throws(typeof(T), testCode);
        }

        /// <summary>
        /// Verifies that the exact exception is thrown (and not a derived exception type).
        ///             Generally used to test property accessors.
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the exception expected to be thrown</typeparam><param name="testCode">A delegate to the code to be tested</param>
        /// <returns>
        /// The exception that was thrown, when successful
        /// </returns>
        /// <exception cref="T:Xunit.Sdk.ThrowsException">Thrown when an exception was not thrown, or when an exception of the incorrect type is thrown</exception>
        public static T Throws<T>(Assert.ThrowsDelegateWithReturn testCode) where T : Exception
        {
            return (T)Assert.Throws(typeof(T), testCode);
        }

        /// <summary>
        /// Verifies that the exact exception is thrown (and not a derived exception type).
        /// 
        /// </summary>
        /// <param name="exceptionType">The type of the exception expected to be thrown</param><param name="testCode">A delegate to the code to be tested</param>
        /// <returns>
        /// The exception that was thrown, when successful
        /// </returns>
        /// <exception cref="T:Xunit.Sdk.ThrowsException">Thrown when an exception was not thrown, or when an exception of the incorrect type is thrown</exception>
        public static Exception Throws(Type exceptionType, Assert.ThrowsDelegate testCode)
        {
            Exception actual = Record.Exception(testCode);
            if (actual == null)
                throw new ThrowsException(exceptionType);
            if (!exceptionType.Equals(actual.GetType()))
                throw new ThrowsException(exceptionType, actual);
            else
                return actual;
        }

        /// <summary>
        /// Verifies that the exact exception is thrown (and not a derived exception type).
        ///             Generally used to test property accessors.
        /// 
        /// </summary>
        /// <param name="exceptionType">The type of the exception expected to be thrown</param><param name="testCode">A delegate to the code to be tested</param>
        /// <returns>
        /// The exception that was thrown, when successful
        /// </returns>
        /// <exception cref="T:Xunit.Sdk.ThrowsException">Thrown when an exception was not thrown, or when an exception of the incorrect type is thrown</exception>
        public static Exception Throws(Type exceptionType, Assert.ThrowsDelegateWithReturn testCode)
        {
            Exception actual = Record.Exception(testCode);
            if (actual == null)
                throw new ThrowsException(exceptionType);
            if (!exceptionType.Equals(actual.GetType()))
                throw new ThrowsException(exceptionType, actual);
            else
                return actual;
        }

        /// <summary>
        /// Verifies that an expression is true.
        /// 
        /// </summary>
        /// <param name="condition">The condition to be inspected</param><exception cref="T:Xunit.Sdk.TrueException">Thrown when the condition is false</exception>
        public static void True(bool condition)
        {
            Assert.True(condition, (string)null);
        }

        /// <summary>
        /// Verifies that an expression is true.
        /// 
        /// </summary>
        /// <param name="condition">The condition to be inspected</param><param name="userMessage">The message to be shown when the condition is false</param><exception cref="T:Xunit.Sdk.TrueException">Thrown when the condition is false</exception>
        public static void True(bool condition, string userMessage)
        {
            if (!condition)
                throw new TrueException(userMessage);
        }

        /// <summary>
        /// Used by the PropertyChanged.
        /// 
        /// </summary>
        public delegate void PropertyChangedDelegate();

        /// <summary>
        /// Used by the Throws and DoesNotThrow methods.
        /// 
        /// </summary>
        public delegate void ThrowsDelegate();

        /// <summary>
        /// Used by the Throws and DoesNotThrow methods.
        /// 
        /// </summary>
        public delegate object ThrowsDelegateWithReturn();

        private class AssertEqualityComparer<T> : IEqualityComparer<T>
        {
            private static Assert.AssertEqualityComparer<object> innerComparer = new Assert.AssertEqualityComparer<object>();

            static AssertEqualityComparer()
            {
            }

            public bool Equals(T x, T y)
            {
                TypeInfo type = typeof(T).GetTypeInfo();
                if (!type.IsValueType || type.IsGenericType && type.GetGenericTypeDefinition().GetTypeInfo().IsAssignableFrom(typeof(Nullable<>).GetTypeInfo()))
                {
                    if (object.Equals((object)x, (object)default(T)))
                        return object.Equals((object)y, (object)default(T));
                    if (object.Equals((object)y, (object)default(T)))
                        return false;
                }
                if (x.GetType() != y.GetType())
                    return false;
                IEquatable<T> equatable = (object)x as IEquatable<T>;
                if (equatable != null)
                    return equatable.Equals(y);
                IComparable<T> comparable1 = (object)x as IComparable<T>;
                if (comparable1 != null)
                    return comparable1.CompareTo(y) == 0;
                IComparable comparable2 = (object)x as IComparable;
                if (comparable2 != null)
                    return comparable2.CompareTo((object)y) == 0;
                IEnumerable enumerable1 = (object)x as IEnumerable;
                IEnumerable enumerable2 = (object)y as IEnumerable;
                if (enumerable1 == null || enumerable2 == null)
                    return object.Equals((object)x, (object)y);
                IEnumerator enumerator1 = enumerable1.GetEnumerator();
                IEnumerator enumerator2 = enumerable2.GetEnumerator();
                do
                {
                    bool flag1 = enumerator1.MoveNext();
                    bool flag2 = enumerator2.MoveNext();
                    if (!flag1 || !flag2)
                        return flag1 == flag2;
                }
                while (Assert.AssertEqualityComparer<T>.innerComparer.Equals(enumerator1.Current, enumerator2.Current));
                return false;
            }

            public int GetHashCode(T obj)
            {
                throw new NotImplementedException();
            }
        }

        private class AssertComparer<T> : IComparer<T> where T : IComparable
        {
            public int Compare(T x, T y)
            {
                TypeInfo type = typeof(T).GetTypeInfo();
                if (!type.IsValueType || type.IsGenericType && type.GetGenericTypeDefinition().GetTypeInfo().IsAssignableFrom(typeof(Nullable<>).GetTypeInfo()))
                {
                    if (object.Equals((object)x, (object)default(T)))
                        return object.Equals((object)y, (object)default(T)) ? 0 : -1;
                    else if (object.Equals((object)y, (object)default(T)))
                        return -1;
                }
                if (x.GetType() != y.GetType())
                    return -1;
                IComparable<T> comparable = (object)x as IComparable<T>;
                if (comparable != null)
                    return comparable.CompareTo(y);
                else
                    return x.CompareTo((object)y);
            }
        }
    }

    /// <summary>
    /// Allows the user to record actions for a test.
    /// 
    /// </summary>
    public class Record
    {
        /// <summary>
        /// Records any exception which is thrown by the given code.
        /// 
        /// </summary>
        /// <param name="code">The code which may thrown an exception.</param>
        /// <returns>
        /// Returns the exception that was thrown by the code; null, otherwise.
        /// </returns>
        public static Exception Exception(Assert.ThrowsDelegate code)
        {
            try
            {
                code();
                return (Exception)null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// Records any exception which is thrown by the given code that has
        ///             a return value. Generally used for testing property accessors.
        /// 
        /// </summary>
        /// <param name="code">The code which may thrown an exception.</param>
        /// <returns>
        /// Returns the exception that was thrown by the code; null, otherwise.
        /// </returns>
        public static Exception Exception(Assert.ThrowsDelegateWithReturn code)
        {
            try
            {
                object obj = code();
                return (Exception)null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }

    /// <summary>
    /// Exception thrown when a collection is unexpectedly empty.
    /// 
    /// </summary>
    public class NotEmptyException : AssertException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.NotEmptyException"/> class.
        /// 
        /// </summary>
        public NotEmptyException()
            : base("Assert.NotEmpty() failure")
        {
        }
    }

    /// <summary>
    /// Exception thrown when two values are unexpectedly equal.
    /// 
    /// </summary>
    public class NotEqualException : AssertException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.NotEqualException"/> class.
        /// 
        /// </summary>
        public NotEqualException()
            : base("Assert.NotEqual() Failure")
        {
        }
    }

    /// <summary>
    /// Exception thrown when a value is unexpectedly in the given range.
    /// 
    /// </summary>
    public class NotInRangeException : AssertException
    {
        private readonly string actual;
        private readonly string high;
        private readonly string low;

        /// <summary>
        /// Gets the actual object value
        /// 
        /// </summary>
        public string Actual
        {
            get
            {
                return this.actual;
            }
        }

        /// <summary>
        /// Gets the high value of the range
        /// 
        /// </summary>
        public string High
        {
            get
            {
                return this.high;
            }
        }

        /// <summary>
        /// Gets the low value of the range
        /// 
        /// </summary>
        public string Low
        {
            get
            {
                return this.low;
            }
        }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// The error message that explains the reason for the exception, or an empty string("").
        /// </returns>
        public override string Message
        {
            get
            {
                return string.Format("{0}\r\nRange:  ({1} - {2})\r\nActual: {3}", (object)base.Message, (object)this.Low, (object)this.High, (object)(this.Actual ?? "(null)"));
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.NotInRangeException"/> class.
        /// 
        /// </summary>
        /// <param name="actual">The actual object value</param><param name="low">The low value of the range</param><param name="high">The high value of the range</param>
        public NotInRangeException(object actual, object low, object high)
            : base("Assert.NotInRange() Failure")
        {
            this.low = low == null ? (string)null : low.ToString();
            this.high = high == null ? (string)null : high.ToString();
            this.actual = actual == null ? (string)null : actual.ToString();
        }
    }

    /// <summary>
    /// Exception thrown when an object is unexpectedly null.
    /// 
    /// </summary>
    public class NotNullException : AssertException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.NotNullException"/> class.
        /// 
        /// </summary>
        public NotNullException()
            : base("Assert.NotNull() Failure")
        {
        }
    }

    /// <summary>
    /// Exception thrown when two values are unexpected the same instance.
    /// 
    /// </summary>
    public class NotSameException : AssertException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.NotSameException"/> class.
        /// 
        /// </summary>
        public NotSameException()
            : base("Assert.NotSame() Failure")
        {
        }
    }

    /// <summary>
    /// Exception thrown when an object reference is unexpectedly not null.
    /// 
    /// </summary>
    public class NullException : AssertActualExpectedException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.NullException"/> class.
        /// 
        /// </summary>
        /// <param name="actual"/>
        public NullException(object actual)
            : base((object)null, actual, "Assert.Null() Failure")
        {
        }
    }

    /// <summary>
    /// Exception thrown when two object references are unexpectedly not the same instance.
    /// 
    /// </summary>
    public class SameException : AssertActualExpectedException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.SameException"/> class.
        /// 
        /// </summary>
        /// <param name="expected">The expected object reference</param><param name="actual">The actual object reference</param>
        public SameException(object expected, object actual)
            : base(expected, actual, "Assert.Same() Failure", true)
        {
        }
    }

    /// <summary>
    /// Exception thrown when the collection did not contain exactly one element.
    /// 
    /// </summary>
    public class SingleException : AssertException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Xunit.Sdk.SingleException"/> class.
        /// 
        /// </summary>
        /// <param name="count">The numbers of items in the collection.</param>
        public SingleException(int count)
            : base(string.Format("The collection contained {0} elements instead of 1.", (object)count))
        {
        }

        public SingleException(int count, object expected)
            : base(string.Format("The collection contained {0} instances of '{1}' instead of 1.", (object)count, expected))
        {
        }
    }
}


#endif