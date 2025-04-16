using UnityEngine;
using Framework.Scriptable;


[RequireComponent( typeof( Collider2D ) )]
public class Physics2DEventForwarder : MonoBehaviour
{
    private enum EnabledEvent
    {
        None = 0,
        TriggerEnter = 1,
        TriggerExit = 1 << 1,
        TriggerStay = 1 << 2,
        CollisionEnter = 1 << 3,
        CollisionExit = 1 << 4,
        CollisionStay = 1 << 5,
    }

    [Header( "Trigger" )]
    [SerializeField] private Collider2DEvent m_triggerEnterEvent;
    [SerializeField] private Collider2DEvent m_triggerStayEvent;
    [SerializeField] private Collider2DEvent m_triggerExitEvent;
    [Header( "Collision" )]
    [SerializeField] private Collision2DEvent m_collisionEnterEvent;
    [SerializeField] private Collision2DEvent m_collisionStayEvent;
    [SerializeField] private Collision2DEvent m_collisionExitEvent;

    private EnabledEvent m_enabledEvent = EnabledEvent.None;

    private void Awake()
    {
        if ( m_triggerEnterEvent != null )
        {
            m_enabledEvent |= EnabledEvent.TriggerEnter;
        }

        if ( m_triggerStayEvent != null )
        {
            m_enabledEvent |= EnabledEvent.TriggerStay;
        }

        if ( m_triggerExitEvent != null )
        {
            m_enabledEvent |= EnabledEvent.TriggerExit;
        }

        if ( m_collisionEnterEvent != null )
        {
            m_enabledEvent |= EnabledEvent.CollisionEnter;
        }

        if ( m_collisionStayEvent != null )
        {
            m_enabledEvent |= EnabledEvent.CollisionStay;
        }

        if ( m_collisionExitEvent != null )
        {
            m_enabledEvent |= EnabledEvent.CollisionExit;
        }
    }

    private void TryRaise<T>( T value, ScriptableEvent<T> @event, EnabledEvent eventType )
    {
        if ( ( m_enabledEvent & eventType ) == eventType )
        {
            @event.Raise( value );
        }
    }

    private void OnTriggerEnter2D( Collider2D collision )
    {
        TryRaise( collision, m_triggerEnterEvent, EnabledEvent.TriggerEnter );
    }


    private void OnTriggerExit2D( Collider2D collision )
    {
        TryRaise( collision, m_triggerExitEvent, EnabledEvent.TriggerExit );
    }


    private void OnTriggerStay2D( Collider2D collision ) 
    {
        TryRaise( collision, m_triggerStayEvent, EnabledEvent.TriggerStay );
    }


    private void OnCollisionEnter2D( Collision2D collision )
    {
        TryRaise( collision, m_collisionEnterEvent, EnabledEvent.CollisionEnter );
    }


    private void OnCollisionExit2D( Collision2D collision )
    {
        TryRaise( collision, m_collisionExitEvent, EnabledEvent.CollisionExit );
    }


    private void OnCollisionStay2D( Collision2D collision )
    {
        TryRaise( collision, m_collisionStayEvent, EnabledEvent.CollisionStay );
    }
}
