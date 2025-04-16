using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Scriptable
{
    public enum SVReferenceMode
    {
        Value = 1,
        Variable = 2,
        Injector = 3,
    }

    [Serializable]
    public class ScriptableVariableReference<T, U, V> where U : ScriptableVariable<T> where V : RuntimeVariableInjector<T, U>, IGenericScriptable
    {
        [SerializeField] private T m_value;
        [SerializeField] private U m_variable;
        [SerializeField] private V m_injector;

        [SerializeField] private SVReferenceMode m_referenceMode = SVReferenceMode.Variable;

        private ScriptableVariable<T>.ValueChangeDelegate m_valueChanged;

        public event ScriptableVariable<T>.ValueChangeDelegate OnValueChanged
        {
            add
            {
                switch ( m_referenceMode )
                {
                    case SVReferenceMode.Variable:
                        m_variable.OnValueChanged += value;
                        break;
                    case SVReferenceMode.Injector:
                        m_injector.OnValueChanged += value;
                        break;
                    case SVReferenceMode.Value:
                    default:
                        m_valueChanged += value;
                        break;
                }
            }
            remove
            {
                switch ( m_referenceMode )
                {
                    case SVReferenceMode.Variable:
                        m_variable.OnValueChanged -= value;
                        break;
                    case SVReferenceMode.Injector:
                        m_injector.OnValueChanged -= value;
                        break;
                    case SVReferenceMode.Value:
                    default:
                        m_valueChanged -= value;
                        break;
                }
            }
        }


        public T Value
        {
            get
            {
                return m_referenceMode switch
                {
                    SVReferenceMode.Value => m_value,
                    SVReferenceMode.Variable => m_variable,
                    SVReferenceMode.Injector => m_injector.Value,
                    _ => m_value,
                };
            }
            set
            {
                switch ( m_referenceMode )
                {
                    case SVReferenceMode.Variable:
                        m_variable.Value = value;
                        break;
                    case SVReferenceMode.Injector:
                        m_injector.Value = value;
                        break;
                    case SVReferenceMode.Value:
                    default:
                        if ( !EqualityComparer<T>.Default.Equals( m_value, value ) )
                        {
                            m_value = value;
                            m_valueChanged?.Invoke( m_value );
                        }
                        break;


                }
            }

        }
    }
}