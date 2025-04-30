using Framework.Core;
using UnityEngine;

namespace Game
{
    public class RotateTowardsVelocityDirection : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D m_rigidbody;
        [SerializeField] private Vector2 m_forward;
        [SerializeField] private float m_rotationSpeed;
#if UNITY_EDITOR
        [Header( "__Debug__" )]
        [SerializeField][Min( 0 )] private float m_lineLength = 2f;
#endif // UNITY_EDITOR

        private void Update()
        {
            Vector2 velocity = m_rigidbody.linearVelocity;
            if ( velocity.sqrMagnitude > 0.0001f )
            {
                float targetAngle = Mathf.Atan2( velocity.y, velocity.x ) * Mathf.Rad2Deg;
                float forwardOffset = Mathf.Atan2( m_forward.y, m_forward.x ) * Mathf.Rad2Deg;
                targetAngle -= forwardOffset;
                float currentAngle = transform.eulerAngles.z;
                float newAngle = Mathf.LerpAngle( currentAngle, targetAngle, Time.deltaTime * m_rotationSpeed );
                transform.rotation = Quaternion.Euler( 0f, 0f, newAngle );
            }
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            if ( m_rigidbody == null )
            {
                m_rigidbody = GetComponent<Rigidbody2D>();
            }

            m_forward.Normalize();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector2 forwardScaled = m_forward * m_lineLength;
            Gizmos.DrawLine( transform.position, transform.position + forwardScaled.ToVector3() );
        }
#endif // UNITY_EDITOR
    }
}
