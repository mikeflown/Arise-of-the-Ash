using Configs;
using Core.Player;
using UnityEngine;

namespace UnityAdapters.Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CameraBobConfig _config;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private PlayerInputReader _inputReader;
        [SerializeField] private float _clipPadding = 0.08f;
        [SerializeField] private LayerMask _cameraCollision = ~0;
        private PlayerLimbs _limbs;
        private float _bobTimer;
        private Vector3 _originalLocalPosition;
        private float _currentVerticalAmp;
        private float _currentSideAmp;
        private float _currentFrequency;
        private float _currentSmoothing;
        private float _xRotation;
        public void Init(PlayerLimbs limbs)
        {
            _limbs = limbs;
            _originalLocalPosition = _cameraTransform.localPosition;
            _limbs.OnLimbsChanged += UpdateBobParameters;
            UpdateBobParameters();
        }
        private void UpdateBobParameters()
        {
            if (_limbs == null || _config == null)
                return;
            if (_limbs.HasBothLegs)
            {
                _currentFrequency = _config.TwoLegsWalkFrequency;
                _currentVerticalAmp = _config.TwoLegsWalkVerticalAmplitude;
                _currentSideAmp = _config.TwoLegsWalkSideAmplitude;
                _currentSmoothing = _config.TwoLegsWalkSmoothing;
            }
        }
        private void Update()
        {
            if (_inputReader == null || _config == null || _cameraTransform == null)
                return;
            HandleMouseLook();
            HandleHeadBob();
            PreventCameraClip();
        }
        private void HandleMouseLook()
        {
            Vector2 lookInput = _inputReader.Current.Look * _config.MouseSensitivity;
            _xRotation -= lookInput.y;
            _xRotation = Mathf.Clamp(_xRotation, -80f, 80f);
            transform.Rotate(Vector3.up * lookInput.x);
        }
        private void HandleHeadBob()
        {
            bool isMoving = _inputReader.Current.Move.sqrMagnitude > 0.01f;
            if (_limbs != null && !_limbs.HasBothLegs)
            {
                HandleOneLegCamera(isMoving);
                return;
            }
            bool isRunning = _inputReader.Current.RunHeld && _limbs != null && _limbs.HasBothLegs;
            if (isRunning)
            {
                _currentFrequency = _config.TwoLegsRunFrequency;
                _currentVerticalAmp = _config.TwoLegsRunVerticalAmplitude;
                _currentSideAmp = _config.TwoLegsRunSideAmplitude;
                _currentSmoothing = _config.TwoLegsRunSmoothing;
            }
            else
            {
                _currentFrequency = _config.TwoLegsWalkFrequency;
                _currentVerticalAmp = _config.TwoLegsWalkVerticalAmplitude;
                _currentSideAmp = _config.TwoLegsWalkSideAmplitude;
                _currentSmoothing = _config.TwoLegsWalkSmoothing;
            }
            if (!isMoving)
            {
                _bobTimer = 0f;
                _cameraTransform.localPosition = Vector3.Lerp(
                    _cameraTransform.localPosition, _originalLocalPosition, Time.deltaTime * _currentSmoothing);
                _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
                return;
            }
            _bobTimer += Time.deltaTime * _currentFrequency;
            float vertical = Mathf.Sin(_bobTimer) * _currentVerticalAmp;
            float side = Mathf.Sin(_bobTimer * 0.5f) * _currentSideAmp;
            Vector3 target = _originalLocalPosition + new Vector3(side, vertical, 0f);
            _cameraTransform.localPosition = Vector3.Lerp(
                _cameraTransform.localPosition, target, Time.deltaTime * _currentSmoothing);
            _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }
        private void HandleOneLegCamera(bool isMoving)
        {
            if (!isMoving)
            {
                _bobTimer = 0f;
                _cameraTransform.localPosition = Vector3.Lerp(
                    _cameraTransform.localPosition, _originalLocalPosition, Time.deltaTime * 8f);
                _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
                return;
            }
            _bobTimer += Time.deltaTime;
            float interval = Mathf.Max(0.05f, _config.OneLegHopInterval);
            float t = Mathf.Repeat(_bobTimer, interval) / interval;
            float vertical = SampleOneLegCycle(t) * _config.OneLegRise;
            float side = Mathf.Sin(t * Mathf.PI * 2f) * _config.OneLegSide;
            _cameraTransform.localPosition = _originalLocalPosition + new Vector3(side, vertical, 0f);
            _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }
        private float SampleOneLegCycle(float t)
        {
            const float pushEnd = 0.12f;
            const float riseEnd = 0.34f;
            const float dropEnd = 0.56f;
            if (t < pushEnd)
            {
                float u = t / pushEnd;
                return -Mathf.Sin(u * Mathf.PI) * (_config.OneLegPush / Mathf.Max(0.001f, _config.OneLegRise));
            }
            if (t < riseEnd)
            {
                float u = (t - pushEnd) / (riseEnd - pushEnd);
                return u * u;
            }
            if (t < dropEnd)
            {
                float u = (t - riseEnd) / (dropEnd - riseEnd);
                return Mathf.Lerp(1f, 0.05f, u * u);
            }
            float slow = (t - dropEnd) / (1f - dropEnd);
            return Mathf.Lerp(0.05f, 0.38f, slow * slow);
        }
        private void PreventCameraClip()
        {
            Camera cam = _cameraTransform.GetComponent<Camera>();
            float near = cam != null ? cam.nearClipPlane : 0.1f;
            float radius = Mathf.Max(0.05f, near * 0.8f);
            Vector3 origin = transform.position + Vector3.up * _originalLocalPosition.y;
            Vector3 desired = _cameraTransform.position;
            Vector3 lookPoint = desired + _cameraTransform.forward * (near + _clipPadding);
            Vector3 delta = lookPoint - origin;
            float distance = delta.magnitude;
            if (distance < 0.001f)
                return;
            int mask = _cameraCollision.value == 0 ? Physics.DefaultRaycastLayers : _cameraCollision.value;
            int held = LayerMask.NameToLayer("Held");
            if (held >= 0)
                mask &= ~(1 << held);
            mask &= ~(1 << gameObject.layer);
            Vector3 direction = delta / distance;
            if (Physics.SphereCast(origin, radius, direction, out RaycastHit hit, distance, mask, QueryTriggerInteraction.Ignore))
            {
                float allowed = Mathf.Max(0.02f, hit.distance - near - _clipPadding);
                float cameraDistance = Vector3.Distance(origin, desired);
                _cameraTransform.position = origin + (desired - origin).normalized * Mathf.Min(cameraDistance, allowed);
            }
        }
        private void OnDestroy()
        {
            if (_limbs != null)
                _limbs.OnLimbsChanged -= UpdateBobParameters;
        }
    }
}