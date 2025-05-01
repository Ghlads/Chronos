using Framework.Core;
using UnityEngine;

namespace Framework.Scriptable
{
    public abstract class RemoveFromRuntimeSet<T> : MonoBehaviour
    {
        public enum Mode
        {
            ByElement,
            ByIndex,
        }

        [SerializeField] private InterfaceReference<IRuntimeSet<T>> m_runtimeSet;
        [SerializeField] private Mode m_removeMode;
        [SerializeField] private T m_element;
        [SerializeField] private int m_index;
        [SerializeField] private bool m_removeOnEnable;

        private void OnEnable()
        {
            if ( m_removeOnEnable )
            {
                switch ( m_removeMode )
                {
                    case Mode.ByElement:
                        RemoveByElement( m_element );
                        break;
                    case Mode.ByIndex:
                        RemoveByIndex( m_index );
                        break;
                }
            }
        }


        public void RemoveByElement( T element )
        {
            m_runtimeSet.Get().Remove( element );
        }


        public void RemoveByIndex( int index )
        {
            m_runtimeSet.Get().RemoveAt( index );
        }
    }
}
