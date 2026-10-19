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
            LightCarryable aimed = AimedItem();
            UpdateHighlight(aimed);
            if (Mouse.current == null)
                return;
            if (Mouse.current.leftButton.wasPressedThisFrame)
                UseHand(LimbType.LeftArm, aimed);
            if (Mouse.current.rightButton.wasPressedThisFrame)
                UseHand(LimbType.RightArm, aimed);
        }
        private void UseHand(LimbType arm, LightCarryable aimed)
        {
            if (!_limbs.Has(arm))
                return;
            if (arm == LimbType.LeftArm && _left != null)
            {
                Throw(ref _left, ref _leftFollow);
                return;
            }
            if (arm == LimbType.RightArm && _right != null)
            {
                Throw(ref _right, ref _rightFollow);
                return;
            }
            if (aimed == null || aimed.IsHeld)
                return;
            if (arm == LimbType.LeftArm)
                Pickup(aimed, ref _left, ref _leftFollow);
            else
                Pickup(aimed, ref _right, ref _rightFollow);
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
                && (_limbs.Has(LimbType.LeftArm) && _left == null
                    || _limbs.Has(LimbType.RightArm) && _right == null);
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
        }
    }
}