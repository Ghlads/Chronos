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
        [SerializeField][Range( 0, 359 )] private float m_damping;
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

            Vector3 directionToTarget = ( m_target.Value.transform.position - m_origin.Value.transform.position );
            if ( directionToTarget.sqrMagnitude < .0001f ) 
            {
                return;
            }

            directionToTarget.Normalize();
            float targetAngle = MathUtils.GetAngleRadBetween( directionToTarget, Vector3.up, Axis.Z ) * Mathf.Rad2Deg;
            if ( targetAngle < 0f )
            {
                targetAngle += 360f;
            }

            float currentAngle = m_arrow.transform.eulerAngles.z;
            float newAngle = Mathf.LerpAngle( currentAngle, targetAngle, m_damping * Time.deltaTime );
            m_arrow.transform.rotation = Quaternion.Euler( 0, 0, newAngle );
        }
    }
}
