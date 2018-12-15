using System;
using System.Threading;

namespace Ninject.Tests.Fakes
{
    public class Startable : MarshalByRefObject, IStartable
    {
        private int _startCount;
        private int _stopCount;

        public int StartCount
        {
            get { return _startCount; }
        }

        public int StopCount
        {
            get { return _stopCount; }
        }

        public void Start()
        {
            Interlocked.Increment(ref _startCount);
        }

        public void Stop()
        {
            Interlocked.Increment(ref _stopCount);
        }
    }
}
