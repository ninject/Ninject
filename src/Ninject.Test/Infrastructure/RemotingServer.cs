#if !NO_REMOTING

using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Ninject.Tests.Infrastructure
{
    public class RemotingServer : IDisposable
    {
        private AppDomain _domain;
        private RemotingHost _remotingHost;

        public RemotingServer()
        {
            _domain = AppDomain.CreateDomain("server", null, new AppDomainSetup { ApplicationBase = AppDomain.CurrentDomain.BaseDirectory });
            _remotingHost = (RemotingHost)_domain.CreateInstanceAndUnwrap(typeof(RemotingHost).Assembly.FullName, typeof(RemotingHost).FullName);
            _remotingHost.Start();
        }

        public void RegisterActivatedService(Type service)
        {
            if (_remotingHost == null)
            {
                throw new InvalidOperationException("Server is not started.");
            }

            _remotingHost.RegisterActivatedService(service);
        }

        public void Dispose()
        {
            _remotingHost?.Dispose();
            if (_domain != null)
            {
                AppDomain.Unload(_domain);
            }
        }

        public class RemotingHost : MarshalByRefObject, IDisposable
        {
            private TcpServerChannel _tcpChannel;

            public void Start()
            {
                _tcpChannel = new TcpServerChannel(1234);
                ChannelServices.RegisterChannel(_tcpChannel, false);
                RemotingConfiguration.ApplicationName = "Ninject";
            }

            public void RegisterActivatedService(Type service)
            {
                RemotingConfiguration.RegisterActivatedServiceType(service);
            }

            public void Dispose()
            {
                _tcpChannel.StopListening(null);
            }
        }
    }
}

#endif // !NO_REMOTING