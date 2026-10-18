using UnityEngine;

namespace UnityAdapters.UI
{
    public class MenuAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioClip _hoverClip;
        [SerializeField] private AudioClip _clickClip;
        [SerializeField] private AudioClip _musicClip;
        private void Awake()
        {
            if (_musicSource != null && _musicClip != null)
            {
                _musicSource.clip = _musicClip;
                _musicSource.loop = true;
                _musicSource.playOnAwake = false;
                _musicSource.Play();
            }
        }
        public void PlayHover()
        {
            if (_sfxSource != null && _hoverClip != null)
                _sfxSource.PlayOneShot(_hoverClip);
        }
        public void PlayClick()
        {
            if (_sfxSource != null && _clickClip != null)
                _sfxSource.PlayOneShot(_clickClip);
        }
    }
}