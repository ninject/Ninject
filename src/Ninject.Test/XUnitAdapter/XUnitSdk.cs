
#if MSTEST
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Xunit.Sdk
{
    /// <summary>
    /// Guard class, used for guard clauses and argument validation
    /// 
    /// </summary>
    public static class Guard
    {
        /// <summary/>
        public static void ArgumentNotNull(string argName, object argValue)
        {
            if (argValue == null)
                throw new ArgumentNullException(argName);
        }

        /// <summary/>
        public static void ArgumentNotNullOrEmpty(string argName, IEnumerable argValue)
        {
            Guard.ArgumentNotNull(argName, (object)argValue);
            IEnumerator enumerator = argValue.GetEnumerator();
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
            throw new ArgumentException("Argument was empty", argName);
        }

        /// <summary/>
        public static void ArgumentValid(string argName, string message, bool test)
        {
            if (!test)
                throw new ArgumentException(message, argName);
        }
    }

    /// <summary>
    /// Exception thrown when a collection unexpectedly does not contain the expected value.
    /// 
    /// </summary>
    public class ContainsException : AssertException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.ContainsException"/> class.
        /// 
        /// </summary>
        /// <param name="expected">The expected object value</param>
        public ContainsException(object expected)
            : base(string.Format("Assert.Contains() failure: Not found: {0}", expected))
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.ContainsException"/> class.
        /// 
        /// </summary>
        /// <param name="expected">The expected object value</param><param name="actual">The actual value</param>
        public ContainsException(object expected, object actual)
            : base(string.Format("Assert.Contains() failure:{2}Not found: {0}{2}In value:  {1}", expected, actual, (object)Environment.NewLine))
        {
        }
    }

    public class AssertException : Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException
    {
        private readonly string stackTrace;

        /// <summary>
        /// Gets a string representation of the frames on the call stack at the time the current exception was thrown.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// A string that describes the contents of the call stack, with the most recent method call appearing first.
        /// </returns>
        public override string StackTrace
        {
            get
            {
                return AssertException.FilterStackTrace(this.stackTrace ?? base.StackTrace);
            }
        }

        /// <summary>
        /// Gets the user message
        /// 
        /// </summary>
        public string UserMessage { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Xunit.Sdk.AssertException"/> class.
        /// 
        /// </summary>
        public AssertException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Xunit.Sdk.AssertException"/> class.
        /// 
        /// </summary>
        /// <param name="userMessage">The user message to be displayed</param>
        public AssertException(string userMessage)
            : base(userMessage)
        {
            this.UserMessage = userMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Xunit.Sdk.AssertException"/> class.
        /// 
        /// </summary>
        /// <param name="userMessage">The user message to be displayed</param><param name="innerException">The inner exception</param>
        protected AssertException(string userMessage, Exception innerException)
            : base(userMessage, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Xunit.Sdk.AssertException"/> class.
        /// 
        /// </summary>
        /// <param name="userMessage">The user message to be displayed</param><param name="stackTrace">The stack trace to be displayed</param>
        protected AssertException(string userMessage, string stackTrace)
            : base(userMessage)
        {
            this.stackTrace = stackTrace;
        }

        /// <summary>
        /// Filters the stack trace to remove all lines that occur within the testing framework.
        /// 
        /// </summary>
        /// <param name="stackTrace">The original stack trace</param>
        /// <returns>
        /// The filtered stack trace
        /// </returns>
        protected static string FilterStackTrace(string stackTrace)
        {
            if (stackTrace == null)
                return (string)null;
            List<string> list = new List<string>();
            foreach (string str1 in AssertException.SplitLines(stackTrace))
            {
                string str2 = str1.TrimStart(new char[0]);
                if (!str2.StartsWith("at Xunit.Assert.") && !str2.StartsWith("at Xunit.Sdk."))
                    list.Add(str1);
            }
            return string.Join(Environment.NewLine, list.ToArray());
        }

        private static IEnumerable<string> SplitLines(string input)
        {
            while (true)
            {
                int idx = input.IndexOf(Environment.NewLine);
                if (idx >= 0)
                {
                    yield return input.Substring(0, idx);
                    input = input.Substring(idx + Environment.NewLine.Length);
                }
                else
                    break;
            }
            yield return input;
        }
    }

    /// <summary>
    /// Exception thrown when a value is unexpectedly false.
    /// 
    /// </summary>
    public class TrueException : AssertException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.TrueException"/> class.
        /// 
        /// </summary>
        /// <param name="userMessage">The user message to be displayed, or null for the default message</param>
        public TrueException(string userMessage)
            : base(userMessage ?? "Assert.True() Failure")
        {
        }
    }

    /// <summary>
    /// Base class for exceptions that have actual and expected values
    /// 
    /// </summary>
    public class AssertActualExpectedException : AssertException
    {
        private readonly string differencePosition = "";
        private  string actual;
        private  string expected;

        /// <summary>
        /// Gets the actual value.
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
        /// Gets the expected value.
        /// 
        /// </summary>
        public string Expected
        {
            get
            {
                return this.expected;
            }
        }

        /// <summary>
        /// Gets a message that describes the current exception. Includes the expected and actual values.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// The error message that explains the reason for the exception, or an empty string("").
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override string Message
        {
            get
            {
                return string.Format("{0}{4}{1}Expected: {2}{4}Actual:   {3}", (object)base.Message, (object)this.differencePosition, (object)AssertActualExpectedException.FormatMultiLine(this.Expected ?? "(null)"), (object)AssertActualExpectedException.FormatMultiLine(this.Actual ?? "(null)"), (object)Environment.NewLine);
            }
        }

        /// <summary>
        /// Creates a new instance of the <see href="AssertActualExpectedException"/> class.
        /// 
        /// </summary>
        /// <param name="expected">The expected value</param><param name="actual">The actual value</param><param name="userMessage">The user message to be shown</param>
        public AssertActualExpectedException(object expected, object actual, string userMessage)
            : this(expected, actual, userMessage, false)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see href="AssertActualExpectedException"/> class.
        /// 
        /// </summary>
        /// <param name="expected">The expected value</param><param name="actual">The actual value</param><param name="userMessage">The user message to be shown</param><param name="skipPositionCheck">Set to true to skip the check for difference position</param>
        public AssertActualExpectedException(object expected, object actual, string userMessage, bool skipPositionCheck)
            : base(userMessage)
        {
            if (!skipPositionCheck)
            {
                IEnumerable enumerable1 = actual as IEnumerable;
                IEnumerable enumerable2 = expected as IEnumerable;
                if (enumerable1 != null && enumerable2 != null)
                {
                    IEnumerator enumerator1 = enumerable1.GetEnumerator();
                    IEnumerator enumerator2 = enumerable2.GetEnumerator();
                    int num = 0;
                    while (enumerator1.MoveNext() && enumerator2.MoveNext() && object.Equals(enumerator1.Current, enumerator2.Current))
                        ++num;
                    this.differencePosition = "Position: First difference is at position " + (object)num + Environment.NewLine;
                }
            }
            this.actual = actual == null ? (string)null : AssertActualExpectedException.ConvertToString(actual);
            this.expected = expected == null ? (string)null : AssertActualExpectedException.ConvertToString(expected);
            if (actual == null || expected == null || (!(actual.ToString() == expected.ToString()) || actual.GetType() == expected.GetType()))
                return;
            AssertActualExpectedException expectedException1 = this;
            string str1 = expectedException1.actual + string.Format(" ({0})", (object)actual.GetType().GetTypeInfo().FullName);
            expectedException1.actual = str1;
            AssertActualExpectedException expectedException2 = this;
            string str2 = expectedException2.expected + string.Format(" ({0})", (object)expected.GetType().GetTypeInfo().FullName);
            expectedException2.expected = str2;
        }

        private static string ConvertToString(object value)
        {
            Array array = value as Array;
            if (array == null)
                return value.ToString();
            List<string> list = new List<string>();
            foreach (object obj in array)
                list.Add(obj == null ? "(null)" : obj.ToString());
            return value.GetType().FullName + " { " + string.Join(", ", list.ToArray()) + " }";
        }

        private static string FormatMultiLine(string value)
        {
            return value.Replace(Environment.NewLine, Environment.NewLine + "          ");
        }
    }

    /// <summary>
    /// Exception thrown when code unexpectedly fails to throw an exception.
    /// 
    /// </summary>
    public class ThrowsException : AssertActualExpectedException
    {
        private readonly string stackTrace;

        /// <summary>
        /// Gets a string representation of the frames on the call stack at the time the current exception was thrown.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// A string that describes the contents of the call stack, with the most recent method call appearing first.
        /// </returns>
        public override string StackTrace
        {
            get
            {
                return AssertException.FilterStackTrace(this.stackTrace ?? base.StackTrace);
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.ThrowsException"/> class. Call this constructor
        ///             when no exception was thrown.
        /// 
        /// </summary>
        /// <param name="expectedType">The type of the exception that was expected</param>
        public ThrowsException(Type expectedType)
            : this(expectedType, "(No exception was thrown)", (string)null, (string)null)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.ThrowsException"/> class. Call this constructor
        ///             when an exception of the wrong type was thrown.
        /// 
        /// </summary>
        /// <param name="expectedType">The type of the exception that was expected</param><param name="actual">The actual exception that was thrown</param>
        public ThrowsException(Type expectedType, Exception actual)
            : this(expectedType, actual.GetType().GetTypeInfo().FullName, actual.Message, actual.StackTrace)
        {
        }

        private ThrowsException(Type expected, string actual, string actualMessage, string stackTrace)
            : base((object)expected, (object)(actual + (actualMessage == null ? "" : ": " + actualMessage)), "Assert.Throws() Failure")
        {
            this.stackTrace = stackTrace;
        }
    }

    /// <summary>
    /// Exception thrown when a collection unexpectedly contains the expected value.
    /// 
    /// </summary>
    public class DoesNotContainException : AssertException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.DoesNotContainException"/> class.
        /// 
        /// </summary>
        /// <param name="expected">The expected object value</param>
        public DoesNotContainException(object expected)
            : base(string.Format("Assert.DoesNotContain() failure: Found: {0}", expected))
        {
        }
    }

    /// <summary>
    /// Exception thrown when two values are unexpectedly not equal.
    /// 
    /// </summary>
    public class EqualException : AssertActualExpectedException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.EqualException"/> class.
        /// 
        /// </summary>
        /// <param name="expected">The expected object value</param><param name="actual">The actual object value</param>
        public EqualException(object expected, object actual)
            : base(expected, actual, "Assert.Equal() Failure")
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.EqualException"/> class.
        /// 
        /// </summary>
        /// <param name="expected">The expected object value</param><param name="actual">The actual object value</param><param name="skipPositionCheck">Set to true to skip the check for difference position</param>
        public EqualException(object expected, object actual, bool skipPositionCheck)
            : base(expected, actual, "Assert.Equal() Failure", skipPositionCheck)
        {
        }
    }

    /// <summary>
    /// Exception thrown when a collection is unexpectedly not empty.
    /// 
    /// </summary>
    public class EmptyException : AssertException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.EmptyException"/> class.
        /// 
        /// </summary>
        public EmptyException()
            : base("Assert.Empty() failure")
        {
        }
    }

    /// <summary>
    /// Exception thrown when code unexpectedly throws an exception.
    /// 
    /// </summary>
    public class DoesNotThrowException : AssertActualExpectedException
    {
        private readonly string stackTrace;

        /// <summary>
        /// Gets a string representation of the frames on the call stack at the time the current exception was thrown.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// A string that describes the contents of the call stack, with the most recent method call appearing first.
        /// </returns>
        public override string StackTrace
        {
            get
            {
                return AssertException.FilterStackTrace(this.stackTrace ?? base.StackTrace);
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.DoesNotThrowException"/> class.
        /// 
        /// </summary>
        /// <param name="actual">Actual exception</param>
        public DoesNotThrowException(Exception actual)
            : base((object)"(No exception)", (object)(actual.GetType().FullName + (actual.Message == null ? "" : ": " + actual.Message)), "Assert.DoesNotThrow() failure", true)
        {
            this.stackTrace = actual.StackTrace;
        }
    }
    /// <summary>
    /// Exception thrown when a value is unexpectedly true.
    /// 
    /// </summary>
    public class FalseException : AssertException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.FalseException"/> class.
        /// 
        /// </summary>
        /// <param name="userMessage">The user message to be display, or null for the default message</param>
        public FalseException(string userMessage)
            : base(userMessage ?? "Assert.False() Failure")
        {
        }
    }

    /// <summary>
    /// Exception thrown when a value is unexpectedly not in the given range.
    /// 
    /// </summary>
    public class InRangeException : AssertException
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
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.InRangeException"/> class.
        /// 
        /// </summary>
        /// <param name="actual">The actual object value</param><param name="low">The low value of the range</param><param name="high">The high value of the range</param>
        public InRangeException(object actual, object low, object high)
            : base("Assert.InRange() Failure")
        {
            this.low = low == null ? (string)null : low.ToString();
            this.high = high == null ? (string)null : high.ToString();
            this.actual = actual == null ? (string)null : actual.ToString();
        }
    }

    /// <summary>
    /// Exception thrown when the value is unexpectedly not of the given type or a derived type.
    /// 
    /// </summary>
    public class IsAssignableFromException : AssertActualExpectedException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.IsTypeException"/> class.
        /// 
        /// </summary>
        /// <param name="expected">The expected type</param><param name="actual">The actual object value</param>
        public IsAssignableFromException(Type expected, object actual)
            : base((object)expected, actual == null ? (object)(Type)null : (object)actual.GetType(), "Assert.IsAssignableFrom() Failure")
        {
        }
    }

    /// <summary>
    /// Exception thrown when the value is unexpectedly of the exact given type.
    /// 
    /// </summary>
    public class IsNotTypeException : AssertActualExpectedException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.IsNotTypeException"/> class.
        /// 
        /// </summary>
        /// <param name="expected">The expected type</param><param name="actual">The actual object value</param>
        public IsNotTypeException(Type expected, object actual)
            : base((object)expected, actual == null ? (object)(Type)null : (object)actual.GetType(), "Assert.IsNotType() Failure")
        {
        }
    }

    /// <summary>
    /// Exception thrown when the value is unexpectedly not of the exact given type.
    /// 
    /// </summary>
    public class IsTypeException : AssertActualExpectedException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.IsTypeException"/> class.
        /// 
        /// </summary>
        /// <param name="expected">The expected type</param><param name="actual">The actual object value</param>
        public IsTypeException(Type expected, object actual)
            : base((object)expected, actual == null ? (object)(Type)null : (object)actual.GetType(), "Assert.IsType() Failure")
        {
        }
    }
}

#endif
