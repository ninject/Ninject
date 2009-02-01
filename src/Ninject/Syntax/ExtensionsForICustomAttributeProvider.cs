using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ninject.Syntax
{
	public static class ExtensionsForICustomAttributeProvider
	{
		public static T GetOneAttribute<T>(this ICustomAttributeProvider member)
			where T : Attribute
		{
			return member.GetAttributes<T>().SingleOrDefault();
		}

		public static object GetOneAttribute(this ICustomAttributeProvider member, Type type)
		{
			return member.GetAttributes(type).SingleOrDefault();
		}

		public static IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider member)
			where T : Attribute
		{
			return member.GetCustomAttributes(typeof(T), true) as T[];
		}

		public static IEnumerable<object> GetAttributes(this ICustomAttributeProvider member, Type type)
		{
			return member.GetCustomAttributes(type, true);
		}

		public static bool HasAttribute<T>(this ICustomAttributeProvider member)
			where T : Attribute
		{
			return member.IsDefined(typeof(T), true);
		}

		public static bool HasAttribute(this ICustomAttributeProvider member, Type type)
		{
			return member.IsDefined(type, true);
		}
	}
}