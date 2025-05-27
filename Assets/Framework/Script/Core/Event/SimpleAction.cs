using UnityEngine;
using UnityEngine.Events;

namespace Framework.Core
{
    public class SimpleAction : MonoBehaviour
    {
        [SerializeField] private bool m_invokeOnEnable = false;
        [Space]
        [SerializeField] private UnityEvent m_action;

        private void OnEnable()
        {
            if ( m_invokeOnEnable )
            {
                m_action.Invoke();
            }
        }


        public void Invoke()
        {
            m_action.Invoke();
        }
    }
}
