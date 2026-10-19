using UnityEngine;

namespace UnityAdapters.UI
{
    public class MenuAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioClip _hoverClip;
        [SerializeField] private AudioClip _clickClip;
        [SerializeField] private AudioClip _musicClip;
        private AudioSource _uiSource;
        private void Awake()
        {
            _uiSource = gameObject.AddComponent<AudioSource>();
            _uiSource.playOnAwake = false;
            _uiSource.loop = false;
            _uiSource.spatialBlend = 0f;
            if (_musicSource != null && _musicClip != null)
            {
                _musicSource.clip = _musicClip;
                _musicSource.loop = true;
                _musicSource.playOnAwake = false;
                if (!_musicSource.isPlaying)
                    _musicSource.Play();
            }
        }
        public void PlayHover() => Play(_hoverClip);
        public void PlayClick() => Play(_clickClip);
        private void Play(AudioClip clip)
        {
            if (clip != null)
                _uiSource.PlayOneShot(clip);
        }
    }
}