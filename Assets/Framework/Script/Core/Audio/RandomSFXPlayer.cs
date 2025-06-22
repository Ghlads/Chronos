using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    [Serializable]
    public class SFXInstance
    {
        [SerializeField] private AudioClip m_audioClip;
        [SerializeField] private AudioSource m_audioSource;
        [SerializeField] private Vector2 m_volumeRange;
        [SerializeField] private Vector2 m_pitchRange;
        [SerializeField] private Vector2 m_frequenceRange;
        private WaitForSeconds m_duration = null;

        public AudioClip AudioClip => m_audioClip;
        public AudioSource AudioSource => m_audioSource;
        public Vector2 Volume => m_volumeRange;
        public Vector2 Pitch => m_pitchRange;
        public Vector2 Frequence => m_frequenceRange;
        public WaitForSeconds Duration => m_duration ??= new WaitForSeconds( m_audioClip.length );
    }



    public class RandomSFXPlayer : MonoBehaviour
    {
        [SerializeField] private List<SFXInstance> m_sfx;

        private void Awake()
        {
            foreach ( SFXInstance sfx in m_sfx )
            {
                sfx.AudioSource.clip = sfx.AudioClip;
            }
        }

        private void Start()
        {
            foreach ( SFXInstance sfx in m_sfx )
            {
                StartCoroutine( PlaySFX( sfx ) );
            }
        }


        private IEnumerator PlaySFX( SFXInstance sfx )
        {
            if ( sfx == null )
            {
                yield break;
            }

            while ( true )
            {
                yield return new WaitForSeconds( UnityEngine.Random.Range( sfx.Frequence.x, sfx.Frequence.y ) );
                sfx.AudioSource.volume = Mathf.Clamp01( UnityEngine.Random.Range( sfx.Volume.x, sfx.Volume.y ) );
                sfx.AudioSource.pitch = Mathf.Clamp01( UnityEngine.Random.Range( sfx.Pitch.x, sfx.Pitch.y ) );
                sfx.AudioSource.Play();
                yield return sfx.Duration;
            }
        }
    }
}
