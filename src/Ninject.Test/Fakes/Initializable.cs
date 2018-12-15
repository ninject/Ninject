using System;
using System.Threading;

namespace Ninject.Tests.Fakes
{
    public class Initializable : MarshalByRefObject, IInitializable
    {
        private int _initializeCount;

        public int InitializeCount
        {
            get { return _initializeCount; }
        }

        public void Initialize()
        {
            Interlocked.Increment(ref _initializeCount);
        }
    }
}
