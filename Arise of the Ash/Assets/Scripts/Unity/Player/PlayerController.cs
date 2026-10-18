using Configs;
using Core.Player;
using Core.Player.Movement;
using UnityEngine;

namespace Unity.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private MovementConfig _movementConfig;
        [SerializeField] private PlayerInputReader _inputReader;
        private CharacterController _controller;
        private PlayerLimbs _limbs;
        private IMovementStrategy _strategy;
        private MovementStrategyFactory _factory;
        public PlayerLimbs Limbs => _limbs;
        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }
        public void Init(PlayerLimbs limbs)
        {
            _limbs = limbs;
            _factory = new MovementStrategyFactory(_movementConfig);
            _limbs.OnLimbsChanged += RebuildStrategy;
            RebuildStrategy();
        }
        private void RebuildStrategy()
        {
            _strategy = _factory.Create(_limbs);
            Debug.Log($"[PlayerController] Strategy changed. Limbs count: {_limbs.LimbCount}");
        }
        private void Update()
        {
            if (_strategy == null || _inputReader == null) return;
            _strategy.Tick(Time.deltaTime, _inputReader.Current, _controller, transform);
        }
        private void OnDestroy()
        {
            if (_limbs != null)
                _limbs.OnLimbsChanged -= RebuildStrategy;
        }
    }
}