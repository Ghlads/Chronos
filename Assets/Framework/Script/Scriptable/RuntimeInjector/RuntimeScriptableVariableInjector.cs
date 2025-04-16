using UnityEngine;


namespace Framework.Scriptable
{
    public abstract class RuntimeVariableInjector<T, U> : MonoBehaviour where U : ScriptableVariable<T>, IGenericScriptable
    {
        [SerializeField] private U m_templateVariable;

        private U m_instanceVariable = null;

        public event ScriptableVariable<T>.ValueChangeDelegate OnValueChanged
        {
            add
            {
                if ( m_instanceVariable != null )
                {
                    m_instanceVariable.OnValueChanged += value;
                }
            }
            remove
            {
                if ( m_instanceVariable != null )
                {
                    m_instanceVariable.OnValueChanged -= value;
                }
            }
        }


        public T Value
        {
            get
            {
                return GetOrInstanciateVariable().Value;
            }
            set
            {
                GetOrInstanciateVariable().Value = value;
            }
        }


        private U GetOrInstanciateVariable()
        {
            if ( m_templateVariable == null )
            {
                return null;
            }

            if ( m_instanceVariable == null )
            {
                m_instanceVariable = Instantiate( m_templateVariable );
            }

            return m_instanceVariable;
        }

    }
}
