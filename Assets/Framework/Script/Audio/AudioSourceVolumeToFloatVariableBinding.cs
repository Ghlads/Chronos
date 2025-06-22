using Framework.Core;
using Framework.Scriptable;
using UnityEngine;

namespace Framework
{
    [RequireComponent( typeof( AudioSource ) )]
    public class AudioSourceVolumeToFloatVariableBinding : MonoBehaviour
    {
        [SerializeField][HideInInspector] private AudioSource m_audioSource;
        [SerializeField] private InterfaceReference<IVariable<float>> m_floatVariableRef;
        [SerializeField] private bool m_inverse0To1 = false;

        private IVariable<float> m_floatVariable;

        private void Awake()
        {
            m_floatVariable = m_floatVariableRef.Get();
            m_floatVariable.OnValueChanged += FloatVariableChangeHandler;
        }


        private void OnDestroy()
        {
            m_floatVariable.OnValueChanged -= FloatVariableChangeHandler;
        }


        private void FloatVariableChangeHandler( float value )
        {
            m_audioSource.volume = m_inverse0To1 ? 1 - value : value;
        }


        private void OnValidate()
        {
            if ( m_audioSource == null )
            {
                m_audioSource = GetComponent<AudioSource>();
            }
        }
    }
}
