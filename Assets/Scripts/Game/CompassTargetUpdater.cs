using Framework.Core;
using Framework.Scriptable;
using UnityEngine;

namespace Game
{
    public class CompassTargetUpdater : MonoBehaviour
    {
        [SerializeField] private Transform m_origin;
        [SerializeField] private InterfaceReference<IRuntimeSet<GameObject>> m_gameObjectSet;
        [SerializeField] private InterfaceReference<IVariable<GameObject>> m_targetVariable;

        private void Update()
        {
            m_targetVariable.Get().Value = CompassUtils.FindClosest( m_gameObjectSet.Get(), m_origin );
        }
    }
}
