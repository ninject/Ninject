using System;
using Ninject.Activation.Caching;

namespace Ninject.Activation.Hooks
{
	public class ContextResolutionHook : IHook
	{
		public IContext Context { get; private set; }
		public ICache Cache { get; private set; }

		public ContextResolutionHook(IContext context, ICache cache)
		{
			Context = context;
			Cache = cache;
		}

		public object Resolve()
		{
			lock (Context.Binding)
			{
				Context.Instance = Cache.TryGet(Context);

				if (Context.Instance != null)
					return Context.Instance;

				Context.Instance = Context.GetProvider().Create(Context);
				Cache.Remember(Context);

				return Context.Instance;
			}
		}
	}
}