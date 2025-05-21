using Framework.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.Scriptable
{
    public class RuntimeSetClearListener : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IBaseRuntimeSet> m_runtimeSet;
        [Space]
        [SerializeField] private UnityEvent m_onSetClear;

        private void OnEnable()
        {
            if ( m_runtimeSet != null )
            {
                m_runtimeSet.Get().OnCleared += ClearHandler;
            }
        }


        private void OnDisable()
        {
            if ( m_runtimeSet != null )
            {
                m_runtimeSet.Get().OnCleared -= ClearHandler;
            }
        }


        private void ClearHandler()
        {
            m_onSetClear.Invoke();
        }
    }
}
