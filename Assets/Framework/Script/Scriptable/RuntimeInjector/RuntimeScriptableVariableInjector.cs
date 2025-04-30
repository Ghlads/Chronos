using Framework.Core;
using UnityEngine;


namespace Framework.Scriptable
{
    public abstract class RuntimeVariableInjector<T> : MonoBehaviour, IVariable<T> 
    {
        [SerializeField] private InterfaceReference<IVariable<T>, RuntimeScriptableObject> m_templateVariable;

        private IVariable<T> m_instanceVariable = null;

        public event IEventBase.Raw OnChangeHappen
        {
            add
            {
                GetOrInstanciateVariable().OnChangeHappen += value;
            }
            remove
            {
                GetOrInstanciateVariable().OnChangeHappen -= value;
            }
        }

        public event IEvent<T>.Signature OnValueChanged
        {
            add
            {
                GetOrInstanciateVariable().OnValueChanged += value;
            }
            remove
            {
                GetOrInstanciateVariable().OnValueChanged -= value;
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


        private IVariable<T> GetOrInstanciateVariable()
        {
            if ( m_templateVariable == null )
            {
                return null;
            }

            if ( m_instanceVariable == null )
            {
                m_instanceVariable = Instantiate( m_templateVariable.GetRaw() ) as IVariable<T>;
            }

            return m_instanceVariable;
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
    }
}
