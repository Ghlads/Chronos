using Framework.Core;

namespace Framework.Scriptable
{
    public class VoidEventInjector : RuntimeEventInjector<NullStruct> 
    {
        public void Raise()
        {
            Raise( NullStruct.Default );
        }
    }
}
