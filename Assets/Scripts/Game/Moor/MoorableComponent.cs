using Framework.Core;
using Framework.Scriptable;
using System.Collections;
using TMPro;
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
            Debug.Log( "Test" );
            while ( Vector3.Distance( transform.position, moorPosition ) > m_mooringTolerance )
            {
                Vector3 direction = ( moorPosition - transform.position ).normalized;
                transform.position += direction * m_mooringSpeed * Time.deltaTime;
                yield return null;
            }

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

            Vector2 closestDirection = Vector2.Dot( harborPerpendicularDirection, transform.up ) >= 0 ? harborPerpendicularDirection : -harborPerpendicularDirection;
            float targetAngle = MathUtils.GetAngleRadBetween( closestDirection, Vector3.up, Axis.Z ) * Mathf.Rad2Deg;
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
