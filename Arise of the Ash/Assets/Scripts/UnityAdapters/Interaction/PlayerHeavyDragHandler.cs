using Configs;
using Core.Player;
using UnityAdapters.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityAdapters.Interaction
{
    public class PlayerHeavyDragHandler : MonoBehaviour
    {
        [SerializeField] private MovementConfig _config;
        [SerializeField] private PlayerInputReader _inputReader;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private PlayerItemHolder _itemHolder;
        [SerializeField] private PlayerSlopeSlide _slopeSlide;
        [SerializeField] private LayerMask _heavyLayer;
        [SerializeField] private float _detectDistance = 3.0f;
        [SerializeField] private float _detectRadius = 0.4f;
        private PlayerLimbs _limbs;
        private CharacterController _controller;
        private HeavyDraggable _current;
        private bool _isDragging;
        private Vector3 _offsetPlayerToCar;
        private float _playerY;
        public bool IsDragging => _isDragging;
        public event System.Action OnDragStarted;
        public event System.Action OnDragEnded;
        public void Init(PlayerLimbs limbs)
        {
            _limbs = limbs;
            _controller = GetComponent<CharacterController>();
            if (_cameraTransform == null && Camera.main != null)
                _cameraTransform = Camera.main.transform;
            if (_itemHolder == null)
                _itemHolder = GetComponent<PlayerItemHolder>();
            if (_slopeSlide == null)
                _slopeSlide = GetComponent<PlayerSlopeSlide>();
        }
        private void Update()
        {
            if (_limbs == null || _config == null || Mouse.current == null)
                return;
            bool bothButtonsHeld =
                Mouse.current.leftButton.isPressed &&
                Mouse.current.rightButton.isPressed;
            bool canDrag =
                _limbs.HasBothArms &&
                _itemHolder != null &&
                _itemHolder.AreBothHandsFree;
            if (_isDragging)
            {
                if (!bothButtonsHeld || !canDrag || _current == null)
                    StopDrag();
                else
                    TickDrag();
                return;
            }
            if (bothButtonsHeld && canDrag)
                TryStartDrag();
        }
        private void TryStartDrag()
        {
            if (_cameraTransform == null)
                return;
            HeavyDraggable heavy = FindHeavyInFront();
            if (heavy == null || !heavy.CanInteract(_limbs))
                return;
            _current = heavy;
            _current.BeginDrag(_controller);
            Vector3 carPos = _current.Position;
            Vector3 playerPos = transform.position;
            _offsetPlayerToCar = carPos - playerPos;
            _offsetPlayerToCar.y = 0f;
            _playerY = playerPos.y;
            _isDragging = true;
            _slopeSlide?.SetSlideEnabled(false);
            _cameraController?.SetLookEnabled(false);
            OnDragStarted?.Invoke();
        }
        private void TickDrag()
        {
            if (_inputReader == null || _current == null || _controller == null)
                return;
            Vector2 move = _inputReader.Current.Move;
            Vector3 moveInput = new Vector3(move.x, 0f, move.y);
            if (moveInput.sqrMagnitude < 0.01f)
                return;
            Vector3 worldDir = transform.TransformDirection(moveInput.normalized);
            worldDir.y = 0f;
            worldDir.Normalize();
            float mass = Mathf.Max(1f, _current.Mass);
            float massFactor = Mathf.Clamp(55f / mass, 0.2f, 1.0f);
            float speed = _config.HeavyDragSpeed * massFactor;
            Vector3 delta = worldDir * (speed * Time.deltaTime);
            _current.MoveTogether(delta);
            _controller.Move(new Vector3(delta.x, 0f, delta.z));
            Vector3 desiredPlayer = _current.Position - _offsetPlayerToCar;
            desiredPlayer.y = _playerY;
            Vector3 correct = desiredPlayer - transform.position;
            correct.y = 0f;
            if (correct.sqrMagnitude > 0.0001f)
                _controller.Move(correct);
        }
        private HeavyDraggable FindHeavyInFront()
        {
            Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
            if (Physics.SphereCast(ray, _detectRadius, out RaycastHit hit, _detectDistance, _heavyLayer, QueryTriggerInteraction.Ignore))
            {
                HeavyDraggable h = hit.collider.GetComponentInParent<HeavyDraggable>();
                if (h != null)
                    return h;
            }
            if (Physics.Raycast(ray, out hit, _detectDistance, _heavyLayer, QueryTriggerInteraction.Ignore))
                return hit.collider.GetComponentInParent<HeavyDraggable>();
            return null;
        }
        private void StopDrag()
        {
            if (!_isDragging)
                return;
            _current?.EndDrag();
            _current = null;
            _isDragging = false;
            _slopeSlide?.SetSlideEnabled(true);
            _cameraController?.SetLookEnabled(true);
            OnDragEnded?.Invoke();
        }
        private void OnDisable()
        {
            StopDrag();
        }
    }
}