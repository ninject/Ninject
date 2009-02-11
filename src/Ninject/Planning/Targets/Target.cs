using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject.Activation;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Language;
using Ninject.Planning.Bindings;

namespace Ninject.Planning.Targets
{
	public abstract class Target<T> : ITarget
		where T : ICustomAttributeProvider
	{
		public T Site { get; private set; }

		public abstract string Name { get; }
		public abstract Type Type { get; }

		protected Target(T site)
		{
			Site = site;
		}

		public IEnumerable<Func<IBindingMetadata, bool>> GetConstraints()
		{
			return Site.GetAttributes<ConstraintAttribute>().Select(a => new Func<IBindingMetadata, bool>(a.Matches));
		}

		public object ResolveWithin(IContext parent)
		{
			if (Type.IsArray)
			{
				Type service = Type.GetElementType();
				return LinqReflection.ToArraySlow(ResolveInstances(service, parent), service);
			}

			if (Type.IsGenericType)
			{
				Type gtd = Type.GetGenericTypeDefinition();
				Type service = Type.GetGenericArguments()[0];

				if (typeof(ICollection<>).IsAssignableFrom(gtd))
					return LinqReflection.ToListSlow(ResolveInstances(service, parent), service);

				if (typeof(IEnumerable<>).IsAssignableFrom(gtd))
					return LinqReflection.CastSlow(ResolveInstances(service, parent), service);
			}

			return ResolveInstances(Type, parent).FirstOrDefault();
		}

		private IEnumerable<object> ResolveInstances(Type service, IContext parent)
		{
			var request = parent.Request.CreateChild(service, this);
			return parent.Kernel.Resolve(request).Select(hook => hook.Resolve());
		}
	}
}