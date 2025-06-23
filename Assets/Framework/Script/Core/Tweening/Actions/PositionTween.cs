using UnityEngine;

namespace Framework.Core
{
    public class PositionTween : TweenAction
    {
        [Header( "Position" )]
        [SerializeField] private Transform m_target;
        [SerializeField] private Vector3 m_startPosition;
        [SerializeField] private Vector3 m_endPosition;
        [SerializeField] private Space m_space;


        protected override void InternalTween( float t )
        {
            m_target.SetPositionInSpace( Vector3.Lerp( m_startPosition, m_endPosition, t ), m_space );
        }


        private void OnDrawGizmosSelected()
        {
            if ( m_target == null )
            {
                return;
            }

            switch ( m_space )
            {
                case Space.Local:
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere( m_target.position + m_startPosition, .2f );
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere( m_target.position + m_endPosition, .2f );
                    break;
                case Space.World:
                default:
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere( m_startPosition, .2f );
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere( m_endPosition, .2f );
                    break;
            }
        }
    }
}
