using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject.Activation;
using Ninject.Infrastructure;
using Ninject.Resolution;
using Ninject.Syntax;

namespace Ninject.Planning.Targets
{
	public abstract class TargetBase<T> : ITarget
		where T : ICustomAttributeProvider
	{
		public T Site { get; private set; }

		public abstract string Name { get; }
		public abstract Type Type { get; }

		protected TargetBase(T site)
		{
			Site = site;
		}

		public IEnumerable<IConstraint> GetConstraints()
		{
			return Site.GetAttributes<ConstraintAttribute>().Cast<IConstraint>();
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
			return parent.Kernel.Resolve(CreateRequest(service, parent)).Select(ctx => ctx.Resolve());
		}

		private IRequest CreateRequest(Type service, IContext context)
		{
			return new Request(service, this, () => context.GetScope());
		}
	}
}