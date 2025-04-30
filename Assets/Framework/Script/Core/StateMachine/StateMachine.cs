using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Framework.Core
{
    public class StateMachine<T> where T: Enum
    {
        public class State
        {

            private readonly T m_id;

            private readonly Action<State> m_enterStateCallback;
            private readonly Action<State> m_exitStateCallback;
            private readonly Func<State, T> m_stateProcessCallback;

            public T ID => m_id;

            public State( T id, Action<State> enterStateCallback, Action<State> stateExitCallback, Func<State,T> stateProcessCallback )
            {
                m_id = id;
                m_enterStateCallback = enterStateCallback;
                m_exitStateCallback = stateExitCallback;
                m_stateProcessCallback = stateProcessCallback;
            }


            public void EnterState()
            {
                m_enterStateCallback( this );
            }


            public void ExitState()
            {
                m_exitStateCallback( this );
            }


            public T Process()
            {
                return m_stateProcessCallback( this );
            }
        }


        private State[] m_states;

        public StateMachine( State[] states )
        {
            m_states = states;
        }

        private T m_currentState;
        private bool m_hasEnterState;

        private State Current => m_states[Convert.ToInt32( m_currentState )];

        private StateMachine( List<State> states, bool startOnConstruct = true )
        {
            Assert.AreEqual( states.Count, typeof( T ).GetEnumValues().Length, $"[StateMachine::ctor] Please provide a state foreach enum entry [given: {states.Count}, expected: {typeof( T ).GetEnumValues().Length}] " );
            m_states = new State[states.Count];
            foreach( State state in states )
            {
                int index = Convert.ToInt32( state.ID );
                Assert.IsNull( m_states[index], $"[StateMachine::ctor] Please give states with unique ID [duplicate of: {state.ID}] " );
                m_states[index] = state;
            }

            m_hasEnterState = false;
            m_currentState = states[0].ID;
            if ( startOnConstruct )
            {
                Start();
            }
        }


        public void Start()
        {
            if ( m_hasEnterState )
            {
                Debug.LogError( "[StateMachine::Start] already started no need to start" );
                return;
            }

            m_hasEnterState = true;
            Current.EnterState();
        }


        public void Process()
        {
            if ( !m_hasEnterState )
            {
                Debug.LogError( "[StateMachine::Process] not started can't process" );
                return;
            }

            ChangeState( Current.Process() );
        }


        public void ChangeState( T newState )
        {
            if ( m_hasEnterState )
            {
                if ( EqualityComparer<T>.Default.Equals( newState, m_currentState ) )
                {
                    return;
                }

                Current.ExitState();
                m_currentState = newState;
                Current.EnterState();
            }
            else
            {
                m_currentState = newState;
            }
        }


        public void Stop()
        {
            if ( !m_hasEnterState )
            {
                Debug.LogError( "[StateMachine::Stop] already stopped no need to stop" );
                return;
            }

            m_hasEnterState = false;
            Current.ExitState();
        }


        public class Builder
        {
            private readonly List<State> m_states;
            private bool m_startOnConstruct = true;

            public Builder( State startingState )
            {
                m_states = new List<State>(){ startingState };
            }


            public Builder AddState( State newState )
            {
                m_states.Add( newState );
                return this;
            }


            public Builder AddStates( List<State> states )
            {
                m_states.AddRange( states );
                return this;
            }


            public Builder ControlStarting()
            {
                m_startOnConstruct = false;
                return this;
            }


            public StateMachine<T> Build()
            {
                return new StateMachine<T>( m_states, m_startOnConstruct );
            }
        }
    }
}
