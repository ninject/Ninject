using System;
using Ninject.Activation.Caching;

namespace Ninject.Activation.Hooks
{
	/// <summary>
	/// A hook that resolves a context.
	/// </summary>
	public class ContextResolutionHook : IHook
	{
		/// <summary>
		/// Gets the context that will be resolved by the hook.
		/// </summary>
		public IContext Context { get; private set; }

		/// <summary>
		/// Gets or sets the cache.
		/// </summary>
		public ICache Cache { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ContextResolutionHook"/> class.
		/// </summary>
		/// <param name="context">The context to resolve.</param>
		/// <param name="cache">The cache to use to look up instances for re-use.</param>
		public ContextResolutionHook(IContext context, ICache cache)
		{
			Context = context;
			Cache = cache;
		}

		/// <summary>
		/// Resolves the instance associated with this hook.
		/// </summary>
		/// <returns>The resolved instance.</returns>
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