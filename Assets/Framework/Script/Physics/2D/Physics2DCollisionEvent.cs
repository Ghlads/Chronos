using Framework.Core;
using Framework.Scriptable;
using UnityEngine;

namespace Framework
{
    [RequireComponent( typeof( Collider2D ) )]
    public class Physics2DCollisionEvent : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IRaiseableEvent<Interaction2DData>> m_collisionEnter;
        [SerializeField] private InterfaceReference<IRaiseableEvent<Interaction2DData>> m_collisionStay;
        [SerializeField] private InterfaceReference<IRaiseableEvent<Interaction2DData>> m_collisionExit;

        private void RaiseEvent( InterfaceReference<IRaiseableEvent<Interaction2DData>> @event, Collision2D collision )
        {
            if ( @event != null )
            {
                @event.Get().Raise( Physics2DUtils.CreateCollisionEvent( gameObject, collision ) );
            }
        }


        private void OnCollisionEnter2D( Collision2D collision )
        {
            RaiseEvent( m_collisionEnter, collision );
        }


        private void OnCollisionStay2D( Collision2D collision )
        {
            RaiseEvent( m_collisionStay, collision );
        }


        private void OnCollisionExit2D( Collision2D collision )
        {
            RaiseEvent( m_collisionExit, collision );
        }
    }
}
