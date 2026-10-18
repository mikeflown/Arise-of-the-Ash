using Configs;
using Core.Interaction;
using Core.Player;
using Core.Player.Movement;
using UnityAdapters.Interaction;
using UnityEngine;

namespace UnityAdapters.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private MovementConfig _movementConfig;
        [SerializeField] private PlayerInputReader _inputReader;
        [SerializeField] private InteractionDetector _interactionDetector;
        private CharacterController _controller;
        private PlayerLimbs _limbs;
        private IMovementStrategy _strategy;
        private MovementStrategyFactory _factory;
        public PlayerLimbs Limbs => _limbs;
        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }
        public void Init(PlayerLimbs limbs, InteractionDetector detector = null)
        {
            _limbs = limbs;
            _factory = new MovementStrategyFactory(_movementConfig);
            _limbs.OnLimbsChanged += RebuildStrategy;
            RebuildStrategy();
            if (detector != null)
            {
                _interactionDetector = detector;
                var context = new PlayerContext(limbs, transform, _controller);
                _interactionDetector.Init(limbs, context);
            }
        }
        private void RebuildStrategy()
        {
            _strategy = _factory.Create(_limbs);
            Debug.Log($"[PlayerController] Strategy changed. Limbs: {_limbs.LimbCount}");
        }
        private void Update()
        {
            if (_strategy == null || _inputReader == null) return;
            var input = _inputReader.Current;
            _strategy.Tick(Time.deltaTime, in input, _controller, transform);
            if (_interactionDetector != null && input.InteractPressed)
            {
                _interactionDetector.TryInteract();
            }
        }
        private void OnDestroy()
        {
            if (_limbs != null)
                _limbs.OnLimbsChanged -= RebuildStrategy;
        }
    }
}