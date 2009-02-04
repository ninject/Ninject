using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Activation.Constraints;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Syntax;

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
			return root.Get<T>(m => m.Name == name, parameters);
		}

		public static T Get<T>(this IResolutionRoot root, Func<IBindingMetadata, bool> predicate, params IParameter[] parameters)
		{
			return (T)root.Resolve(typeof(T), new[] { new PredicateConstraint(predicate) }, parameters).Select(ctx => ctx.Resolve()).FirstOrDefault();
		}

		public static IEnumerable<T> GetAll<T>(this IResolutionRoot root, params IParameter[] parameters)
		{
			return root.Resolve(typeof(T), new IConstraint[0], parameters).Select(ctx => ctx.Resolve()).Cast<T>();
		}

		public static IEnumerable<T> GetAll<T>(this IResolutionRoot root, string name, params IParameter[] parameters)
		{
			return root.GetAll<T>(m => m.Name == name, parameters);
		}

		public static IEnumerable<T> GetAll<T>(this IResolutionRoot root, Func<IBindingMetadata, bool> predicate, params IParameter[] parameters)
		{
			return root.Resolve(typeof(T), new[] { new PredicateConstraint(predicate) }, parameters).Select(ctx => ctx.Resolve()).Cast<T>();
		}

		public static object Get(this IResolutionRoot root, Type type, params IParameter[] parameters)
		{
			return root.Resolve(type, new IConstraint[0], parameters).Select(ctx => ctx.Resolve()).FirstOrDefault();
		}

		public static object Get(this IResolutionRoot root, Type type, string name, params IParameter[] parameters)
		{
			return root.Get(type, m => m.Name == name, parameters);
		}

		public static object Get(this IResolutionRoot root, Type type, Func<IBindingMetadata, bool> predicate, params IParameter[] parameters)
		{
			return root.Resolve(type, new[] { new PredicateConstraint(predicate) }, parameters).Select(ctx => ctx.Resolve()).FirstOrDefault();
		}

		public static IEnumerable<object> GetAll(this IResolutionRoot root, Type type, params IParameter[] parameters)
		{
			return root.Resolve(type, new IConstraint[0], parameters).Select(ctx => ctx.Resolve());
		}

		public static IEnumerable<object> GetAll(this IResolutionRoot root, Type type, string name, params IParameter[] parameters)
		{
			return root.GetAll(type, m => m.Name == name, parameters);
		}

		public static IEnumerable<object> GetAll(this IResolutionRoot root, Type type, Func<IBindingMetadata, bool> predicate, params IParameter[] parameters)
		{
			return root.Resolve(type, new[] { new PredicateConstraint(predicate) }, parameters).Select(ctx => ctx.Resolve());
		}
	}
}
