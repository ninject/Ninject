#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion
namespace Ninject.Infrastructure
{
    using System;

    /// <summary>
    /// Represents a possible value.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    public class Maybe<T> : IEquatable<Maybe<T>>
    {
        private readonly T _value;
        private readonly bool _hasValue;
        private static readonly Maybe<T> none = new Maybe<T>();

        private Maybe() {}

        /// <summary>
        /// Initializes a new instance of the Maybe&lt;T&gt; class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Maybe(T value)
        {
            _value = value;
            _hasValue = true;
        }

        /// <summary>
        /// Gets the value representing "No Value."
        /// </summary>
        public static Maybe<T> None { get { return none; } }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">if the instance has no value.</exception>
        public T Value
        {
            get
            {
                if(!this.HasValue)
                {
                    throw new InvalidOperationException("Maybe has no value.");
                }
                return _value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has value.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has value; otherwise, <c>false</c>.
        /// </value>
        public bool HasValue
        {
            get { return _hasValue; }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.
        ///                 </param>
        public bool Equals(Maybe<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._hasValue.Equals(this._hasValue) && Equals(other._value, this._value);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
        ///                 </param><exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.
        ///                 </exception><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || Equals(obj as Maybe<T>);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                return this._hasValue.GetHashCode() ^
                    (this._hasValue ? this._value.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Maybe<T> left, Maybe<T> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Maybe<T> left, Maybe<T> right)
        {
            return !Equals(left, right);
        }
    }
}
