using Framework.Core;
using Framework.Scriptable.Generated;
using UnityEngine;

namespace Game
{
    public class DeadSeaVelocityModifier : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D m_rigidbody;

        [Header( "Constraint" )]
        [SerializeField] private RectVariable m_mapBounds;
        [SerializeField] private FloatVariable m_deadSeaSize;
        [SerializeField] private FloatVariable m_deadSeaDistanceRatio;
        [SerializeField][Range( 0, 1 )] private float m_outOfBoundsDirectionTolerance = .3f;
        [SerializeField] private Exponant m_deadSeaFrictionPower;

        [Header( "Debug" )]
        [SerializeField] private bool m_logModification = false;

        private Rect m_deadSeaBounds;

        private void Start()
        {
             m_deadSeaBounds = m_mapBounds.Value.Resize( m_deadSeaSize.Value ); 
        }


        private void FixedUpdate()
        {
            Vector2 currentPosition = m_rigidbody.position;
            if ( !m_mapBounds.Value.Contains( currentPosition ) )
            {
                Vector2 velocity = m_rigidbody.linearVelocity;
                Vector2 velocityDirection = velocity.normalized;
                Vector2 closetMapEdgePosition = m_mapBounds.Value.Clamp( currentPosition );
                Vector2 currentPosToEdge = closetMapEdgePosition - currentPosition;

                float distanceFromEdge = currentPosToEdge.magnitude;
                m_deadSeaDistanceRatio.Value = Mathf.InverseLerp( 0, m_deadSeaSize, distanceFromEdge );

                Vector2 toMapEdgeDirection = currentPosToEdge / distanceFromEdge;
                if ( Vector2.Dot( velocityDirection, toMapEdgeDirection ) < 1 - m_outOfBoundsDirectionTolerance )
                {
                    velocity *= Mathf.Clamp01( 1 - MathUtils.RaiseExponant( m_deadSeaDistanceRatio.Value, m_deadSeaFrictionPower ) );
                    m_rigidbody.linearVelocity = velocity;
                    if ( !m_deadSeaBounds.Contains( currentPosition ) )
                    {
                        m_deadSeaBounds.Clamp( ref currentPosition );
                        m_rigidbody.position = currentPosition;
                    }
                }

                if ( m_logModification )
                {
                    Debug.Log( @$"Out of bounds : 
                        edge : {closetMapEdgePosition}
                        distance : {distanceFromEdge}
                        ratio : {m_deadSeaDistanceRatio.Value}
                        velocity : {velocity}
                        dot : {Vector2.Dot( velocityDirection, toMapEdgeDirection )}
                        tolerance : {1 - m_outOfBoundsDirectionTolerance}" 
                    );
                }
            }
            else
            {
                m_deadSeaDistanceRatio.Value = 0;
            }
        }

    }
}
