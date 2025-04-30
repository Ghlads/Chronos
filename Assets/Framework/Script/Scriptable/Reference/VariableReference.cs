using Framework.Core;
using System;
using UnityEngine;

namespace Framework.Scriptable
{
    public enum SVReferenceMode
    {
        Value = 0,
        Reference = 1,
    }

    [Serializable]
    public class VariableReference<T> : IVariable<T>
    {
        [SerializeField] private T m_value;
        [SerializeField] private InterfaceReference<IVariable<T>> m_variableReference;
        [SerializeField] private SVReferenceMode m_variableReferenceType;

        private IEventBase.Raw m_onChangeHappen;
        private IEvent<T>.Signature m_onValueChanged;

        public T Value
        {
            get
            {
                return m_variableReferenceType switch
                {
                    SVReferenceMode.Value => m_value,
                    SVReferenceMode.Reference => m_variableReference.Get().Value,
                    _ => m_value,
                };
            }
            set
            {
                switch ( m_variableReferenceType )
                {
                    case SVReferenceMode.Reference:
                        m_variableReference.Get().Value = value;
                        break;
                    case SVReferenceMode.Value:
                    default:
                        m_value = value;
                        m_onValueChanged?.Invoke( value );
                        m_onChangeHappen?.Invoke();
                        break;

                }
            }
        }


        public event IEventBase.Raw OnChangeHappen
        {
            add
            {
                switch ( m_variableReferenceType )
                {
                    case SVReferenceMode.Reference:
                        m_variableReference.Get().OnChangeHappen += value;
                        break;
                    case SVReferenceMode.Value:
                    default:
                        m_onChangeHappen += value;
                        break;

                }
            }
            remove
            {
                switch ( m_variableReferenceType )
                {
                    case SVReferenceMode.Reference:
                        m_variableReference.Get().OnChangeHappen -= value;
                        break;
                    case SVReferenceMode.Value:
                    default:
                        m_onChangeHappen -= value;
                        break;
                }
            }
        }


        public event IEvent<T>.Signature OnValueChanged
        {
            add
            {
                switch ( m_variableReferenceType )
                {
                    case SVReferenceMode.Reference:
                        m_variableReference.Get().OnValueChanged += value;
                        break;
                    case SVReferenceMode.Value:
                    default:
                        m_onValueChanged += value;
                        break;

                }
            }
            remove
            {
                switch ( m_variableReferenceType )
                {
                    case SVReferenceMode.Reference:
                        m_variableReference.Get().OnValueChanged -= value;
                        break;
                    case SVReferenceMode.Value:
                    default:
                        m_onValueChanged -= value;
                        break;
                }
            }
        }


        public void AddListener( IEvent<T>.Signature listener )
        {
            OnValueChanged += listener;
        }


        public void AddListener( IEventBase.Raw listener )
        {
            OnChangeHappen += listener;
        }


        public void RemoveListener( IEvent<T>.Signature listener )
        {
            OnValueChanged -= listener;
        }


        public void RemoveListener( IEventBase.Raw listener )
        {
            OnChangeHappen -= listener;
        }
    }
}