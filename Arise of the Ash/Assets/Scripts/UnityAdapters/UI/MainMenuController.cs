using Core.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnityAdapters.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private string _gameSceneName = "Level_01";
        [SerializeField] private GameObject _mainPanel;
        [SerializeField] private GameObject _settingsPanel;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _quitButton;
        private void Awake()
        {
            GameSettings.Load();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _playButton.onClick.AddListener(Play);
            _settingsButton.onClick.AddListener(OpenSettings);
            _quitButton.onClick.AddListener(Quit);

            ShowMain();
        }
        private void Play()
        {
            GameSettings.Save();
            SceneManager.LoadScene(_gameSceneName);
        }
        private void OpenSettings()
        {
            _mainPanel.SetActive(false);
            _settingsPanel.SetActive(true);
        }
        public void ShowMain()
        {
            _settingsPanel.SetActive(false);
            _mainPanel.SetActive(true);
        }
        private void Quit()
        {
            GameSettings.Save();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}