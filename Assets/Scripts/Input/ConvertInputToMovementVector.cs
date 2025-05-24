using Framework.Scriptable.Generated;
using UnityEngine;

namespace Game
{
    [RequireComponent( typeof( Movement2D ) )]
    public class ConvertInputToMovementVector : MonoBehaviour
    {
        [SerializeField] private InputSource m_inputSource;
        [SerializeField] private CameraVariable m_cameraVariable;
        [SerializeField] private Movement2D m_movement;

        private void OnEnable()
        {
            m_inputSource.Enable();
        }


        private void OnDisable()
        {
            m_inputSource.Disable();
        }


        private void Update()
        {
            if ( !m_inputSource.IsPointerDown )
            {
                return;
            }

            Vector3 worldPointerPosition = m_cameraVariable.Value.ScreenToWorldPoint( m_inputSource.PointerPosition );
            worldPointerPosition.z = 0;

            Vector3 worldPosition = transform.position;
            worldPosition.z = 0;

            m_movement.SetMovement( worldPointerPosition - worldPosition );
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if ( m_movement == null )
            {
                m_movement = GetComponent<Movement2D>();
            }
        }
#endif // UNITY_EDITOR
    }
}