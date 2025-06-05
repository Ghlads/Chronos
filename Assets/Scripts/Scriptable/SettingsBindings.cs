using Framework.Core;
using Framework.Scriptable;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Audio;

namespace Game
{
    [CreateAssetMenu(fileName = "SettingsBindings", menuName = "Scriptable/Chronos/SettingsBindings")]
    public class SettingsBindings : RuntimeScriptableObject
    {
        public const string MASTER_VOLUME_KEY = "MASTER_VOLUME";
        public const string MUSIC_VOLUME_KEY = "MUSIC_VOLUME";
        public const string SFX_VOLUME_KEY = "SFX_VOLUME";

        [SerializeField] private AudioMixer m_audioMixer;

        private float m_masterVolumeLinear;
        private float m_musicVolumeLinear;
        private float m_sfxVolumeLinear;

        [CreateProperty]
        public float MasterVolume
        {
            get
            {
                return m_masterVolumeLinear;
            }
            set
            {
                if ( m_masterVolumeLinear == value )
                {
                    return;
                }

                m_masterVolumeLinear = value;
                m_audioMixer.SetFloat( MASTER_VOLUME_KEY, AudioUtils.LinearToDb( m_masterVolumeLinear ) );
                PlayerPrefs.SetFloat( MASTER_VOLUME_KEY, m_masterVolumeLinear );
            }
        }


        [CreateProperty]
        public float MusicVolume
        {
            get
            {
                return m_musicVolumeLinear;
            }
            set
            {
                if ( m_musicVolumeLinear == value )
                {
                    return;
                }

                m_musicVolumeLinear = value;
                m_audioMixer.SetFloat( MUSIC_VOLUME_KEY, AudioUtils.LinearToDb( m_musicVolumeLinear ) );
                PlayerPrefs.SetFloat( MUSIC_VOLUME_KEY, m_musicVolumeLinear );
            }
        }


        [CreateProperty]
        public float SfxVolume
        {
            get
            {
                return m_sfxVolumeLinear;
            }
            set
            {
                if ( m_sfxVolumeLinear == value )
                {
                    return;
                }

                m_sfxVolumeLinear = value;
                m_audioMixer.SetFloat( SFX_VOLUME_KEY, AudioUtils.LinearToDb( m_sfxVolumeLinear ) );
                PlayerPrefs.SetFloat( SFX_VOLUME_KEY, m_sfxVolumeLinear );
            }
        }

        public override void RuntimeReset()
        {
            MasterVolume = PlayerPrefs.GetFloat( MASTER_VOLUME_KEY, .25f );
            MusicVolume = PlayerPrefs.GetFloat( MUSIC_VOLUME_KEY, .25f );
            SfxVolume = PlayerPrefs.GetFloat( SFX_VOLUME_KEY, .25f );
        }
    }
}
