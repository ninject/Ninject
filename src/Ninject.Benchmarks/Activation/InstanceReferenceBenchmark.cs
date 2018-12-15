using BenchmarkDotNet.Attributes;
using Ninject.Activation;
using Ninject.Tests.Fakes;
#if !NO_REMOTING
using Ninject.Tests.Infrastructure;
#endif // !NO_REMOTING
using System;

namespace Ninject.Benchmarks.Activation
{
    [MemoryDiagnoser]
    [DisassemblyDiagnoser]
    public class InstanceReferenceBenchmark
    {
        private InstanceReference _disposable;
        private InstanceReference _nonDisposable;
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
        public void IsInstanceOf_NotTransparantProxy_IsAnInstanceOf()
        {
           if (!_disposable.IsInstanceOf<IDisposable>(out _))
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void IsInstanceOf_NotTransparantProxy_IsNotAnInstanceOf()
        {
            if (_nonDisposable.IsInstanceOf<IDisposable>(out _))
            {
                throw new Exception();
            }
        }

#if !NO_REMOTING
        [Benchmark]
        public void IsInstanceOf_TransparantProxy_IsAnInstanceOf()
        {
            if (!_disposableTransparentProxy.IsInstanceOf<IDisposable>(out _))
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void IsInstanceOf_TransparantProxy_IsNotAnInstanceOf()
        {
            if (_nonDisposableTransparentProxy.IsInstanceOf<IDisposable>(out _))
            {
                throw new Exception();
            }
        }
#endif // !NO_REMOTING
    }
}
