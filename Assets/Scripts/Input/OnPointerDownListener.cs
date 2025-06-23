using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Game
{
    public class OnPointerDownListener : MonoBehaviour
    {
        [SerializeField] private UnityEvent m_onInputAction;

        [Header( "Settings" )]
        [SerializeField] private InputSource m_inputSource;
        [SerializeField] private InputActionPhase m_phase;

        private void Start()
        {
            if ( m_phase == InputActionPhase.Disabled || m_phase == InputActionPhase.Waiting )
            {
                m_phase = InputActionPhase.Started;
            }

            switch ( m_phase )
            {
                case InputActionPhase.Started:
                    m_inputSource.OnPointerDownStarted += InvokeAction;
                    break;
                case InputActionPhase.Performed:
                    m_inputSource.OnPointerDownPerformed += InvokeAction;
                    break;
                case InputActionPhase.Canceled:
                    m_inputSource.OnPointerDownCanceled += InvokeAction;
                    break;
                default:
                    break;
            }
        }


        private void InvokeAction()
        {
            m_inputSource.OnPointerDownStarted -= InvokeAction;
            m_inputSource.OnPointerDownPerformed -= InvokeAction;
            m_inputSource.OnPointerDownCanceled -= InvokeAction;

            m_onInputAction.Invoke();
        }
    }
}
