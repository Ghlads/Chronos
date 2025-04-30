using System.Collections.Generic;
using UnityEngine;

namespace Framework.Scriptable
{
    public abstract class ScriptableVariable<T> : RuntimeScriptableObject, IVariable<T>
    {
        public event IEventBase.Raw OnChangeHappen;

        public event IRaiseableEvent<T>.Signature OnValueChanged;

        [SerializeField] private T m_defaultValue;

        [SerializeField] private T m_value;

        public T Value
        {
            get => m_value;
            set
            {
                if ( EqualityComparer<T>.Default.Equals( m_value, value ) )
                {
                    return;
                }

                m_value = value;
                OnValueChanged?.Invoke( m_value );
                OnChangeHappen?.Invoke();
            }
        }
        

        public override void RuntimeReset()
        {
            OnValueChanged = null;
            OnChangeHappen = null;
            m_value = m_defaultValue;
        }


        public static implicit operator T( ScriptableVariable<T> variable )
        {
            return variable.Value;
        }


        void IEvent<T>.AddListener( IEvent<T>.Signature listener )
        {
            OnValueChanged += listener;
        }


        void IEvent<T>.RemoveListener( IEvent<T>.Signature listener )
        {
            OnValueChanged -= listener;
        }


        void IEventBase.AddListener( IEventBase.Raw listener )
        {
            OnChangeHappen += listener;
        }


        void IEventBase.RemoveListener( IEventBase.Raw listener )
        {
            OnChangeHappen -= listener;
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            OnValueChanged?.Invoke( m_value );
            OnChangeHappen?.Invoke();
        }
#endif // UNITY_EDITOR
    }


    public interface IVariable<T> : IEvent<T>
    {
        public T Value { get; set; }
        public event Raw OnChangeHappen;
        public event Signature OnValueChanged;
    }
}
