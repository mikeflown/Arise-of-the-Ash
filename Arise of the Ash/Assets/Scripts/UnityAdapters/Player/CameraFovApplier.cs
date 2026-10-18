using Core.Settings;
using UnityEngine;

namespace UnityAdapters.Player
{
    public class CameraFovApplier : MonoBehaviour
    {
        private void Awake()
        {
            GameSettings.Load();
            if (TryGetComponent<Camera>(out var camera))
                camera.fieldOfView = GameSettings.Fov;
        }
    }
}