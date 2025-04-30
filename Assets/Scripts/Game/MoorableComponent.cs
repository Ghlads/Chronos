using Framework.Core;
using Framework.Scriptable;
using Game.Generated.Scriptable;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class MoorableComponent : MonoBehaviour
    {
        [SerializeField] private VariableReference<bool> m_isMoored;
        [SerializeField] private float m_mooringSpeed = 5f;
        [SerializeField] private float m_mooringTolerance = 0.3f;

        private Coroutine m_mooringRoutine = null;

        public void Moor( Vector3 moorPosition )
        {
            if ( m_isMoored.Value )
            {
                return;
            }

            Assert.IsNull( m_mooringRoutine );
            m_isMoored.Value = true;
            m_mooringRoutine = StartCoroutine( MoorRoutine( moorPosition ) );
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
        }


        private IEnumerator MoorRoutine( Vector3 moorPosition )
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
    }
}
