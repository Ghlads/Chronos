using UnityEngine;

namespace Framework.Scriptable
{
    public abstract class RuntimeEventInjector<T, U> : MonoBehaviour, IGenericScriptable where U : ScriptableEvent<T>
    {
        [SerializeField] private U m_templateEvent;

        private U m_instanceEvent = null;

        public U Event
        {
            get
            {
                if ( m_instanceEvent == null && m_templateEvent != null )
                {
                    m_instanceEvent = Instantiate( m_templateEvent );
                }

                return m_instanceEvent;
            }
        }
    }
}
