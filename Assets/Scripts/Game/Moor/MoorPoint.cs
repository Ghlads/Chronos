using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class MoorPoint : MonoBehaviour
    {
        [SerializeField] private UnityEvent m_onMoorableEntered;
        [SerializeField] private Transform m_targetTransform;

        private void OnTriggerEnter2D( Collider2D collider )
        {
            if ( !collider.TryGetComponent( out MoorableComponent moorable ) )
            {
                return;
            }

            if ( collider.TryGetComponent( out Rigidbody2D rigidbody ) )
            {
                rigidbody.linearVelocity = Vector2.zero;
            }

            moorable.Moor( m_targetTransform );
            m_onMoorableEntered.Invoke();
        }
    }
}
