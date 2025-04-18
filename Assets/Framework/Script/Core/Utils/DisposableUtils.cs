using System;

namespace Framework
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
