#if !NO_REMOTING

using System;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Ninject.Tests.Infrastructure
{
    public class RemotingClient : IDisposable
    {
        private TcpClientChannel _tcpChannel;
        private UrlAttribute[] _urls;

        public RemotingClient()
        {
            _tcpChannel = new TcpClientChannel("client", null);
            ChannelServices.RegisterChannel(_tcpChannel, false);
            _urls = new[] { new UrlAttribute("tcp://localhost:1234/Ninject") };
        }

        public T GetService<T>()
        {
            var handle = Activator.CreateInstance(typeof(T).Assembly.FullName, typeof(T).FullName, _urls);
            return (T) handle.Unwrap();
        }

        public void Dispose()
        {
            if (_tcpChannel != null)
            {
                ChannelServices.UnregisterChannel(_tcpChannel);
            }
        }
    }
}

#endif // !NO_REMOTING