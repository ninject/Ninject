using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using RemotingServer;

namespace RemotingTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            RemotingConfiguration.RegisterWellKnownClientType(
                typeof(MyService),
                "tcp://localhost:33000/MyServiceUri");

            using (var kernel = new StandardKernel())
            {
                kernel.Bind<MyService>().To<MyService>().InSingletonScope();

                var service = kernel.Get<MyService>();
                Console.WriteLine(service.Func1());                
            }

            Console.ReadKey();
        }
    }
}
