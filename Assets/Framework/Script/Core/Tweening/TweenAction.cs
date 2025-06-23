using UnityEngine;

namespace Framework.Core
{
    [RequireComponent( typeof( Tweening ) )]
    public abstract class TweenAction : MonoBehaviour
    {
        [Header( "TweenAction" )]
        [SerializeField] private bool m_clampT = false;
        [Header( "Debug" )]
        [SerializeField] private bool m_debug = false;

        public void Tween( float t )
        {
            if ( m_debug )
            {
                Debug.Log( $"[Tween] Debug : [{this.GetType()}] : T = {t}" );
            }

            InternalTween( m_clampT ? Mathf.Clamp01( t ) : t );
        }

        protected abstract void InternalTween( float t );
    }
}
