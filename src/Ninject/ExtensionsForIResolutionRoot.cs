using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Bindings;
using Ninject.Infrastructure;
using Ninject.Parameters;
using Ninject.Resolution;

namespace Ninject
{
	public static class ExtensionsForIResolutionRoot
	{
		public static T Get<T>(this IResolutionRoot root, params IParameter[] parameters)
		{
			return root.Resolve(typeof(T), new IConstraint[0], parameters).Select(ctx => ctx.Resolve()).Cast<T>().FirstOrDefault();
		}

		public static T Get<T>(this IResolutionRoot root, string name, params IParameter[] parameters)
		{
			return root.Get<T>(b => b.Name == name, parameters);
		}

		public static T Get<T>(this IResolutionRoot root, Func<IBinding, bool> predicate, params IParameter[] parameters)
		{
			return (T)root.Resolve(typeof(T), new[] { new PredicateConstraint(predicate) }, parameters).Select(ctx => ctx.Resolve()).FirstOrDefault();
		}

		public static IEnumerable<T> GetAll<T>(this IResolutionRoot root, params IParameter[] parameters)
		{
			return root.Resolve(typeof(T), new IConstraint[0], parameters).Select(ctx => ctx.Resolve()).Cast<T>();
		}

		public static IEnumerable<T> GetAll<T>(this IResolutionRoot root, string name, params IParameter[] parameters)
		{
			return root.GetAll<T>(b => b.Name == name, parameters);
		}

		public static IEnumerable<T> GetAll<T>(this IResolutionRoot root, Func<IBinding, bool> predicate, params IParameter[] parameters)
		{
			return root.Resolve(typeof(T), new[] { new PredicateConstraint(predicate) }, parameters).Select(ctx => ctx.Resolve()).Cast<T>();
		}

		public static object Get(this IResolutionRoot root, Type type, params IParameter[] parameters)
		{
			return root.Resolve(type, new IConstraint[0], parameters).Select(ctx => ctx.Resolve()).FirstOrDefault();
		}

		public static object Get(this IResolutionRoot root, Type type, string name, params IParameter[] parameters)
		{
			return root.Get(type, b => b.Name == name, parameters);
		}

		public static object Get(this IResolutionRoot root, Type type, Func<IBinding, bool> predicate, params IParameter[] parameters)
		{
			return root.Resolve(type, new[] { new PredicateConstraint(predicate) }, parameters).Select(ctx => ctx.Resolve()).FirstOrDefault();
		}

		public static IEnumerable<object> GetAll(this IResolutionRoot root, Type type, params IParameter[] parameters)
		{
			return root.Resolve(type, new IConstraint[0], parameters).Select(ctx => ctx.Resolve());
		}

		public static IEnumerable<object> GetAll(this IResolutionRoot root, Type type, string name, params IParameter[] parameters)
		{
			return root.GetAll(type, b => b.Name == name, parameters);
		}

		public static IEnumerable<object> GetAll(this IResolutionRoot root, Type type, Func<IBinding, bool> predicate, params IParameter[] parameters)
		{
			return root.Resolve(type, new[] { new PredicateConstraint(predicate) }, parameters).Select(ctx => ctx.Resolve());
		}
	}
}
