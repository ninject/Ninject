using System;
using System.Threading;

namespace Ninject.Tests.Fakes
{
    public class Disposable : MarshalByRefObject, IDisposable
    {
        private int _disposeCount;

        public int DisposeCount
        {
            get { return _disposeCount; }
        }

        void IDisposable.Dispose()
        {
            Interlocked.Increment(ref _disposeCount);
            GC.SuppressFinalize(this);
        }
    }
}
