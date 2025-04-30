using Framework.Core;
using Framework.Scriptable;
using UnityEngine;

namespace Framework
{
    [RequireComponent( typeof( Collider2D ) )]
    public class Physics2DTriggerEvent : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IRaiseableEvent<Interaction2DData>> m_triggerEnter;
        [SerializeField] private InterfaceReference<IRaiseableEvent<Interaction2DData>> m_triggerStay;
        [SerializeField] private InterfaceReference<IRaiseableEvent<Interaction2DData>> m_triggerExit;

        private void OnTriggerEnter2D( Collider2D collider )
        {
            RaiseEvent( m_triggerEnter, collider );
        }


        private void OnTriggerStay2D( Collider2D collider )
        {
            RaiseEvent( m_triggerStay, collider );
        }


        private void OnTriggerExit2D( Collider2D collider )
        {
            RaiseEvent( m_triggerExit, collider );
        }


        private void RaiseEvent( InterfaceReference<IRaiseableEvent<Interaction2DData>> @event, Collider2D collider )
        {
            if ( @event != null )
            {
                @event.Get().Raise( Physics2DUtils.CreateTriggerEvent( gameObject, collider ) );
            }
        }
    }
}
