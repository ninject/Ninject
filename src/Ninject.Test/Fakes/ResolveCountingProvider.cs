namespace Ninject.Tests.Fakes
{
    using System;

    using Ninject.Activation;

    public class ResolveCountingProvider
    {
        private readonly Func<IContext, IProvider> providerCallback;

        public int Count { get; set; }

        public ResolveCountingProvider(Func<IContext, IProvider> providerCallback)
        {
            this.providerCallback = providerCallback;
        }

        public IProvider Callback(IContext ctx)
        {
            return new ProviderImpl(this, this.providerCallback(ctx));
        }

        private class ProviderImpl : IProvider
        {
            private readonly ResolveCountingProvider parent;
            private readonly IProvider authority;

            public ProviderImpl(ResolveCountingProvider parent, IProvider authority)
            {
                this.parent = parent;
                this.authority = authority;
            }

            public Type Type
            {
                get
                {
                    return this.authority.Type;
                }
            }

            public object Create(IContext context)
            {
                ++this.parent.Count;
                return this.authority.Create(context);
            }
        }
    }
}