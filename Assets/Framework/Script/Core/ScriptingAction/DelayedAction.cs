using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.Core
{
    public class DelayedAction : MonoBehaviour
    {
        [SerializeField] private float m_delay;
        [SerializeField] private UnityEvent m_action;

        private WaitForSeconds m_wait;

        private void Awake()
        {
            m_wait = new WaitForSeconds( m_delay );
        }

        public void Invoke()
        {
            StartCoroutine( DelayedInvoke() );
        }


        private IEnumerator DelayedInvoke()
        {
            yield return m_wait;
            m_action.Invoke();
        }
    }
}
