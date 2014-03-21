using System;
using System.Diagnostics;

namespace RemotingServer
{
    public class MyService : MarshalByRefObject, IMyService, IDisposable
    {
        public MyService()
        {
        }

        public string Func1()
        {
            return "MyService.Func1() from " + Process.GetCurrentProcess().ProcessName;
        }

        public void Dispose()
        {
            Console.WriteLine("Disposing instance on " + Process.GetCurrentProcess().ProcessName);
        }
    }
}