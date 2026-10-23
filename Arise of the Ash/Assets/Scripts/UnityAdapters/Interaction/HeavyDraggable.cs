using Core.Interaction;
using Core.Player;
using UnityEngine;

namespace UnityAdapters.Interaction
{
    [RequireComponent(typeof(Rigidbody))]
    public class HeavyDraggable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _prompt = "Тащить";
        [SerializeField] private float _mass = 60f;
        private Rigidbody _body;
        private Collider[] _cols;
        private Collider _playerCol;
        private bool _ignoringPlayer;
        private float _lockedY;
        public string InteractionPrompt => _prompt;
        public bool IsBeingDragged { get; private set; }
        public float Mass => _mass;
        public Vector3 Position => _body != null ? _body.position : transform.position;
        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _cols = GetComponentsInChildren<Collider>();
            _body.mass = _mass;
            _body.useGravity = false;
            _body.isKinematic = false;
            _body.interpolation = RigidbodyInterpolation.Interpolate;
            _body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _body.constraints = RigidbodyConstraints.FreezeRotation;
        }
        public bool CanInteract(PlayerLimbs limbs)
        {
            return !IsBeingDragged && limbs != null && limbs.HasBothArms;
        }
        public void Interact(PlayerContext context) { }
        public void BeginDrag(Collider playerCollider)
        {
            IsBeingDragged = true;
            _lockedY = _body.position.y;
            _body.isKinematic = true;
            if (playerCollider != null)
            {
                _playerCol = playerCollider;
                SetIgnorePlayer(true);
            }
        }
        public void EndDrag()
        {
            IsBeingDragged = false;
            SetIgnorePlayer(false);
            _body.isKinematic = false;
        }
        public void MoveTogether(Vector3 worldDelta)
        {
            if (!IsBeingDragged)
                return;
            worldDelta.y = 0f;
            Vector3 pos = _body.position + worldDelta;
            pos.y = _lockedY;
            _body.MovePosition(pos);
        }
        private void SetIgnorePlayer(bool ignore)
        {
            if (_playerCol == null || _cols == null)
                return;
            foreach (var c in _cols)
            {
                if (c != null)
                    Physics.IgnoreCollision(c, _playerCol, ignore);
            }
            _ignoringPlayer = ignore;
        }
        private void OnDisable()
        {
            if (_ignoringPlayer)
                SetIgnorePlayer(false);
        }
    }
}