using UnityEngine;

namespace Framework.Scriptable
{
    public struct NullStruct
    {
        public static NullStruct Default = default;
    }


    [CreateAssetMenu( fileName = "ScriptableEvent", menuName = "Scriptable/Event/Primitive/Void" )]
    public class ScriptableEvent : ScriptableEvent<NullStruct>
    {
        public new delegate void Signature();

        public void Raise()
        {
            Raise( NullStruct.Default );
        }
    }


    public abstract class ScriptableEvent<T> : RuntimeScriptableObject, IGenericScriptable
    {
        public delegate void Signature( T value );

        private Signature m_onInvoke;

        [SerializeField] private T m_dummyValue;

        public void Raise( T value )
        {
            m_onInvoke?.Invoke( value );
        }


        public void AddListener( Signature listener )
        {
            m_onInvoke += listener;
        }


        public void RemoveListener( Signature listener )
        {
            m_onInvoke -= listener;
        }


        private void RemoveAllListener()
        {
            m_onInvoke = null;
        }


        public override void RuntimeReset()
        {
            RemoveAllListener();
        }
    }
}