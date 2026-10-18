using UnityEngine;

namespace Core.Settings
{
    public static class GameSettings
    {
        private const string VolumeKey = "settings.volume";
        private const string FovKey = "settings.fov";
        public static float Volume { get; private set; } = 1f;
        public static float Fov { get; private set; } = 70f;
        public static void Load()
        {
            Volume = PlayerPrefs.GetFloat(VolumeKey, 1f);
            Fov = PlayerPrefs.GetFloat(FovKey, 70f);
            AudioListener.volume = Volume;
        }
        public static void SetVolume(float value)
        {
            Volume = Mathf.Clamp01(value);
            AudioListener.volume = Volume;
            PlayerPrefs.SetFloat(VolumeKey, Volume);
        }
        public static void SetFov(float value)
        {
            Fov = Mathf.Clamp(value, 50f, 110f);
            PlayerPrefs.SetFloat(FovKey, Fov);
        }
        public static void Save()
        {
            PlayerPrefs.Save();
        }
    }
}