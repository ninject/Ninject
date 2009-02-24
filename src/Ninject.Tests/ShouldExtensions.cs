using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Ninject.Tests
{
	public static class ShouldExtensions
	{
		public static void ShouldContain(this string self, string str)
		{
			Assert.Contains(str, self);
		}

		public static void ShouldContain(this string self, string str, StringComparison comparison)
		{
			Assert.Contains(str, self, comparison);
		}

		public static void ShouldContain<T>(this IEnumerable<T> series, T item)
		{
			Assert.Contains(item, series);
		}

		public static void ShouldContain<T>(this IEnumerable<T> series, T item, IComparer<T> comparer)
		{
			Assert.Contains(item, series, comparer);
		}

		public static void ShouldNotContain(this string self, string str)
		{
			Assert.DoesNotContain(str, self);
		}

		public static void ShouldNotContain(this string self, string str, StringComparison comparison)
		{
			Assert.DoesNotContain(str, self, comparison);
		}

		public static void ShouldNotContain<T>(this IEnumerable<T> series, T item)
		{
			Assert.DoesNotContain(item, series);
		}

		public static void ShouldNotContain<T>(this IEnumerable<T> series, T item, IComparer<T> comparer)
		{
			Assert.DoesNotContain(item, series, comparer);
		}

		public static void ShouldBeEmpty(this IEnumerable series)
		{
			Assert.Empty(series);
		}

		public static void ShouldNotBeEmpty(this IEnumerable series)
		{
			Assert.NotEmpty(series);
		}

		public static void ShouldBe<T>(this T self, T other)
		{
			Assert.Equal(other, self);
		}

		public static void ShouldBe<T>(this T self, T other, IComparer<T> comparer)
		{
			Assert.Equal(other, self, comparer);
		}

		public static void ShouldBeNull(this object self)
		{
			Assert.Null(self);
		}

		public static void ShouldNotBeNull(this object self)
		{
			Assert.NotNull(self);
		}

		public static void ShouldBeSameAs(this object self, object other)
		{
			Assert.Same(other, self);
		}

		public static void ShouldNotBeSameAs(this object self, object other)
		{
			Assert.NotSame(other, self);
		}

		public static void ShouldBeTrue(this bool self)
		{
			Assert.True(self);
		}

		public static void ShouldBeTrue(this bool self, string message)
		{
			Assert.True(self, message);
		}

		public static void ShouldBeFalse(this bool self)
		{
			Assert.False(self);
		}

		public static void ShouldBeFalse(this bool self, string message)
		{
			Assert.False(self, message);
		}

		public static void ShouldBeInstanceOf<T>(this object self)
		{
			Assert.IsType<T>(self);
		}

		public static void ShouldBeInstanceOf(this object self, Type type)
		{
			Assert.IsType(type, self);
		}

		public static void ShouldNotBeInstanceOf<T>(this object self)
		{
			Assert.IsNotType<T>(self);
		}

		public static void ShouldNotBeInstanceOf(this object self, Type type)
		{
			Assert.IsNotType(type, self);
		}

		public static void ShouldBeThrownBy<T>(this T self, Assert.ThrowsDelegate method)
			where T : Exception
		{
			Assert.Throws<T>(method);
		}
	}
}