using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    [DisallowMultipleComponent]
    public class Tweening : CoreBehaviour
    {
        [Header( "Tween Settings" )]
        [SerializeField, Min( 0.1f )] private float m_tweenDurationSeconds = 1f;
        [SerializeField] private AnimationCurve m_tweenCurve;
        [SerializeField] private bool m_isLoop = false;
        [SerializeField] private bool m_isPingPong = false;
        [SerializeField][HideInInspector] private List<TweenAction> m_actions = new();


        private Coroutine m_tweeningRoutine = null;

        protected override void CancelOnDisableHandler()
        {
            Stop();
        }


        protected override void ExecuteOnEnableHandler()
        {
            Tween();
        }


        public void Tween()
        {
            if ( m_tweeningRoutine != null )
            {
                Debug.LogError( "[Tween] tween already started. Current will be stopped and a new one started" );
                Stop();
            }

            Func<IEnumerator> tweening = m_isPingPong ? PingPongTweening : WrapTweening;
            m_tweeningRoutine = StartCoroutine( m_isLoop ? LoopTweening( tweening ) : tweening() );

            IEnumerator WrapTweening()
            {
                yield return TweenRoutine();
            }
        }


        public void Stop( float stopAtT = 0f )
        {
            if ( m_tweeningRoutine != null )
            {
                StopCoroutine( m_tweeningRoutine );
                m_tweeningRoutine = null;
            }

            TweenActions( stopAtT );
        }


        public void TweenActions( float t )
        {
            foreach ( TweenAction action in m_actions )
            {
                action.Tween( t );
            }
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
            yield return TweenRoutine( isReversed: false );
            yield return TweenRoutine( isReversed: true );
        }


        private IEnumerator TweenRoutine( bool isReversed = false )
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

                TweenActions( m_tweenCurve.Evaluate( t ) );
                yield return null;
            }
        }


        private void OnValidate()
        {
            gameObject.GetComponents<TweenAction>( m_actions );
        }
    }
}
