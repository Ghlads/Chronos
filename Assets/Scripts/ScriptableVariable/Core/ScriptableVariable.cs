using System.Collections.Generic;
using UnityEngine;

namespace Scriptable.Variable
{
    public abstract class ScriptableVariable<VariableType> : RuntimeScriptableObject
    {
        public delegate void ValueChangeDelegate( VariableType newValue );

        public event ValueChangeDelegate OnValueChanged;

        [SerializeField] private VariableType m_defaultValue;

        [SerializeField] private VariableType m_value;

        public VariableType Value
        {
            get => m_value;
            set
            {
                if ( EqualityComparer<VariableType>.Default.Equals( m_value, value ) )
                {
                    return;
                }

                m_value = value;
                OnValueChanged?.Invoke( m_value );
            }
        }
        

        public override void RuntimeReset()
        {
            m_value = m_defaultValue;
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            OnValueChanged?.Invoke( m_value );
        }
#endif // UNITY_EDITOR
    }
}
