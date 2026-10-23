using Core.Player;
using UnityAdapters.Interaction;
using UnityAdapters.Player;
using UnityEngine;

namespace UnityAdapters
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private CameraController _cameraController;
        [Header("Start State")]
        [SerializeField] private bool _startWithOneLeg = true;
        private PlayerLimbs _playerLimbs;
        private void Awake()
        {
            _playerLimbs = new PlayerLimbs();
            if (_startWithOneLeg)
                _playerLimbs.Add(LimbType.LeftLeg);
            var detector = _playerController.GetComponent<InteractionDetector>();
            _playerController.Init(_playerLimbs, detector);
            _cameraController.Init(_playerLimbs);
        }
    }
}