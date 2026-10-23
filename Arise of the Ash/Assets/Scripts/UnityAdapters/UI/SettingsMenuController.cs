using Core.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UnityAdapters.UI
{
    public class SettingsMenuController : MonoBehaviour
    {
        [SerializeField] private MainMenuController _mainMenu;
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private Slider _fovSlider;
        [SerializeField] private Button _backButton;
        private void Awake()
        {
            GameSettings.Load();
            _volumeSlider.minValue = 0f;
            _volumeSlider.maxValue = 1f;
            _volumeSlider.SetValueWithoutNotify(GameSettings.Volume);
            _fovSlider.minValue = 50f;
            _fovSlider.maxValue = 110f;
            _fovSlider.SetValueWithoutNotify(GameSettings.Fov);
            _volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            _fovSlider.onValueChanged.AddListener(OnFovChanged);
            _backButton.onClick.AddListener(Back);
        }
        private void OnVolumeChanged(float value)
        {
            GameSettings.SetVolume(value);
        }
        private void OnFovChanged(float value)
        {
            GameSettings.SetFov(value);
        }
        private void Back()
        {
            GameSettings.Save();
            _mainMenu.ShowMain();
        }
    }
}