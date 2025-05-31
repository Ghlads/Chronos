using UnityEngine;
using UnityEngine.Audio;

namespace Framework.Core
{
    public class AudioMixerFloatSetter : MonoBehaviour
    {
        [SerializeField] private AudioMixer m_mixer;
        [SerializeField] private string m_property;
        [SerializeField] private float m_value;
        [SerializeField] private bool m_isLinearToDecibel = true;

        public void Set()
        {
            Set( m_value );
        }


        public void Set( float value )
        {
            if ( m_isLinearToDecibel )
            {
                value = LinearToDecibel( value );
            }

            m_mixer.SetFloat( m_property, value );
        }


        public static float LinearToDecibel( float value )
        {
            if ( value <= 0.0001f )
            {
                return -80f;
            }

            return Mathf.Log10( value ) * 20f;
        }
    }
}
