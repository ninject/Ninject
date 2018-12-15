using BenchmarkDotNet.Attributes;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Strategies;
#if !NO_REMOTING
using Ninject.Tests.Infrastructure;
#endif // !NO_REMOTING
using Ninject.Tests.Fakes;

namespace Ninject.Benchmarks.Activation.Strategies
{
    [MemoryDiagnoser]
    [DisassemblyDiagnoser]
    public class DisposableStrategyBenchmark
    {
        private IContext _context;
        private InstanceReference _disposable;
        private InstanceReference _nonDisposable;
        private DisposableStrategy _strategy;
#if !NO_REMOTING
        private RemotingServer _remotingServer;
        private RemotingClient _remotingClient;
        private InstanceReference _disposableTransparentProxy;
        private InstanceReference _nonDisposableTransparentProxy;
#endif // !NO_REMOTING

        [GlobalSetup]
        public void GlobalSetup()
        {
#if !NO_REMOTING
            _remotingServer = new RemotingServer();
            _remotingClient = new RemotingClient();

            _remotingServer.RegisterActivatedService(typeof(Disposable));
            _remotingServer.RegisterActivatedService(typeof(Monk));

            _disposableTransparentProxy = new InstanceReference { Instance = _remotingClient.GetService<Disposable>() };
            _nonDisposableTransparentProxy = new InstanceReference { Instance = _remotingClient.GetService<Monk>() };
#endif // !NO_REMOTING

            _disposable = new InstanceReference { Instance = new Disposable() };
            _nonDisposable = new InstanceReference { Instance = new Monk() };

            _context = new Mock<IContext>(MockBehavior.Strict).Object;
            _strategy = new DisposableStrategy();
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
#if !NO_REMOTING
            _remotingClient?.Dispose();
            _remotingServer?.Dispose();
#endif // !NO_REMOTING
        }

        [Benchmark]
        public void Deactivate_NotTransparantProxy_Disposable()
        {
            _strategy.Deactivate(_context, _disposable);
        }

        [Benchmark]
        public void Deactivate_NotTransparantProxy_NonDisposable()
        {
            _strategy.Deactivate(_context, _nonDisposable);
        }

#if !NO_REMOTING
        [Benchmark]
        public void Deactivate_TransparantProxy_Disposable()
        {
            _strategy.Deactivate(_context, _disposableTransparentProxy);
        }

        [Benchmark]
        public void Deactivate_TransparantProxy_NonDisposable()
        {
            _strategy.Deactivate(_context, _nonDisposableTransparentProxy);
        }
#endif // !NO_REMOTING
    }
}
