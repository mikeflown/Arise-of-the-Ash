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
        private PlayerLimbs _limbs;
        private bool _isMantling;
        private Coroutine _mantleRoutine;
        public bool IsMantling => _isMantling;
        public event System.Action OnMantleStarted;
        public event System.Action OnMantleFinished;
        public void Init(PlayerLimbs limbs)
        {
            _limbs = limbs;
            if (_controller == null)
                _controller = GetComponent<CharacterController>();
        }
        private void Update()
        {
            if (_isMantling || _limbs == null || _config == null)
                return;
            if (!_controller.isGrounded)
                TryDetectMantle();
        }
        private void TryDetectMantle()
        {
            Vector3 forward = transform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.01f)
                return;
            forward.Normalize();
            Vector3 origin = transform.position + Vector3.up * 0.3f;
            if (!Physics.Raycast(origin, forward, out RaycastHit wallHit,
                    _config.MantleCheckDistance, _config.MantleLayer, QueryTriggerInteraction.Ignore))
                return;
            Vector3 ledgeProbe = wallHit.point + Vector3.up * _config.MantleMaxHeight + forward * 0.15f;
            if (!Physics.Raycast(ledgeProbe, Vector3.down, out RaycastHit ledgeHit,
                    _config.MantleMaxHeight + 0.4f, _config.MantleLayer, QueryTriggerInteraction.Ignore))
                return;
            float heightDiff = ledgeHit.point.y - transform.position.y;
            if (heightDiff < _config.MantleMinHeight || heightDiff > _config.MantleMaxHeight)
                return;
            Vector3 standPos = ledgeHit.point + Vector3.up * 0.05f;
            if (Physics.CheckSphere(standPos + Vector3.up * 0.9f, 0.3f, _config.MantleLayer, QueryTriggerInteraction.Ignore))
                return;
            StartMantle(standPos + forward * _config.MantleForwardOffset);
        }
        private void StartMantle(Vector3 targetPosition)
        {
            if (_mantleRoutine != null)
                StopCoroutine(_mantleRoutine);
            _isMantling = true;
            _cameraController?.SetLookEnabled(false);
            OnMantleStarted?.Invoke();
            _mantleRoutine = StartCoroutine(MantleRoutine(targetPosition));
        }
        private IEnumerator MantleRoutine(Vector3 target)
        {
            Vector3 start = transform.position;
            float elapsed = 0f;
            float duration = Mathf.Max(0.15f, _config.MantleDuration);
            _controller.enabled = false;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                Vector3 mid = new Vector3(start.x, target.y, start.z);
                Vector3 pos = t < 0.55f
                    ? Vector3.Lerp(start, mid, t / 0.55f)
                    : Vector3.Lerp(mid, target, (t - 0.55f) / 0.45f);
                transform.position = pos;
                yield return null;
            }
            transform.position = target;
            _controller.enabled = true;
            _isMantling = false;
            _cameraController?.SetLookEnabled(true);
            OnMantleFinished?.Invoke();
            _mantleRoutine = null;
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
            _cameraController?.SetLookEnabled(true);
        }
    }
}