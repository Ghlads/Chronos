using Framework.Core;
using UnityEngine;

[RequireComponent( typeof( Rigidbody2D ) )]
public class Movement2D : MonoBehaviour
{
    [SerializeField] private Rigidbody2D m_rigidbody;

    [Header( "Settings" )]
    [SerializeField][Min( 1.0f )] private float m_maxSpeed = 15.0f;
    [Space]
    [SerializeField][Min( 1.0f )] private float m_acceleration = 5.0f;
    [SerializeField] private Exponant m_accelerationStrength = Exponant.Linear;
    [Space]
    [SerializeField][Min( 0.1f )] private float m_friction = 2.5f;
    [SerializeField] private Exponant m_frictionStrength = Exponant.Linear;
    [Space]
    [SerializeField][Range( -1, 0 )][Tooltip( "Tolerance to wich movement will consider that it no longer steer to meet desired direction but stop and proceed to the new direction" )] private float m_flipThreshold = -0.8f;
    [SerializeField][Min( 1.0f )] private float m_steeringForce = 3.0f;

    private Vector2 m_movement = Vector2.zero;

    public void SetMovement( Vector2 movement )
    {
        m_movement = movement;
    }


    private void FixedUpdate()
    {
        ApplyFriction();
        ApplyAcceleration();
        ClampSpeed();
    }


    private void ApplyFriction()
    {
        if ( m_rigidbody.linearVelocity.IsNearlyZero() )
        {
            return;
        }

        if ( !m_movement.IsNearlyZero() )
        {
            return;
        }

        float velocityMagnitude = m_rigidbody.linearVelocity.magnitude;
        float slowedVelocityMagnitude = Mathf.Max( 0, velocityMagnitude - ( MathUtils.RaiseExponant( m_friction, m_frictionStrength ) * Time.fixedDeltaTime ) );
        m_rigidbody.linearVelocity = slowedVelocityMagnitude * m_rigidbody.linearVelocity / velocityMagnitude;
    }


    private void ApplyAcceleration()
    {
        if ( m_movement.IsNearlyZero() )
        {
            return;
        }

        Vector2 normalizedMovement = m_movement.normalized;
        if ( m_rigidbody.linearVelocity.IsNearlyZero() )
        {
            m_rigidbody.linearVelocity = MathUtils.RaiseExponant( m_acceleration, m_accelerationStrength ) * Time.fixedDeltaTime * normalizedMovement;
        }
        else
        {
            Vector2 normalizedVelocity = m_rigidbody.linearVelocity.normalized;
            float dot = Vector2.Dot( normalizedVelocity, normalizedMovement );
            if ( dot < m_flipThreshold )
            {
                m_rigidbody.linearVelocity = MathUtils.RaiseExponant( m_acceleration, m_accelerationStrength ) * Time.fixedDeltaTime * normalizedMovement;
            }
            else
            {
                float dotScalarImpact = Mathf.Lerp( 1.0f, m_steeringForce, MathUtils.InverseLerpUnclamp( 1.0f, -1.0f, dot ) );
                m_rigidbody.linearVelocity += dotScalarImpact * Time.fixedDeltaTime * MathUtils.RaiseExponant( m_acceleration, m_accelerationStrength ) * normalizedMovement;
            }
        }


        m_movement = Vector2.zero;
    }


    private void ClampSpeed()
    {
        float velocityMagnitude = m_rigidbody.linearVelocity.magnitude;
        if ( velocityMagnitude > m_maxSpeed )
        {
            m_rigidbody.linearVelocity = m_maxSpeed * ( m_rigidbody.linearVelocity / velocityMagnitude );
        }
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if ( m_rigidbody == null )
        {
            m_rigidbody = GetComponent<Rigidbody2D>();
        }

        if ( m_accelerationStrength == Exponant.Constant )
        {
            Debug.LogWarning( "Movement2D : acceleration can't be constant", this );
            m_accelerationStrength = Exponant.Linear;
        }

        if ( m_frictionStrength == Exponant.Constant )
        {
            Debug.LogWarning( "Movement2D : friction can't be constant", this );
            m_frictionStrength = Exponant.Linear;
        }
    }
#endif
}
