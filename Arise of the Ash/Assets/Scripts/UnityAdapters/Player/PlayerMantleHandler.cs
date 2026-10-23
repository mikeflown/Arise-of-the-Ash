using System.Collections;
using Configs;
using Core.Player;
using UnityEngine;

namespace UnityAdapters.Player
{
    public class PlayerMantleHandler : MonoBehaviour
    {
        [SerializeField] private MovementConfig _config;
        [SerializeField] private PlayerInputReader _inputReader;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private CharacterController _controller;
        [SerializeField] private PlayerSlopeSlide _slopeSlide;
        [Header("Anti false-trigger")]
        [SerializeField] private float _maxUpwardVelocity = 2.0f;
        [SerializeField] private float _cooldown = 0.35f;
        [SerializeField] private float _ledgeNormalMinY = 0.65f;
        [SerializeField] private float _postMantleSlideLock = 0.6f;
        private PlayerLimbs _limbs;
        private bool _isMantling;
        private Coroutine _mantleRoutine;
        private float _lastMantleTime = -10f;
        private float _slideUnlockTime;
        private Vector3 _lastPos;
        public bool IsMantling => _isMantling;
        public event System.Action OnMantleStarted;
        public event System.Action OnMantleFinished;
        public void Init(PlayerLimbs limbs)
        {
            _limbs = limbs;
            if (_controller == null)
                _controller = GetComponent<CharacterController>();
            if (_slopeSlide == null)
                _slopeSlide = GetComponent<PlayerSlopeSlide>();
            _lastPos = transform.position;
        }
        private void Update()
        {
            if (_isMantling || _limbs == null || _config == null || _controller == null)
                return;
            if (Time.time - _lastMantleTime < _cooldown)
                return;
            if (_controller.isGrounded)
                return;
            Vector2 move = _inputReader != null ? _inputReader.Current.Move : Vector2.zero;
            if (move.y < 0.1f)
                return;
            TryDetectMantle();
        }
        private void TryDetectMantle()
        {
            Vector3 forward = transform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.01f)
                return;
            forward.Normalize();
            float verticalApprox = (transform.position.y - _lastPos.y) / Mathf.Max(Time.deltaTime, 0.001f);
            _lastPos = transform.position;
            if (verticalApprox > _maxUpwardVelocity)
                return;
            float checkDist = Mathf.Max(1.0f, _config.MantleCheckDistance);
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            if (!Physics.SphereCast(origin, 0.3f, forward, out RaycastHit wallHit,
                    checkDist, _config.MantleLayer, QueryTriggerInteraction.Ignore))
                return;
            float wallAngle = Vector3.Angle(wallHit.normal, Vector3.up);
            if (wallAngle < 50f)
                return;
            float maxH = _config.MantleMaxHeight;
            Vector3 ledgeProbe = wallHit.point + Vector3.up * maxH + forward * 0.2f;
            if (!Physics.Raycast(ledgeProbe, Vector3.down, out RaycastHit ledgeHit,
                    maxH + 0.8f, _config.MantleLayer, QueryTriggerInteraction.Ignore))
                return;
            if (ledgeHit.normal.y < _ledgeNormalMinY)
                return;
            float heightDiff = ledgeHit.point.y - transform.position.y;
            if (heightDiff < _config.MantleMinHeight || heightDiff > maxH)
                return;
            float forwardPush = Mathf.Clamp(_config.MantleForwardOffset, 0.15f, 0.35f);
            Vector3 standPos = ledgeHit.point + Vector3.up * 0.08f + forward * forwardPush;
            if (Physics.Raycast(standPos + Vector3.up * 0.5f, Vector3.down, out RaycastHit footHit,
                    0.9f, _config.MantleLayer, QueryTriggerInteraction.Ignore))
            {
                standPos.y = footHit.point.y + 0.08f;
                standPos.x = footHit.point.x;
                standPos.z = footHit.point.z;
                standPos += forward * 0.1f;
            }
            float radius = _controller.radius * 0.85f;
            float halfH = _controller.height * 0.45f;
            if (Physics.CheckSphere(standPos + Vector3.up * halfH, radius,
                    _config.MantleLayer, QueryTriggerInteraction.Ignore))
                return;
            StartMantle(standPos);
        }
        private void StartMantle(Vector3 targetPosition)
        {
            if (_mantleRoutine != null)
                StopCoroutine(_mantleRoutine);
            _isMantling = true;
            _lastMantleTime = Time.time;
            if (_slopeSlide != null)
            {
                _slopeSlide.SetSlideEnabled(false);
                _slopeSlide.enabled = false;
            }
            _cameraController?.SetLookEnabled(false);
            OnMantleStarted?.Invoke();
            _mantleRoutine = StartCoroutine(MantleRoutine(targetPosition));
        }
        private IEnumerator MantleRoutine(Vector3 target)
        {
            Vector3 start = transform.position;
            float duration = Mathf.Max(0.25f, _config.MantleDuration);
            float elapsed = 0f;
            _controller.enabled = false;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                Vector3 upPoint = new Vector3(start.x, target.y, start.z);
                Vector3 pos = t < 0.65f ? Vector3.Lerp(start, upPoint, t / 0.65f) : Vector3.Lerp(upPoint, target, (t - 0.65f) / 0.35f);
                transform.position = pos;
                yield return null;
            }
            transform.position = target;
            _controller.enabled = true;
            _isMantling = false;
            _cameraController?.SetLookEnabled(true);
            _slideUnlockTime = Time.time + _postMantleSlideLock;
            StartCoroutine(UnlockSlideAfterDelay());
            OnMantleFinished?.Invoke();
            _mantleRoutine = null;
        }
        private IEnumerator UnlockSlideAfterDelay()
        {
            while (Time.time < _slideUnlockTime)
                yield return null;
            if (_slopeSlide != null)
            {
                _slopeSlide.enabled = true;
                _slopeSlide.SetSlideEnabled(true);
            }
        }
        private void OnDisable()
        {
            if (_mantleRoutine != null)
            {
                StopCoroutine(_mantleRoutine);
                _mantleRoutine = null;
            }
            _isMantling = false;
            if (_controller != null)
                _controller.enabled = true;
            if (_slopeSlide != null)
            {
                _slopeSlide.enabled = true;
                _slopeSlide.SetSlideEnabled(true);
            }
            _cameraController?.SetLookEnabled(true);
        }
    }
}