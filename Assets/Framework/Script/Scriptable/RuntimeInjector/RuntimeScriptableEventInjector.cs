using Framework.Core;
using UnityEngine;

namespace Framework.Scriptable
{

    public abstract class RuntimeEventInjector<T> : MonoBehaviour, IRaiseableEvent<T>
    {
        [SerializeField] private InterfaceReference<IRaiseableEvent<T>, RuntimeScriptableObject> m_templateEvent;

        private IRaiseableEvent<T> m_instanceEvent = null;

        private IRaiseableEvent<T> Event
        {
            get
            {
                if ( m_instanceEvent == null && m_templateEvent.GetRaw() != null )
                {
                    m_instanceEvent = Instantiate( m_templateEvent.GetRaw() ) as IRaiseableEvent<T>;
                }

                return m_instanceEvent;
            }
        }


        public void AddListener( IRaiseableEvent<T>.Signature listener )
        {
            Event.AddListener( listener );
        }


        public void AddListener( IEventBase.Raw listener )
        {
            Event.AddListener( listener );
        }


        public void Raise( T value )
        {
            Event.Raise( value );
        }


        public void RemoveListener( IRaiseableEvent<T>.Signature listener )
        {
            Event.RemoveListener( listener );
        }


        public void RemoveListener( IEventBase.Raw listener )
        {
            Event.RemoveListener( listener );
        }
    }
}
