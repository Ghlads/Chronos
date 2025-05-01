using Framework.Core;
using UnityEngine;

namespace Framework.Scriptable
{
    public abstract class AddToRuntimeSet<T> : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IRuntimeSet<T>> m_runtimeSet;
        [SerializeField] private T m_value;
        [SerializeField] private bool m_setOnEnable;


        private void OnEnable()
        {
            if ( m_setOnEnable )
            {
                Add( m_value );
            }
        }


        public void Add( T value )
        {
            if ( m_runtimeSet != null )
            {
                m_runtimeSet.Get().Add( value );
            }
        }
        
    }
}
