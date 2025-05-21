using Framework.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.Scriptable
{
    public class GenericEventListener : MonoBehaviour, IGenericScriptable
    {
        [SerializeField] private InterfaceReference<IEventBase> m_event;

        [SerializeField] private UnityEvent m_onRaised;


        private void Awake()
        {
            m_event.Get().AddListener( RaiseHandler );
        }


        private void OnDestroy()
        {
            m_event.Get().RemoveListener( RaiseHandler );
        }

        private void RaiseHandler()
        {
            m_onRaised.Invoke();
        }
    }


    public abstract class EventListener<T> : MonoBehaviour, IGenericScriptable 
    {
        [SerializeField] private InterfaceReference<IEvent<T>> m_event;

        [SerializeField] private UnityEvent<T> m_onRaised;

        private void Awake()
        {
            m_event.Get().AddListener( RaiseHandler );
        }


        private void OnDestroy()
        {
            m_event.Get().RemoveListener( RaiseHandler );
        }


        private void RaiseHandler( T value )
        {
            m_onRaised.Invoke( value );
        }
    }
}
