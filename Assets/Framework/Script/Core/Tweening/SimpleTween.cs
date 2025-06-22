using System;
using System.Collections;
using UnityEngine;

namespace Framework.Core
{
    public class SimpleTween : CoreBehaviour // TODO If more behaviour are required like rotation scale etc, convert this in a component base tween with a master tween and sub tween component
    {
        [Header( "Tween Settings" )]
        [SerializeField, Min(0.1f)] private float m_tweenDurationSeconds = 1f;
        [SerializeField] private AnimationCurve m_tweenCurve;
        [SerializeField] private Vector3 m_startPosition;
        [SerializeField] private Vector3 m_endPosition;
        [SerializeField] private bool m_isLoop = false;
        [SerializeField] private bool m_isPingPong = false;
        [SerializeField] private Space m_positionSpace;

        private Coroutine m_tweenRoutine = null;

        protected override void ExecuteOnEnableHandler()
        {
            Tween();
        }


        protected override void CancelOnDisableHandler()
        {
            StopTween();
        }

        
        public void Tween()
        {
            if (m_tweenRoutine != null )
            {
                Debug.LogError( "[Tween] tween already started. Current will be stopped and a new one started" );
                StopTween();
            }

            Func<IEnumerator> tweening = m_isPingPong ? PingPongTweening : WrapTweening;
            m_tweenRoutine = StartCoroutine( m_isLoop ? LoopTweening( tweening ) : tweening() );

            IEnumerator WrapTweening()
            {
                yield return Tweening();
            }
        }


        public void StopTween()
        {
            if ( m_tweenRoutine != null )
            {
                StopCoroutine( m_tweenRoutine );
                m_tweenRoutine = null;
            }
        }


        private IEnumerator Tweening( bool isReversed = false )
        {
            float startTime = Time.time;
            float endTime = startTime + m_tweenDurationSeconds;
            while ( Time.time < endTime )
            {
                float t;
                if ( !isReversed )
                {
                    t = Mathf.InverseLerp( startTime, endTime, Time.time );
                }
                else
                {
                    t = Mathf.InverseLerp( endTime, startTime, Time.time );
                }

                SetPositionInSpace( Vector3.Lerp( m_startPosition, m_endPosition, m_tweenCurve.Evaluate(t) ) );
                yield return null;
            }

            SetPositionInSpace( m_endPosition );
            yield break;
        }


        private IEnumerator LoopTweening( Func<IEnumerator> TweeningMethod )
        {
            while ( true )
            {
                yield return TweeningMethod();
            }
        }


        private IEnumerator PingPongTweening()
        {
            yield return Tweening( isReversed: false );
            yield return Tweening( isReversed: true );
        }


        private void SetPositionInSpace( Vector3 newPosition )
        {
            switch ( m_positionSpace )
            {
                case Space.Local:
                    transform.localPosition = newPosition;
                    break;
                case Space.World:
                default:
                    transform.position = newPosition;
                    break;
            }
        }
    }
}
