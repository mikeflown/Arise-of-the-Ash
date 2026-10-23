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
        private PlayerHeavyDragHandler _heavyDrag;
        private PlayerMantleHandler _mantle;
        public PlayerLimbs Limbs => _limbs;
        private void Awake()
        {
            CacheComponents();
        }
        public void Init(PlayerLimbs limbs, InteractionDetector detector = null)
        {
            CacheComponents();
            _limbs = limbs;
            _factory = new MovementStrategyFactory(_movementConfig);
            _limbs.OnLimbsChanged += RebuildStrategy;
            RebuildStrategy();
            if (_movementConfig != null && _controller != null)
                _controller.slopeLimit = _movementConfig.MaxWalkableSlope;
            if (detector != null)
            {
                _interactionDetector = detector;
                _interactionDetector.Init(limbs, new PlayerContext(limbs, transform, _controller));
            }
            GetComponent<PlayerItemHolder>()?.Init(limbs);
            GetComponent<LimbDebugToggle>()?.Init(limbs);
            _heavyDrag?.Init(limbs);
            _mantle?.Init(limbs);
        }
        private void CacheComponents()
        {
            if (_controller == null)
                _controller = GetComponent<CharacterController>();
            if (_heavyDrag == null)
                _heavyDrag = GetComponent<PlayerHeavyDragHandler>();
            if (_mantle == null)
                _mantle = GetComponent<PlayerMantleHandler>();
        }
        private void RebuildStrategy()
        {
            _strategy = _factory.Create(_limbs);
        }
        private void Update()
        {
            if (_strategy == null || _inputReader == null)
                return;
            if ((_heavyDrag != null && _heavyDrag.IsDragging) ||
                (_mantle != null && _mantle.IsMantling))
            {
                return;
            }
            var input = _inputReader.Current;
            _strategy.Tick(Time.deltaTime, in input, _controller, transform);
            if (_interactionDetector != null && input.InteractPressed)
                _interactionDetector.TryInteract();
        }
        private void OnDestroy()
        {
            if (_limbs != null)
                _limbs.OnLimbsChanged -= RebuildStrategy;
        }
    }
}