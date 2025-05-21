using Framework.Core;
using UnityEngine;

namespace Game
{
    public class TestComponent : MonoBehaviour
    {
        [SerializeField] private Actyx m_action;
        [SerializeField] private Actyx<bool> m_actionBool;
        [SerializeField] private Actyx<bool,GameObject> m_action2;
        
        [ContextMenu("Test")]
        public void Test()
        {
            m_actionBool.Invoke( true );
        }
    }
}
