using System;

namespace Framework.Core
{
    public class NoopDisposable : IDisposable
    {
        public NoopDisposable() {}
        public void Dispose() {}
    }

    public class DisposableUtils
    {
    
    }
}
