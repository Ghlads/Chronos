using UnityEngine;

namespace Framework.Core
{
    public abstract class CoreBehaviour : MonoBehaviour
    {
        [Header( "Core Settings" )]
        [SerializeField] private bool m_executeOnEnable = false;
        [SerializeField] private bool m_cancelOnDisable = false;

        protected virtual void OnEnable()
        {
            if ( m_executeOnEnable )
            {
                ExecuteOnEnableHandler();
            }
        }


        protected abstract void ExecuteOnEnableHandler();


        protected virtual void OnDisable()
        {
            if ( m_cancelOnDisable )
            {
                CancelOnDisableHandler();
            }
        }


        protected abstract void CancelOnDisableHandler();
    }
}
