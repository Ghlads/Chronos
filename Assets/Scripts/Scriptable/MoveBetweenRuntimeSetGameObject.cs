using Framework.Core;
using Framework.Scriptable;
using UnityEngine;

namespace Game
{
    public class MoveBetweenRuntimeSetGameObject : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IRuntimeSet<GameObject>> m_setA;
        [SerializeField] private InterfaceReference<IRuntimeSet<GameObject>> m_setB;

        public void MoveAtoB()
        {
            Move( m_setA.Get(), m_setB.Get() );
        }


        public void MoveBtoA()
        {
            Move( m_setB.Get(), m_setA.Get() );
        }


        private void Move( IRuntimeSet<GameObject> setA, IRuntimeSet<GameObject> setB )
        {
            foreach ( GameObject go in setA )
            {
                setB.Add( go );
            }

            setA.Clear();
        }
    }
}
