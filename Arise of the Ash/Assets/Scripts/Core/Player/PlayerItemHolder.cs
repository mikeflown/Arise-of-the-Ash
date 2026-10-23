using Core.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityAdapters.Interaction
{
    public class PlayerItemHolder : MonoBehaviour
    {
        [SerializeField] private Transform _camera;
        [SerializeField] private LayerMask _lightLayer;
        [SerializeField] private float _distance = 3.5f;
        [SerializeField] private float _holdDistance = 1.3f;
        [SerializeField] private float _sideOffset = 0.40f;
        [SerializeField] private float _downOffset = 0.20f;
        [SerializeField] private float _throwForce = 9f;
        [SerializeField] private float _followTime = 0.04f;
        [SerializeField] private float _rotateSpeed = 18f;
        private PlayerLimbs _limbs;
        private LightCarryable _left;
        private LightCarryable _right;
        private Vector3 _leftFollow;
        private Vector3 _rightFollow;
        private InteractableHighlight _highlight;
        public bool IsLeftHandFree => _left == null;
        public bool IsRightHandFree => _right == null;
        public bool AreBothHandsFree => _left == null && _right == null;
        private void Awake()
        {
            if (_camera == null && Camera.main != null)
                _camera = Camera.main.transform;
        }
        public void Init(PlayerLimbs limbs)
        {
            _limbs = limbs;
        }
        private void Update()
        {
            if (_limbs == null || _camera == null || Time.timeScale == 0f)
                return;
            if (Mouse.current == null)
                return;
            LightCarryable aimed = AimedItem();
            UpdateHighlight(aimed);
            UpdateHand(LimbType.LeftArm, Mouse.current.leftButton.isPressed, aimed, ref _left, ref _leftFollow);
            UpdateHand(LimbType.RightArm, Mouse.current.rightButton.isPressed, aimed, ref _right, ref _rightFollow);
        }
        private void UpdateHand(
            LimbType arm, bool buttonHeld, LightCarryable aimed, ref LightCarryable slot, ref Vector3 follow)
        {
            if (!_limbs.Has(arm))
            {
                if (slot != null)
                    Drop(ref slot, ref follow);
                return;
            }
            if (buttonHeld)
            {
                if (slot == null && aimed != null && !aimed.IsHeld)
                    Pickup(aimed, ref slot, ref follow);
            }
            else
            {
                if (slot != null)
                    Throw(ref slot, ref follow);
            }
        }
        private void Pickup(LightCarryable item, ref LightCarryable slot, ref Vector3 follow)
        {
            item.SetHeld(true);
            slot = item;
            follow = Vector3.zero;
        }
        private void Throw(ref LightCarryable slot, ref Vector3 follow)
        {
            LightCarryable item = slot;
            slot = null;
            follow = Vector3.zero;
            item.Throw(_camera.forward * _throwForce + Vector3.up * 0.6f);
        }
        private void Drop(ref LightCarryable slot, ref Vector3 follow)
        {
            LightCarryable item = slot;
            slot = null;
            follow = Vector3.zero;
            item.Throw(Vector3.zero);
        }
        private void LateUpdate()
        {
            if (_camera == null)
                return;
            Hold(_left, -_sideOffset, ref _leftFollow);
            Hold(_right, _sideOffset, ref _rightFollow);
        }
        private void Hold(LightCarryable item, float side, ref Vector3 follow)
        {
            if (item == null)
                return;
            Vector3 target = _camera.position
                + _camera.forward * _holdDistance
                + _camera.right * side
                - _camera.up * _downOffset;
            item.transform.position = Vector3.SmoothDamp(
                item.transform.position, target, ref follow, _followTime);
            item.transform.rotation = Quaternion.Slerp(
                item.transform.rotation, _camera.rotation, Time.deltaTime * _rotateSpeed);
        }
        private LightCarryable AimedItem()
        {
            Ray ray = new Ray(_camera.position, _camera.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, _distance, _lightLayer, QueryTriggerInteraction.Collide))
                return null;
            return hit.collider.GetComponentInParent<LightCarryable>();
        }
        private void UpdateHighlight(LightCarryable aimed)
        {
            InteractableHighlight next = null;
            bool canTake = aimed != null
                && !aimed.IsHeld
                && (
                    (_limbs.Has(LimbType.LeftArm) && _left == null) ||
                    (_limbs.Has(LimbType.RightArm) && _right == null)
                );
            if (canTake)
                next = aimed.GetComponent<InteractableHighlight>();
            if (next == _highlight)
                return;
            _highlight?.SetHighlighted(false);
            _highlight = next;
            _highlight?.SetHighlighted(true);
        }
        private void OnDisable()
        {
            _highlight?.SetHighlighted(false);
            _highlight = null;
            if (_left != null)
                Drop(ref _left, ref _leftFollow);
            if (_right != null)
                Drop(ref _right, ref _rightFollow);
        }
    }
}