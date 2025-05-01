using Framework.Core;
using Framework.Scriptable;
using UnityEngine;

namespace Game
{
    public class CompassOrientationComponent : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IVariable<GameObject>> m_targetVariable;
        [SerializeField] private InterfaceReference<IVariable<GameObject>> m_originVariable;

        [SerializeField] private GameObject m_arrow;
        private IVariable<GameObject> m_target;
        private IVariable<GameObject> m_origin;

        private void Start()
        {
            m_target = m_targetVariable.Get();
            m_origin = m_originVariable.Get();
        }


        private void Update()
        {
            if ( m_target.Value == null || m_origin.Value == null )
            {
                return;
            }

            Vector3 directionToTarget = ( m_target.Value.transform.position - m_origin.Value.transform.position ).normalized;
            float targetAngle = MathUtils.GetAngleRadBetween( directionToTarget, m_arrow.transform.up, Axis.Z ) * Mathf.Rad2Deg;
            m_arrow.transform.rotation = Quaternion.Euler( 0, 0, targetAngle ) * m_arrow.transform.rotation;
        }
    }
}
