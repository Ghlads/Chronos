using Framework.Core;
using UnityEngine;

namespace Framework.Scriptable
{
    public class VariableSetter<T> : MonoBehaviour, IGenericScriptable
    {
        [SerializeField] private InterfaceReference<IVariable<T>> m_variable;
        [SerializeField] private T m_value;
        [Space]
        [SerializeField] private bool m_setOnEnable = false;
        

        private void OnEnable()
        { 
            if ( m_setOnEnable )
            {
                SetValueFromField();
            }
        }


        public void SetValue( T value )
        {
            m_variable.Get().Value = value;
        }


        public void SetValueFromField()
        {
            SetValue( m_value );
        }
    }
}
