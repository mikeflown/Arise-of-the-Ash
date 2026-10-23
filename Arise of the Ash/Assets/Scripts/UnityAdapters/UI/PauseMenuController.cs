using Core.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnityAdapters.UI
{
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject _pauseRoot;
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private GameObject _settingsPanel;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private Slider _fovSlider;
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private AudioSource _ambient;
        [SerializeField] private AudioClip _pauseClip;
        private AudioSource _uiSource;
        [SerializeField] private Behaviour[] _disableWhilePaused;
        [SerializeField] private string _menuSceneName = "MainMenu";
        private bool _paused;
        private void Awake()
        {
            GameSettings.Load();
            _volumeSlider.minValue = 0f;
            _volumeSlider.maxValue = 1f;
            _fovSlider.minValue = 50f;
            _fovSlider.maxValue = 110f;
            _volumeSlider.SetValueWithoutNotify(GameSettings.Volume);
            _fovSlider.SetValueWithoutNotify(GameSettings.Fov);
            ApplyFov(GameSettings.Fov);
            _continueButton.onClick.AddListener(Resume);
            _settingsButton.onClick.AddListener(OpenSettings);
            _exitButton.onClick.AddListener(ExitToMenu);
            _backButton.onClick.AddListener(ShowPauseButtons);
            _volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            _fovSlider.onValueChanged.AddListener(OnFovChanged);
            _pauseRoot.SetActive(false);
            _uiSource = gameObject.AddComponent<AudioSource>();
            _uiSource.playOnAwake = false;
            _uiSource.ignoreListenerPause = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        private void Update()
        {
            if (KeyboardEscapePressed())
            {
                if (_paused)
                    Resume();
                else
                    Pause();
            }
        }
        private static bool KeyboardEscapePressed()
        {
            return UnityEngine.InputSystem.Keyboard.current != null
                   && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame;
        }
        private void LateUpdate()
        {
            if (_paused && _ambient != null && _ambient.isPlaying)
                _ambient.Pause();
        }
        private void Pause()
        {
            _paused = true;
            if (_pauseClip != null)
                _uiSource.PlayOneShot(_pauseClip);
            Time.timeScale = 0f;
            if (_ambient != null && _ambient.isPlaying)
                _ambient.Pause();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            foreach (Behaviour behaviour in _disableWhilePaused)
            {
                if (behaviour != null)
                    behaviour.enabled = false;
            }
            _pauseRoot.SetActive(true);
            ShowPauseButtons();
        }
        private void Resume()
        {
            _paused = false;
            GameSettings.Save();
            Time.timeScale = 1f;
            if (_ambient != null)
                _ambient.UnPause();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _pauseRoot.SetActive(false);
            foreach (Behaviour behaviour in _disableWhilePaused)
            {
                if (behaviour != null)
                    behaviour.enabled = true;
            }
        }
        private void OpenSettings()
        {
            _pausePanel.SetActive(false);
            _settingsPanel.SetActive(true);
        }
        private void ShowPauseButtons()
        {
            _settingsPanel.SetActive(false);
            _pausePanel.SetActive(true);
        }
        private void OnVolumeChanged(float value)
        {
            GameSettings.SetVolume(value);
        }
        private void OnFovChanged(float value)
        {
            GameSettings.SetFov(value);
            ApplyFov(value);
        }
        private void ApplyFov(float value)
        {
            if (_playerCamera != null)
                _playerCamera.fieldOfView = value;
        }
        private void ExitToMenu()
        {
            Time.timeScale = 1f;
            if (_ambient != null)
                _ambient.Stop();
            GameSettings.Save();
            SceneManager.LoadScene(_menuSceneName);
        }
    }
}