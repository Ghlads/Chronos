using Framework.Core;
using Framework.Scriptable;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class MoorableComponent : MonoBehaviour
    {
        [SerializeField] private VariableReference<bool> m_isMoored;
        [Header( "Position" )]
        [SerializeField] private float m_mooringSpeed = 5f;
        [SerializeField] private float m_mooringTolerance = 0.3f;
        [Header( "Rotation" )]
        [SerializeField] private float m_rotationSpeed = 3f; 

        private Coroutine m_mooringRoutine = null;
        private Coroutine m_rotationRoutine = null;

        public void Moor( Transform moorTransform )
        {
            if ( m_isMoored.Value )
            {
                return;
            }

            Assert.IsNull( m_mooringRoutine );
            Assert.IsNull( m_rotationRoutine );
            m_isMoored.Value = true;
            m_mooringRoutine = StartCoroutine( MoorPositionRoutine( moorTransform.position ) );
            m_rotationRoutine = StartCoroutine( MoorRotationRoutine( moorTransform.right ) );
        }


        public void Unmoor()
        {
            if ( !m_isMoored.Value )
            {
                return;
            }

            m_isMoored.Value = false;
            if ( m_mooringRoutine != null )
            {
                StopCoroutine( m_mooringRoutine );
                m_mooringRoutine = null;
            }

            if ( m_rotationRoutine != null )
            {
                StopCoroutine( m_rotationRoutine );
                m_rotationRoutine = null;
            }
        }


        private IEnumerator MoorPositionRoutine( Vector3 moorPosition )
        {
            Vector3 direction = ( moorPosition - transform.position ).normalized;
            do
            {
                Vector3 distance = moorPosition - transform.position;
                Vector3 movement = Time.deltaTime * m_mooringSpeed * direction;
                if ( movement.sqrMagnitude >= distance.sqrMagnitude )
                {
                    movement = distance;
                }

                transform.position += movement;
                yield return null;
            } while ( !MathUtils.Vector3Equal( transform.position, moorPosition, m_mooringTolerance ) );

            transform.position = moorPosition;
            m_mooringRoutine = null;
        }


        private IEnumerator MoorRotationRoutine( Vector3 harborPerpendicularDirection )
        {
            if( harborPerpendicularDirection.sqrMagnitude <= 0.0001f )
            {
                Debug.LogWarning( "Invalid direction received" );
                yield break;
            }

            Vector2 closestDirection = Vector2.Dot( harborPerpendicularDirection, Vector2.up ) >= 0 ? harborPerpendicularDirection : -harborPerpendicularDirection;
            float targetAngle = Mathf.Atan2( closestDirection.y, closestDirection.x ) * Mathf.Rad2Deg;
            float forwardOffset = Mathf.Atan2( Vector2.up.y, Vector2.up.x ) * Mathf.Rad2Deg;
            targetAngle -= forwardOffset;
            float startAngle = transform.eulerAngles.z;
            float t = 0;
            do
            {
                t = Mathf.Clamp01( t + ( Time.deltaTime * m_rotationSpeed ) );
                float newAngle = Mathf.LerpAngle( startAngle, targetAngle, t );
                transform.rotation = Quaternion.Euler( 0f, 0f, newAngle );
                yield return null;
            } while ( transform.eulerAngles.z != targetAngle );

            transform.rotation = Quaternion.Euler( 0f, 0f, targetAngle );
            m_rotationRoutine = null;
        }
    }
}
