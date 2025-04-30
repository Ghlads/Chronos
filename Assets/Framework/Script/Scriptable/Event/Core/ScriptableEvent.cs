using Framework.Core;
using UnityEngine;

namespace Framework.Scriptable
{

    [CreateAssetMenu( fileName = "ScriptableEvent", menuName = "Scriptable/Event/Primitive/Void" )]
    public class ScriptableEvent : ScriptableEvent<NullStruct>
    {
        public void Raise()
        {
            Raise( NullStruct.Default );
        }
    }


    public abstract class ScriptableEvent<T> : ScriptableEventBase, IRaiseableEvent<T>
    {
        [SerializeField] private T m_dummyValue;

        public void Raise( T value )
        {
            m_onInvoke?.Invoke( value );
            RaiseBase();
        }


        protected IRaiseableEvent<T>.Signature m_onInvoke;


        public void AddListener( IRaiseableEvent<T>.Signature listener )
        {
            m_onInvoke += listener;
        }


        public void RemoveListener( IRaiseableEvent<T>.Signature listener )
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


    public abstract class ScriptableEventBase : RuntimeScriptableObject, IEventBase
    {
        private IEventBase.Raw m_rawDelegate;
        public void AddListener( IEventBase.Raw listener )
        {
            m_rawDelegate += listener;
        }

        public void RemoveListener( IEventBase.Raw listener )
        {
            m_rawDelegate -= listener;
        }


        protected void RaiseBase()
        {
            m_rawDelegate?.Invoke();
        }
    }


    public interface IEventBase : IGenericScriptable
    {
        public delegate void Raw();
        public void AddListener( Raw listener );
        public void RemoveListener( Raw listener );
    }


    public interface IEvent<T> : IEventBase
    {
        public delegate void Signature( T value );
        public void AddListener( Signature listener );
        public void RemoveListener( Signature listener );
    }


    public interface IRaiseableEvent<T> : IEvent<T>
    {
        public void Raise( T value );
    }
}