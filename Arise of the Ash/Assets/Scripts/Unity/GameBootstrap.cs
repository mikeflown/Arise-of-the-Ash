using Core.Player;
using Unity.Player;
using UnityEngine;

namespace Unity
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController _playerController;
        [Header("Start State")]
        [SerializeField] private bool _startWithOneLeg = true;
        private PlayerLimbs _playerLimbs;
        private void Awake()
        {
            _playerLimbs = new PlayerLimbs();
            if (_startWithOneLeg)
            {
                _playerLimbs.Add(LimbType.LeftLeg);
            }
            _playerController.Init(_playerLimbs);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _playerLimbs.Add(LimbType.RightLeg);
                Debug.Log("Добавлена правая нога!");
            }
        }
    }
}