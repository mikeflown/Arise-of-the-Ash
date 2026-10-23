using Configs;
using Core.Player;
using UnityAdapters.Player;
using UnityEngine;

namespace UnityAdapters.Interaction
{
    public class PlayerHeavyDragHandler : MonoBehaviour
    {
        [SerializeField] private MovementConfig _config;
        [SerializeField] private PlayerInputReader _inputReader;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private LayerMask _heavyLayer;
        [SerializeField] private float _detectDistance = 3.5f;
        private PlayerLimbs _limbs;
        private CharacterController _controller;
        private HeavyDraggable _current;
        private bool _isDragging;
        public bool IsDragging => _isDragging;
        public event System.Action OnDragStarted;
        public event System.Action OnDragEnded;
        public void Init(PlayerLimbs limbs)
        {
            _limbs = limbs;
            _controller = GetComponent<CharacterController>();
            if (_cameraTransform == null && Camera.main != null)
                _cameraTransform = Camera.main.transform;
        }
        private void Update()
        {
            if (_limbs == null || _config == null || _inputReader == null)
                return;
            if (!_limbs.HasBothArms)
            {
                if (_isDragging)
                    StopDrag();
                return;
            }
            if (_isDragging)
            {
                if (!_inputReader.Current.InteractHeld || !IsStillInRange())
                    StopDrag();
                else
                    TickDrag();
                return;
            }
            if (_inputReader.Current.InteractPressed)
                TryStartDrag();
        }
        private void TryStartDrag()
        {
            Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, _detectDistance, _heavyLayer, QueryTriggerInteraction.Ignore))
                return;
            HeavyDraggable heavy = hit.collider.GetComponentInParent<HeavyDraggable>();
            if (heavy == null || !heavy.CanInteract(_limbs))
                return;
            _current = heavy;
            _current.BeginDrag();
            _isDragging = true;
            _cameraController?.SetLookEnabled(false);
            OnDragStarted?.Invoke();
        }
        private void TickDrag()
        {
            Vector3 moveInput = new Vector3(_inputReader.Current.Move.x, 0f, _inputReader.Current.Move.y);
            if (moveInput.sqrMagnitude > 0.01f)
            {
                Vector3 worldDir = transform.TransformDirection(moveInput.normalized);
                worldDir.y = 0f;
                Vector3 playerMove = worldDir * (_config.HeavyDragSpeed * 0.7f);
                _controller.Move(playerMove * Time.deltaTime);
                _current.ApplyDragForce(worldDir, _config.HeavyDragSpeed);
            }
            else
            {
                _current.ApplyDragForce(Vector3.zero, 0f);
            }
        }
        private bool IsStillInRange()
        {
            if (_current == null)
                return false;
            float dist = Vector3.Distance(transform.position, _current.transform.position);
            return dist <= _config.HeavyDragMaxDistance;
        }
        private void StopDrag()
        {
            if (!_isDragging)
                return;
            _current?.EndDrag();
            _current = null;
            _isDragging = false;
            _cameraController?.SetLookEnabled(true);
            OnDragEnded?.Invoke();
        }
        private void OnDisable()
        {
            StopDrag();
        }
    }
}