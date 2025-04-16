using System;
using UnityEngine;

namespace Framework.Scriptable
{
    public enum SEReferenceMode
    {
        Event = 1,
        Injector = 2,
    }

    [Serializable]
    public class ScriptableEventReference<T, U, V> where U : ScriptableEvent<T> where V : RuntimeEventInjector<T, U>
    {
        [SerializeField] private U m_event;
        [SerializeField] private V m_injector;

        [SerializeField] private SEReferenceMode m_referenceMode = SEReferenceMode.Event;

        private U GetEvent()
        {
            return m_referenceMode switch
            {
                SEReferenceMode.Event => m_event,
                SEReferenceMode.Injector => m_injector.Event,
                _ => null,
            };
        }


        public void Raise( T value )
        {
            GetEvent().Raise( value );
        }


        public void AddListener( ScriptableEvent<T>.Signature listener )
        {
            GetEvent().AddListener( listener );
        }


        public void RemoveListener( ScriptableEvent<T>.Signature listener )
        {
            GetEvent().RemoveListener( listener );
        }
    }
}
