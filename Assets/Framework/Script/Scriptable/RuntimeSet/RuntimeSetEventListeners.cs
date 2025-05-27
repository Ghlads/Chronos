using Framework.Core;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.Scriptable
{
    public abstract class RuntimeSetAddListener<T> : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IRuntimeSet<T>> m_runtimeSet;
        [Space]
        [SerializeField] private UnityEvent<T, int> m_onElementAdded;

        private void OnEnable()
        {
            if ( m_runtimeSet != null )
            {
                m_runtimeSet.Get().OnElementAdded += AddHandler;
            }
        }


        private void OnDisable()
        {
            if ( m_runtimeSet != null )
            {
                m_runtimeSet.Get().OnElementAdded -= AddHandler;
            }
        }


        private void AddHandler( T element, int index )
        {
            m_onElementAdded.Invoke( element, index );
        }
    }


    public abstract class RuntimeSetRemoveListener<T> : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IRuntimeSet<T>> m_runtimeSet;
        [Space]
        [SerializeField] private UnityEvent<T, int> m_onElementRemoved;

        private void OnEnable()
        {
            if ( m_runtimeSet != null )
            {
                m_runtimeSet.Get().OnElementRemoved += RemoveHandler;
            }
        }


        private void OnDisable()
        {
            if ( m_runtimeSet != null )
            {
                m_runtimeSet.Get().OnElementRemoved -= RemoveHandler;
            }
        }


        private void RemoveHandler( T element, int index )
        {
            m_onElementRemoved.Invoke( element, index );
        }
    }


    public abstract class RuntimeSetChangeListener<T> : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IRuntimeSet<T>> m_runtimeSet;
        [Space]
        [SerializeField] private UnityEvent<T, T, int> m_onElementChange;

        private void OnEnable()
        {
            if ( m_runtimeSet != null )
            {
                m_runtimeSet.Get().OnElementChanged += ChangeHandler;
            }
        }


        private void OnDisable()
        {
            if ( m_runtimeSet != null )
            {
                m_runtimeSet.Get().OnElementChanged -= ChangeHandler;
            }
        }


        private void ChangeHandler( T previousElement, T newElement, int index )
        {
            m_onElementChange.Invoke( previousElement, newElement, index );
        }
    }


    public abstract class RuntimeSetEmptyListener<T> : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IRuntimeSet<T>> m_runtimeSet;
        [Space]
        [SerializeField][Tooltip( "Either from a clear or removing the last element" )] private UnityEvent m_onSetEmpty;


        private void Start()
        {
            IRuntimeSet<T> set = m_runtimeSet.Get();
            set.OnCleared += ClearHandler;
            set.OnElementRemoved += RemoveHandler;
        }


        private void RemoveHandler( T newElement, int index )
        {
            if ( m_runtimeSet.Get().Count <= 0 )
            {
                EmptyHandler();
            }
        }


        private void ClearHandler()
        {
            EmptyHandler();
        }
        

        [ContextMenu("Debug_Invoke")]
        public void EmptyHandler()
        {
            m_onSetEmpty.Invoke();
        }
    }
}
