using Configs;
using UnityEngine;

namespace UnityAdapters.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerSlopeSlide : MonoBehaviour
    {
        [SerializeField] private MovementConfig _config;
        [SerializeField] private PlayerMantleHandler _mantle;
        [SerializeField] private float _slideMultiplier = 1.25f;
        [SerializeField] private float _downForce = 12f;
        private CharacterController _controller;
        private Vector3 _hitNormal = Vector3.up;
        private bool _touchingSlope;
        private float _defaultStepOffset;
        private bool _slideEnabled = true;
        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _defaultStepOffset = _controller.stepOffset;
            if (_mantle == null)
                _mantle = GetComponent<PlayerMantleHandler>();
        }
        public void SetSlideEnabled(bool enabled)
        {
            _slideEnabled = enabled;
            if (!enabled)
            {
                _touchingSlope = false;
                if (_controller != null)
                    _controller.stepOffset = _defaultStepOffset;
            }
        }
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (!enabled || !_slideEnabled)
                return;
            if (_mantle != null && _mantle.IsMantling)
                return;
            if (hit.normal.y >= 0.55f)
            {
                _touchingSlope = false;
                return;
            }
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle <= GetMaxSlope())
            {
                _touchingSlope = false;
                return;
            }
            _touchingSlope = true;
            _hitNormal = hit.normal;
        }
        private void Update()
        {
            if (!enabled || !_slideEnabled)
                return;
            if (_config == null || _controller == null)
                return;
            if (_mantle != null && _mantle.IsMantling)
            {
                _touchingSlope = false;
                _controller.stepOffset = _defaultStepOffset;
                return;
            }
            if (!_touchingSlope)
            {
                _controller.stepOffset = _defaultStepOffset;
                return;
            }
            _controller.stepOffset = 0f;
            Vector3 slide = Vector3.ProjectOnPlane(Vector3.down, _hitNormal);
            if (slide.sqrMagnitude < 0.0001f)
                slide = Vector3.down;
            else
                slide.Normalize();
            float speed = _config.SlopeSlideSpeed * _slideMultiplier;
            Vector3 motion = slide * speed + Vector3.down * _downForce;
            _controller.Move(motion * Time.deltaTime);
            _touchingSlope = false;
        }
        private float GetMaxSlope()
        {
            return _config != null ? _config.MaxWalkableSlope : 40f;
        }
    }
}